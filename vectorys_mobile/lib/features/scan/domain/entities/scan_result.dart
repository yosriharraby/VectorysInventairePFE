import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';

class ScanResult {
  const ScanResult({
    required this.scanId,
    required this.emplacementId,
    required this.inventaireId,
    required this.codeBarresScan,
    required this.qteScannee,
    required this.qteAttendue,
    required this.emplacementStatut,
    this.autoTermine = false,
    this.lignesStock = const [],
  });

  final int scanId;
  final int emplacementId;
  final int inventaireId;
  final String codeBarresScan;
  final int qteScannee;
  final int qteAttendue;
  final String emplacementStatut;
  final bool autoTermine;
  final List<StockLigneProgress> lignesStock;

  factory ScanResult.fromJson(Map<String, dynamic> json) {
    final rawLignes = json['lignesStock'] as List<dynamic>? ?? const [];
    return ScanResult(
      scanId: (json['scanId'] as num?)?.toInt() ?? 0,
      emplacementId: (json['emplacementId'] as num).toInt(),
      inventaireId: (json['inventaireId'] as num).toInt(),
      codeBarresScan: json['codeBarresScan'] as String? ?? '',
      qteScannee: (json['qteScannee'] as num?)?.toInt() ?? 0,
      qteAttendue: (json['qteAttendue'] as num?)?.toInt() ?? 0,
      emplacementStatut: json['emplacementStatut'] as String? ?? '',
      autoTermine: json['autoTermine'] as bool? ?? false,
      lignesStock: rawLignes
          .whereType<Map<String, dynamic>>()
          .map(StockLigneProgress.fromJson)
          .toList(growable: false),
    );
  }
}

class FinComptageResult {
  const FinComptageResult({
    required this.emplacementId,
    required this.inventaireId,
    required this.statut,
    required this.qteScannee,
    required this.qteAttendue,
  });

  final int emplacementId;
  final int inventaireId;
  final String statut;
  final int qteScannee;
  final int qteAttendue;

  factory FinComptageResult.fromJson(Map<String, dynamic> json) => FinComptageResult(
        emplacementId: (json['emplacementId'] as num).toInt(),
        inventaireId: (json['inventaireId'] as num).toInt(),
        statut: json['statut'] as String? ?? 'TERMINE',
        qteScannee: (json['qteScannee'] as num?)?.toInt() ?? 0,
        qteAttendue: (json['qteAttendue'] as num?)?.toInt() ?? 0,
      );
}
