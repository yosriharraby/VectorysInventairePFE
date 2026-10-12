class StockLigneProgress {
  const StockLigneProgress({
    required this.codeUs,
    required this.designationUs,
    required this.qteTheorique,
    required this.qteScannee,
    this.client,
  });

  final String codeUs;
  final String designationUs;
  final int qteTheorique;
  final int qteScannee;
  final String? client;

  bool get isComplete => qteTheorique > 0 && qteScannee >= qteTheorique;

  factory StockLigneProgress.fromJson(Map<String, dynamic> json) => StockLigneProgress(
        codeUs: json['codeUs'] as String? ?? '',
        designationUs: json['designationUs'] as String? ?? '',
        qteTheorique: (json['qteTheorique'] as num?)?.toInt() ?? 0,
        qteScannee: (json['qteScannee'] as num?)?.toInt() ?? 0,
        client: json['client'] as String?,
      );
}

class MesEmplacement {
  const MesEmplacement({
    required this.id,
    required this.code,
    required this.designation,
    required this.statut,
    required this.codeBarres,
    required this.qteAttendue,
    required this.qteScannee,
    required this.inventaireId,
    required this.inventaireReference,
    required this.inventaireIntitule,
    required this.inventaireZone,
    this.lignesStock = const [],
    this.passageType = 'C1',
    this.recomptageId,
    this.recomptageLibelle,
    this.recomptageStatut,
  });

  final int id;
  final String code;
  final String designation;
  final String statut;
  final String codeBarres;
  final int qteAttendue;
  final int qteScannee;
  final int inventaireId;
  final String inventaireReference;
  final String inventaireIntitule;
  final String inventaireZone;
  final List<StockLigneProgress> lignesStock;
  final String passageType;
  final int? recomptageId;
  final String? recomptageLibelle;
  final String? recomptageStatut;

  factory MesEmplacement.fromJson(Map<String, dynamic> json) {
    final rawLignes = json['lignesStock'] as List<dynamic>? ?? const [];
    return MesEmplacement(
      id: (json['id'] as num).toInt(),
      code: json['code'] as String? ?? '',
      designation: json['designation'] as String? ?? '',
      statut: json['statut'] as String? ?? 'EN_ATTENTE',
      codeBarres: json['codeBarres'] as String? ?? '',
      qteAttendue: (json['qteAttendue'] as num?)?.toInt() ?? 0,
      qteScannee: (json['qteScannee'] as num?)?.toInt() ?? 0,
      inventaireId: (json['inventaireId'] as num?)?.toInt() ?? 0,
      inventaireReference: json['inventaireReference'] as String? ?? '',
      inventaireIntitule: json['inventaireIntitule'] as String? ?? '',
      inventaireZone: json['inventaireZone'] as String? ?? '',
      lignesStock: rawLignes
          .whereType<Map<String, dynamic>>()
          .map(StockLigneProgress.fromJson)
          .toList(growable: false),
      passageType: json['passageType'] as String? ?? 'C1',
      recomptageId: (json['recomptageId'] as num?)?.toInt(),
      recomptageLibelle: json['recomptageLibelle'] as String?,
      recomptageStatut: json['recomptageStatut'] as String?,
    );
  }

  bool get isTermine => statut.toUpperCase() == 'TERMINE';
  bool get isEnCours => statut.toUpperCase() == 'EN_COURS';

  String get displayTitle => code.isNotEmpty ? code : designation;

  String? get displaySubtitle {
    final d = designation.trim();
    final c = code.trim();
    if (d.isEmpty || d.toUpperCase() == c.toUpperCase()) return null;
    return d;
  }
}

class InventaireEmplacementGroup {
  const InventaireEmplacementGroup({
    required this.inventaireId,
    required this.reference,
    required this.intitule,
    required this.zone,
    required this.emplacements,
  });

  final int inventaireId;
  final String reference;
  final String intitule;
  final String zone;
  final List<MesEmplacement> emplacements;

  List<({String title, List<MesEmplacement> items})> get sections {
    final initial = emplacements.where((e) => e.recomptageId == null).toList();
    final byRec = <int, List<MesEmplacement>>{};
    for (final e in emplacements.where((e) => e.recomptageId != null)) {
      byRec.putIfAbsent(e.recomptageId!, () => []).add(e);
    }

    final result = <({String title, List<MesEmplacement> items})>[];
    if (initial.isNotEmpty) {
      result.add((title: 'Inventaire initial', items: initial));
    }
    final recEntries = byRec.entries.toList()
      ..sort((a, b) => a.key.compareTo(b.key));
    for (final entry in recEntries) {
      final label = entry.value.first.recomptageLibelle?.trim();
      result.add((
        title: (label != null && label.isNotEmpty) ? label : 'Recomptage #${entry.key}',
        items: entry.value,
      ));
    }
    return result;
  }

  static List<InventaireEmplacementGroup> fromList(List<MesEmplacement> list) {
    final byId = <int, List<MesEmplacement>>{};
    for (final e in list) {
      byId.putIfAbsent(e.inventaireId, () => []).add(e);
    }
    return byId.entries.map((entry) {
      final first = entry.value.first;
      return InventaireEmplacementGroup(
        inventaireId: entry.key,
        reference: first.inventaireReference,
        intitule: first.inventaireIntitule,
        zone: first.inventaireZone,
        emplacements: entry.value,
      );
    }).toList(growable: false);
  }
}
