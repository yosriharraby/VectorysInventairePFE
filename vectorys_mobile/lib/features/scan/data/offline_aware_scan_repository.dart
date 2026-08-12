import 'package:dio/dio.dart';
import 'package:uuid/uuid.dart';
import 'package:vectorys_mobile/features/offline_sync/data/local_database.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/scan_result.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/traiter_scan_result.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/scan_repository.dart';

class OfflineAwareScanRepository implements ScanRepository {
  OfflineAwareScanRepository({
    required ScanRepository remote,
    required OfflineSyncService offlineSync,
    required LocalDatabase localDatabase,
  })  : _remote = remote,
        _offlineSync = offlineSync,
        _db = localDatabase;

  final ScanRepository _remote;
  final OfflineSyncService _offlineSync;
  final LocalDatabase _db;
  final _uuid = const Uuid();

  @override
  Future<ScanResult> enregistrerScan({
    required String codeBarresScan,
    required int emplacementId,
    String? idLocalFlutter,
  }) async {
    final idLocal = idLocalFlutter ?? _uuid.v4();

    if (_offlineSync.isOnline) {
      try {
        final result = await _remote.enregistrerScan(
          codeBarresScan: codeBarresScan,
          emplacementId: emplacementId,
          idLocalFlutter: idLocal,
        );
        await _db.updateCachedEmplacementProgress(
          emplacementId: emplacementId,
          qteScannee: result.qteScannee,
          statut: result.emplacementStatut,
          lignesStock: result.lignesStock,
        );
        return result;
      } on DioException catch (e) {
        if (!_offlineSync.isNetworkError(e)) rethrow;
      }
    }

    return _registerUsOffline(
      codeBarresScan: codeBarresScan,
      emplacementId: emplacementId,
      idLocal: idLocal,
    );
  }

  @override
  Future<TraiterScanTerrainResult> traiterScanTerrain({
    required String codeBarresScan,
    int? emplacementActifId,
    String? idLocalFlutter,
  }) async {
    final idLocal = idLocalFlutter ?? _uuid.v4();

    if (_offlineSync.isOnline) {
      try {
        final result = await _remote.traiterScanTerrain(
          codeBarresScan: codeBarresScan,
          emplacementActifId: emplacementActifId,
          idLocalFlutter: idLocal,
        );
        await _syncCacheFromTerrainResult(result);
        return result;
      } on DioException catch (e) {
        if (!_offlineSync.isNetworkError(e)) rethrow;
      }
    }

    return _traiterScanTerrainOffline(
      codeBarresScan: codeBarresScan,
      emplacementActifId: emplacementActifId,
      idLocal: idLocal,
    );
  }

  @override
  Future<FinComptageResult> finComptage(int emplacementId) async {
    if (_offlineSync.isOnline) {
      try {
        final result = await _remote.finComptage(emplacementId);
        await _db.updateCachedEmplacementProgress(
          emplacementId: emplacementId,
          qteScannee: result.qteScannee,
          statut: result.statut,
        );
        return result;
      } on DioException catch (e) {
        if (!_offlineSync.isNetworkError(e)) rethrow;
      }
    }

    await _db.insertPendingFinComptage(emplacementId);
    await _offlineSync.refreshPendingCounts();
    final cached = await _db.getCachedEmplacement(emplacementId);
    return FinComptageResult(
      emplacementId: emplacementId,
      inventaireId: cached?.inventaireId ?? 0,
      statut: 'TERMINE',
      qteScannee: cached?.qteScannee ?? 0,
      qteAttendue: cached?.qteAttendue ?? 0,
    );
  }

  Future<void> _syncCacheFromTerrainResult(TraiterScanTerrainResult result) async {
    if (result.emplacementPrecedentTermine && result.emplacementPrecedentId != null) {
      final previous = await _db.getCachedEmplacement(result.emplacementPrecedentId!);
      if (previous != null) {
        await _db.updateCachedEmplacementProgress(
          emplacementId: previous.id,
          qteScannee: previous.qteScannee,
          statut: 'TERMINE',
        );
      }
    }

    await _db.updateCachedEmplacementProgress(
      emplacementId: result.emplacementActifId,
      qteScannee: result.qteScannee,
      statut: result.emplacementStatut,
      lignesStock: result.scanUs?.lignesStock,
    );
  }

