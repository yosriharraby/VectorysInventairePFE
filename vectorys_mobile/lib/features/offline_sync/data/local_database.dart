import 'package:path/path.dart' as p;
import 'package:sqflite/sqflite.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';

class LocalDatabase {
  Database? _db;

  Future<void> init() async {
    if (_db != null) return;
    final basePath = await getDatabasesPath();
    _db = await openDatabase(
      p.join(basePath, 'vectorys_offline.db'),
      version: 2,
      onCreate: (db, version) async {
        await _createSchema(db);
      },
      onUpgrade: (db, oldVersion, newVersion) async {
        if (oldVersion < 2) {
          await db.execute('''
            CREATE TABLE IF NOT EXISTS cached_stock_lines (
              id INTEGER PRIMARY KEY AUTOINCREMENT,
              emplacement_id INTEGER NOT NULL,
              code_us TEXT NOT NULL,
              designation_us TEXT NOT NULL,
              qte_theorique INTEGER NOT NULL,
              qte_scannee INTEGER NOT NULL DEFAULT 0,
              UNIQUE(emplacement_id, code_us)
            )
          ''');
        }
      },
    );
  }

  Future<void> _createSchema(Database db) async {
    await db.execute('''
      CREATE TABLE cached_emplacements (
        id INTEGER PRIMARY KEY,
        code TEXT NOT NULL,
        designation TEXT NOT NULL,
        statut TEXT NOT NULL,
        code_barres TEXT NOT NULL,
        qte_attendue INTEGER NOT NULL,
        qte_scannee INTEGER NOT NULL,
        inventaire_id INTEGER NOT NULL,
        inventaire_reference TEXT NOT NULL,
        inventaire_intitule TEXT NOT NULL,
        inventaire_zone TEXT NOT NULL,
        cached_at TEXT NOT NULL
      )
    ''');
    await db.execute('''
      CREATE TABLE cached_stock_lines (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        emplacement_id INTEGER NOT NULL,
        code_us TEXT NOT NULL,
        designation_us TEXT NOT NULL,
        qte_theorique INTEGER NOT NULL,
        qte_scannee INTEGER NOT NULL DEFAULT 0,
        UNIQUE(emplacement_id, code_us)
      )
    ''');
    await db.execute('''
      CREATE TABLE pending_scans (
        id_local TEXT PRIMARY KEY,
        emplacement_id INTEGER NOT NULL,
        code_barres_scan TEXT NOT NULL,
        date_heure_scan TEXT NOT NULL,
        created_at TEXT NOT NULL
      )
    ''');
    await db.execute('''
      CREATE TABLE pending_fin_comptage (
        emplacement_id INTEGER PRIMARY KEY,
        created_at TEXT NOT NULL
      )
    ''');
  }

  Database get db {
    final database = _db;
    if (database == null) throw StateError('LocalDatabase not initialized');
    return database;
  }

  Future<void> replaceCachedEmplacements(List<MesEmplacement> list) async {
    final now = DateTime.now().toUtc().toIso8601String();
    await db.transaction((txn) async {
      await txn.delete('cached_emplacements');
      await txn.delete('cached_stock_lines');
      for (final e in list) {
        await txn.insert('cached_emplacements', {
          'id': e.id,
          'code': e.code,
          'designation': e.designation,
          'statut': e.statut,
          'code_barres': e.codeBarres,
          'qte_attendue': e.qteAttendue,
          'qte_scannee': e.qteScannee,
          'inventaire_id': e.inventaireId,
          'inventaire_reference': e.inventaireReference,
          'inventaire_intitule': e.inventaireIntitule,
          'inventaire_zone': e.inventaireZone,
          'cached_at': now,
        });
        for (final ligne in e.lignesStock) {
          await txn.insert('cached_stock_lines', {
            'emplacement_id': e.id,
            'code_us': ligne.codeUs,
            'designation_us': ligne.designationUs,
            'qte_theorique': ligne.qteTheorique,
            'qte_scannee': ligne.qteScannee,
          });
        }
      }
    });
  }

