import 'package:vectorys_mobile/features/scan/domain/entities/scan_result.dart';

class TraiterScanTerrainResult {
  const TraiterScanTerrainResult({
    required this.type,
    required this.emplacementActifId,
    required this.emplacementActifCode,
    required this.emplacementActifDesignation,
    required this.emplacementStatut,
    required this.qteScannee,
    required this.qteAttendue,
    this.emplacementPrecedentId,
    this.emplacementPrecedentCode,
    this.emplacementPrecedentTermine = false,
    this.scanUs,
    this.message,
  });

  static const typeEmplacementOuvert = 'EMPLACEMENT_OUVERT';
  static const typeEmplacementBascule = 'EMPLACEMENT_BASCULE';
  static const typeEmplacementDejaActif = 'EMPLACEMENT_DEJA_ACTIF';
  static const typeUsEnregistre = 'US_ENREGISTRE';

  final String type;
  final int emplacementActifId;
  final String emplacementActifCode;
  final String emplacementActifDesignation;
  final String emplacementStatut;
  final int qteScannee;
  final int qteAttendue;
  final int? emplacementPrecedentId;
  final String? emplacementPrecedentCode;
  final bool emplacementPrecedentTermine;
  final ScanResult? scanUs;
  final String? message;

  bool get isEmplacementEvent =>
      type == typeEmplacementOuvert ||
      type == typeEmplacementBascule ||
      type == typeEmplacementDejaActif;

  bool get isUsEnregistre => type == typeUsEnregistre;

  factory TraiterScanTerrainResult.fromJson(Map<String, dynamic> json) {
    final scanUsJson = json['scanUs'];
    return TraiterScanTerrainResult(
      type: json['type'] as String? ?? '',
      emplacementActifId: (json['emplacementActifId'] as num).toInt(),
      emplacementActifCode: json['emplacementActifCode'] as String? ?? '',
      emplacementActifDesignation: json['emplacementActifDesignation'] as String? ?? '',
      emplacementStatut: json['emplacementStatut'] as String? ?? '',
      qteScannee: (json['qteScannee'] as num?)?.toInt() ?? 0,
      qteAttendue: (json['qteAttendue'] as num?)?.toInt() ?? 0,
      emplacementPrecedentId: (json['emplacementPrecedentId'] as num?)?.toInt(),
      emplacementPrecedentCode: json['emplacementPrecedentCode'] as String?,
      emplacementPrecedentTermine: json['emplacementPrecedentTermine'] as bool? ?? false,
      scanUs: scanUsJson is Map<String, dynamic> ? ScanResult.fromJson(scanUsJson) : null,
      message: json['message'] as String?,
    );
  }
}
