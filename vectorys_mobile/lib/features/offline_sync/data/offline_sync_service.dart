import 'dart:async';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:vectorys_mobile/features/offline_sync/data/connectivity_service.dart';
import 'package:vectorys_mobile/features/offline_sync/data/local_database.dart';
import 'package:vectorys_mobile/features/offline_sync/domain/entities/sync_result.dart';
import 'package:vectorys_mobile/features/scan/data/scan_repository_impl.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';

class OfflineSyncService extends ChangeNotifier {
  OfflineSyncService({
    required Dio dio,
    required LocalDatabase localDatabase,
    required ConnectivityService connectivityService,
  })  : _dio = dio,
        _db = localDatabase,
        _connectivity = connectivityService;

  final Dio _dio;
  final LocalDatabase _db;
  final ConnectivityService _connectivity;
  StreamSubscription<bool>? _connectivitySub;

  bool _syncing = false;
  String? _lastSyncMessage;

  bool get isOnline => _connectivity.isOnline;
  bool get isSyncing => _syncing;
  String? get lastSyncMessage => _lastSyncMessage;

  Future<void> init() async {
    await _connectivity.init();
    _connectivitySub = _connectivity.onConnectivityChanged.listen((online) {
      notifyListeners();
      if (online) {
        unawaited(syncPending());
      }
    });
    await refreshPendingCounts();
  }

  Future<int> pendingScansCount() => _db.countPendingScans();

  Future<int> pendingFinComptageCount() => _db.countPendingFinComptage();

  Future<int> totalPendingCount() async {
    final scans = await pendingScansCount();
    final fin = await pendingFinComptageCount();
    return scans + fin;
  }

  Future<void> refreshPendingCounts() async {
    notifyListeners();
  }

  bool isNetworkError(DioException e) {
    return e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout ||
        e.type == DioExceptionType.receiveTimeout ||
        e.type == DioExceptionType.sendTimeout;
  }

  Future<SyncResult> syncPending() async {
    if (_syncing || !_connectivity.isOnline) {
      return const SyncResult(
        scansSynced: 0,
        scansFailed: 0,
        scansDuplicates: 0,
        finComptageSynced: 0,
        finComptageFailed: 0,
      );
    }

    _syncing = true;
    _lastSyncMessage = null;
    notifyListeners();

    var scansSynced = 0;
    var scansFailed = 0;
    var scansDuplicates = 0;
    var finSynced = 0;
    var finFailed = 0;

    try {
      final pending = await _db.getPendingScans();
      if (pending.isNotEmpty) {
        final payload = {
          'scans': pending
              .map(
                (row) => {
                  'codeBarresScan': row['code_barres_scan'],
                  'emplacementId': row['emplacement_id'],
                  'dateHeureScan': row['date_heure_scan'],
                  'idLocalFlutter': row['id_local'],
                },
              )
              .toList(),
        };

        final response = await _dio.post<Map<String, dynamic>>('/scans/batch', data: payload);
        final data = response.data ?? const {};
        final items = (data['items'] as List<dynamic>? ?? const []);

        for (final raw in items) {
          if (raw is! Map<String, dynamic>) continue;
          final idLocal = raw['idLocalFlutter'] as String?;
          final success = raw['success'] as bool? ?? false;
          final isDuplicate = raw['isDuplicate'] as bool? ?? false;

          if (success && idLocal != null) {
            await _db.deletePendingScan(idLocal);
            if (isDuplicate) {
              scansDuplicates++;
            } else {
              scansSynced++;
              final result = raw['result'] as Map<String, dynamic>?;
              if (result != null) {
                final rawLignes = result['lignesStock'] as List<dynamic>? ?? const [];
                await _db.updateCachedEmplacementProgress(
                  emplacementId: (result['emplacementId'] as num).toInt(),
                  qteScannee: (result['qteScannee'] as num?)?.toInt() ?? 0,
                  statut: result['emplacementStatut'] as String? ?? 'EN_COURS',
                  lignesStock: rawLignes
                      .whereType<Map<String, dynamic>>()
                      .map(StockLigneProgress.fromJson)
                      .toList(growable: false),
                );
              }
            }
          } else {
            scansFailed++;
          }
        }
      }

      final finIds = await _db.getPendingFinComptageIds();
      final scanRepo = ScanRepositoryImpl(_dio);
      for (final emplacementId in finIds) {
        try {
          await scanRepo.finComptage(emplacementId);
          await _db.deletePendingFinComptage(emplacementId);
          await _db.updateCachedEmplacementProgress(
            emplacementId: emplacementId,
            qteScannee: (await _db.getCachedEmplacement(emplacementId))?.qteScannee ?? 0,
            statut: 'TERMINE',
          );
          finSynced++;
        } catch (_) {
          finFailed++;
        }
      }

      _lastSyncMessage = finFailed > 0 || scansFailed > 0
          ? 'Synchronisation partielle'
          : 'Synchronisation réussie';
    } on DioException catch (e) {
      _lastSyncMessage = isNetworkError(e)
          ? 'Réseau indisponible'
          : (e.response?.data is Map ? (e.response!.data['message'] as String?) : null) ??
              'Erreur de synchronisation';
    } catch (e) {
      _lastSyncMessage = 'Erreur : $e';
    } finally {
      _syncing = false;
      notifyListeners();
    }

    return SyncResult(
      scansSynced: scansSynced,
      scansFailed: scansFailed,
      scansDuplicates: scansDuplicates,
      finComptageSynced: finSynced,
      finComptageFailed: finFailed,
    );
  }

  @override
  void dispose() {
    _connectivitySub?.cancel();
    _connectivity.dispose();
    super.dispose();
  }
}