  Future<List<StockLigneProgress>> getCachedStockLines(int emplacementId) async {
    final pendingByCode = await _pendingScanCountsByEmplacementAndCode(emplacementId);
    final rows = await db.query(
      'cached_stock_lines',
      where: 'emplacement_id = ?',
      whereArgs: [emplacementId],
      orderBy: 'code_us ASC',
    );
    return rows
        .map(
          (row) => StockLigneProgress(
            codeUs: row['code_us'] as String,
            designationUs: row['designation_us'] as String,
            qteTheorique: row['qte_theorique'] as int,
            qteScannee: (row['qte_scannee'] as int) + (pendingByCode[row['code_us'] as String] ?? 0),
          ),
        )
        .toList(growable: false);
  }

  Future<List<MesEmplacement>> getCachedEmplacements() async {
    final rows = await db.query('cached_emplacements', orderBy: 'code ASC');
    final pendingCounts = await _pendingScanCountsByEmplacement();
    final result = <MesEmplacement>[];
    for (final row in rows) {
      final id = row['id'] as int;
      final pending = pendingCounts[id] ?? 0;
      final serverScanned = row['qte_scannee'] as int;
      final lignes = await getCachedStockLines(id);
      result.add(
        MesEmplacement(
          id: id,
          code: row['code'] as String,
          designation: row['designation'] as String,
          statut: row['statut'] as String,
          codeBarres: row['code_barres'] as String,
          qteAttendue: row['qte_attendue'] as int,
          qteScannee: serverScanned + pending,
          inventaireId: row['inventaire_id'] as int,
          inventaireReference: row['inventaire_reference'] as String,
          inventaireIntitule: row['inventaire_intitule'] as String,
          inventaireZone: row['inventaire_zone'] as String,
          lignesStock: lignes,
        ),
      );
    }
    return result;
  }

  Future<Map<int, int>> _pendingScanCountsByEmplacement() async {
    final rows = await db.rawQuery(
      'SELECT emplacement_id, COUNT(*) as c FROM pending_scans GROUP BY emplacement_id',
    );
    return {
      for (final row in rows) row['emplacement_id'] as int: row['c'] as int,
    };
  }

  Future<Map<String, int>> _pendingScanCountsByEmplacementAndCode(int emplacementId) async {
    final rows = await db.rawQuery(
      'SELECT code_barres_scan, COUNT(*) as c FROM pending_scans WHERE emplacement_id = ? GROUP BY code_barres_scan',
      [emplacementId],
    );
    return {
      for (final row in rows) row['code_barres_scan'] as String: row['c'] as int,
    };
  }

  Future<void> insertPendingScan({
    required String idLocal,
    required int emplacementId,
    required String codeBarresScan,
    required DateTime dateHeureScan,
  }) async {
    await db.insert('pending_scans', {
      'id_local': idLocal,
      'emplacement_id': emplacementId,
      'code_barres_scan': codeBarresScan,
      'date_heure_scan': dateHeureScan.toUtc().toIso8601String(),
      'created_at': DateTime.now().toUtc().toIso8601String(),
    });
    await db.update(
      'cached_emplacements',
      {'statut': 'EN_COURS'},
      where: 'id = ? AND statut = ?',
      whereArgs: [emplacementId, 'EN_ATTENTE'],
    );
    await db.rawUpdate(
      '''
      UPDATE cached_stock_lines
      SET qte_scannee = qte_scannee + 1
      WHERE emplacement_id = ? AND code_us = ?
      ''',
      [emplacementId, codeBarresScan.trim()],
    );
  }

  Future<MesEmplacement?> findCachedEmplacementByBarcode(String code) async {
    final normalized = code.trim();
    final list = await getCachedEmplacements();
    for (final e in list) {
      if (e.isTermine) continue;
      if (_equalsIgnoreCase(e.code, normalized) || _equalsIgnoreCase(e.codeBarres, normalized)) {
        return e;
      }
    }
    return null;
  }