  Future<TraiterScanTerrainResult> _traiterScanTerrainOffline({
    required String codeBarresScan,
    required int? emplacementActifId,
    required String idLocal,
  }) async {
    final code = codeBarresScan.trim();
    final matched = await _db.findCachedEmplacementByBarcode(code);

    if (matched != null) {
      if (matched.isTermine) {
        throw DioException(
          requestOptions: RequestOptions(path: '/scans/traiter'),
          response: Response(
            requestOptions: RequestOptions(path: '/scans/traiter'),
            statusCode: 400,
            data: {'message': 'L\'emplacement « ${matched.code} » est déjà terminé.'},
          ),
          type: DioExceptionType.badResponse,
        );
      }

      if (emplacementActifId == matched.id) {
        return _buildEmplacementResult(
          TraiterScanTerrainResult.typeEmplacementDejaActif,
          matched,
          message: 'Emplacement « ${matched.code} » déjà actif.',
        );
      }

      int? precedentId;
      String? precedentCode;
      var precedentTermine = false;

      if (emplacementActifId != null && emplacementActifId != matched.id) {
        await _db.insertPendingFinComptage(emplacementActifId);
        final previous = await _db.getCachedEmplacement(emplacementActifId);
        precedentId = emplacementActifId;
        precedentCode = previous?.code;
        precedentTermine = true;
      }

      await _db.activateCachedEmplacement(matched.id);
      final refreshed = await _db.getCachedEmplacement(matched.id) ?? matched;

      final type = precedentTermine
          ? TraiterScanTerrainResult.typeEmplacementBascule
          : TraiterScanTerrainResult.typeEmplacementOuvert;

      final message = precedentTermine
          ? 'Emplacement « $precedentCode » terminé (local). Comptage ouvert sur « ${refreshed.code} ».'
          : 'Comptage ouvert sur l\'emplacement « ${refreshed.code} » (hors ligne).';

      await _offlineSync.refreshPendingCounts();

      return _buildEmplacementResult(
        type,
        refreshed,
        emplacementPrecedentId: precedentId,
        emplacementPrecedentCode: precedentCode,
        emplacementPrecedentTermine: precedentTermine,
        message: message,
      );
    }

    if (emplacementActifId == null) {
      throw DioException(
        requestOptions: RequestOptions(path: '/scans/traiter'),
        response: Response(
          requestOptions: RequestOptions(path: '/scans/traiter'),
          statusCode: 400,
          data: {'message': 'Scannez d\'abord le code-barres de l\'emplacement pour démarrer le comptage.'},
        ),
        type: DioExceptionType.badResponse,
      );
    }

    final scan = await _registerUsOffline(
      codeBarresScan: code,
      emplacementId: emplacementActifId,
      idLocal: idLocal,
    );
    final actif = await _db.getCachedEmplacement(emplacementActifId);

    return TraiterScanTerrainResult(
      type: TraiterScanTerrainResult.typeUsEnregistre,
      emplacementActifId: emplacementActifId,
      emplacementActifCode: actif?.code ?? '',
      emplacementActifDesignation: actif?.designation ?? '',
      emplacementStatut: scan.emplacementStatut,
      qteScannee: scan.qteScannee,
      qteAttendue: scan.qteAttendue,
      scanUs: scan,
      message: 'US « $code » enregistré localement.',
    );
  }

  Future<ScanResult> _registerUsOffline({
    required String codeBarresScan,
    required int emplacementId,
    required String idLocal,
  }) async {
    final validation = await _db.validateOfflineScan(
      emplacementId: emplacementId,
      codeBarresScan: codeBarresScan,
    );
    if (!validation.allowed) {
      throw DioException(
        requestOptions: RequestOptions(path: '/scans'),
        response: Response(
          requestOptions: RequestOptions(path: '/scans'),
          statusCode: 400,
          data: {'message': validation.reason},
        ),
        type: DioExceptionType.badResponse,
      );
    }

    await _db.insertPendingScan(
      idLocal: idLocal,
      emplacementId: emplacementId,
      codeBarresScan: codeBarresScan,
      dateHeureScan: DateTime.now(),
    );
    await _offlineSync.refreshPendingCounts();

    final cached = await _db.getCachedEmplacement(emplacementId);
    final lignes = await _db.getCachedStockLines(emplacementId);

    return ScanResult(
      scanId: 0,
      emplacementId: emplacementId,
      inventaireId: cached?.inventaireId ?? 0,
      codeBarresScan: codeBarresScan,
      qteScannee: cached?.qteScannee ?? 0,
      qteAttendue: cached?.qteAttendue ?? 0,
      emplacementStatut: cached?.statut ?? 'EN_COURS',
      autoTermine: false,
      lignesStock: lignes,
    );
  }

  TraiterScanTerrainResult _buildEmplacementResult(
    String type,
    MesEmplacement emplacement, {
    int? emplacementPrecedentId,
    String? emplacementPrecedentCode,
    bool emplacementPrecedentTermine = false,
    String? message,
  }) {
    return TraiterScanTerrainResult(
      type: type,
      emplacementActifId: emplacement.id,
      emplacementActifCode: emplacement.code,
      emplacementActifDesignation: emplacement.designation,
      emplacementStatut: emplacement.statut,
      qteScannee: emplacement.qteScannee,
      qteAttendue: emplacement.qteAttendue,
      emplacementPrecedentId: emplacementPrecedentId,
      emplacementPrecedentCode: emplacementPrecedentCode,
      emplacementPrecedentTermine: emplacementPrecedentTermine,
      message: message,
    );
  }
}
