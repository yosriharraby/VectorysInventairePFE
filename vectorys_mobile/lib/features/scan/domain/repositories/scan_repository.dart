import 'package:vectorys_mobile/features/scan/domain/entities/scan_result.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/traiter_scan_result.dart';

abstract class ScanRepository {
  Future<ScanResult> enregistrerScan({
    required String codeBarresScan,
    required int emplacementId,
    String? idLocalFlutter,
  });

  Future<TraiterScanTerrainResult> traiterScanTerrain({
    required String codeBarresScan,
    int? emplacementActifId,
    String? idLocalFlutter,
  });

  Future<FinComptageResult> finComptage(int emplacementId);
}