  Future<void> activateCachedEmplacement(int emplacementId) async {
    await db.update(
      'cached_emplacements',
      {'statut': 'EN_COURS'},
      where: 'id = ? AND statut = ?',
      whereArgs: [emplacementId, 'EN_ATTENTE'],
    );
  }

  bool _equalsIgnoreCase(String a, String b) => a.toLowerCase() == b.toLowerCase();

  Future<({bool allowed, String? reason})> validateOfflineScan({
    required int emplacementId,
    required String codeBarresScan,
  }) async {
    final emplacement = await db.query(
      'cached_emplacements',
      where: 'id = ?',
      whereArgs: [emplacementId],
      limit: 1,
    );
    if (emplacement.isEmpty) {
      return (allowed: false, reason: 'Emplacement introuvable en cache local.');
    }
    if ((emplacement.first['statut'] as String).toUpperCase() == 'TERMINE') {
      return (allowed: false, reason: 'Cet emplacement est déjà terminé.');
    }

    return (allowed: true, reason: null);
  }

  Future<List<Map<String, dynamic>>> getPendingScans() async {
    return db.query('pending_scans', orderBy: 'created_at ASC');
  }

  Future<void> deletePendingScan(String idLocal) async {
    await db.delete('pending_scans', where: 'id_local = ?', whereArgs: [idLocal]);
  }

  Future<int> countPendingScans() async {
    final row = await db.rawQuery('SELECT COUNT(*) as c FROM pending_scans');
    return Sqflite.firstIntValue(row) ?? 0;
  }

  Future<void> insertPendingFinComptage(int emplacementId) async {
    await db.insert(
      'pending_fin_comptage',
      {
        'emplacement_id': emplacementId,
        'created_at': DateTime.now().toUtc().toIso8601String(),
      },
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    await db.update(
      'cached_emplacements',
      {'statut': 'TERMINE'},
      where: 'id = ?',
      whereArgs: [emplacementId],
    );
  }

  Future<List<int>> getPendingFinComptageIds() async {
    final rows = await db.query('pending_fin_comptage', orderBy: 'created_at ASC');
    return rows.map((r) => r['emplacement_id'] as int).toList(growable: false);
  }

  Future<void> deletePendingFinComptage(int emplacementId) async {
    await db.delete(
      'pending_fin_comptage',
      where: 'emplacement_id = ?',
      whereArgs: [emplacementId],
    );
  }

  Future<int> countPendingFinComptage() async {
    final row = await db.rawQuery('SELECT COUNT(*) as c FROM pending_fin_comptage');
    return Sqflite.firstIntValue(row) ?? 0;
  }

  Future<void> updateCachedEmplacementProgress({
    required int emplacementId,
    required int qteScannee,
    required String statut,
    List<StockLigneProgress>? lignesStock,
  }) async {
    await db.update(
      'cached_emplacements',
      {'qte_scannee': qteScannee, 'statut': statut},
      where: 'id = ?',
      whereArgs: [emplacementId],
    );
    if (lignesStock != null) {
      for (final ligne in lignesStock) {
        await db.update(
          'cached_stock_lines',
          {'qte_scannee': ligne.qteScannee},
          where: 'emplacement_id = ? AND code_us = ?',
          whereArgs: [emplacementId, ligne.codeUs],
        );
      }
    }
  }

  Future<MesEmplacement?> getCachedEmplacement(int emplacementId) async {
    final list = await getCachedEmplacements();
    for (final e in list) {
      if (e.id == emplacementId) return e;
    }
    return null;
  }

  Future<void> clearAll() async {
    await db.delete('cached_emplacements');
    await db.delete('cached_stock_lines');
    await db.delete('pending_scans');
    await db.delete('pending_fin_comptage');
  }
}
