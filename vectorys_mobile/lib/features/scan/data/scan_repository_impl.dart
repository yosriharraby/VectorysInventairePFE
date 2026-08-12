import 'package:dio/dio.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/scan_result.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/traiter_scan_result.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/scan_repository.dart';

class ScanRepositoryImpl implements ScanRepository {
  ScanRepositoryImpl(this._dio);

  final Dio _dio;

  @override
  Future<ScanResult> enregistrerScan({
    required String codeBarresScan,
    required int emplacementId,
    String? idLocalFlutter,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '/scans',
      data: {
        'codeBarresScan': codeBarresScan,
        'emplacementId': emplacementId,
        'dateHeureScan': DateTime.now().toUtc().toIso8601String(),
        'idLocalFlutter': ?idLocalFlutter,
      },
    );
    return ScanResult.fromJson(response.data ?? const {});
  }

  @override
  Future<TraiterScanTerrainResult> traiterScanTerrain({
    required String codeBarresScan,
    int? emplacementActifId,
    String? idLocalFlutter,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '/scans/traiter',
      data: {
        'codeBarresScan': codeBarresScan,
        'emplacementActifId': emplacementActifId,
        'dateHeureScan': DateTime.now().toUtc().toIso8601String(),
        'idLocalFlutter': idLocalFlutter,
      },
    );
    return TraiterScanTerrainResult.fromJson(response.data ?? const {});
  }

  @override
  Future<FinComptageResult> finComptage(int emplacementId) async {
    final response = await _dio.put<Map<String, dynamic>>(
      '/scans/$emplacementId/fin-comptage',
    );
    return FinComptageResult.fromJson(response.data ?? const {});
  }
}
