import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vectorys_mobile/features/agent/presentation/widgets/agent_shell.dart';
import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';
import 'package:vectorys_mobile/features/offline_sync/presentation/widgets/offline_banner.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/emplacements_repository.dart';

enum EmplacementStatutFilter { tous, enAttente, enCours, termine }

class EmplacementsPage extends StatefulWidget {
  const EmplacementsPage({
    super.key,
    required this.authRepository,
    required this.prefs,
    required this.emplacementsRepository,
    this.offlineSync,
  });

  final AuthRepository authRepository;
  final SharedPreferences prefs;
  final EmplacementsRepository emplacementsRepository;
  final OfflineSyncService? offlineSync;

  @override
  State<EmplacementsPage> createState() => _EmplacementsPageState();
}

class _EmplacementsPageState extends State<EmplacementsPage> {
  late Future<List<MesEmplacement>> _future;
  EmplacementStatutFilter _filter = EmplacementStatutFilter.tous;

  @override
  void initState() {
    super.initState();
    _future = widget.emplacementsRepository.getMesEmplacements();
  }

  void _refresh() {
    setState(() => _future = widget.emplacementsRepository.getMesEmplacements());
  }

  List<MesEmplacement> _applyFilter(List<MesEmplacement> list) {
    return switch (_filter) {
      EmplacementStatutFilter.tous => list,
      EmplacementStatutFilter.enAttente =>
        list.where((e) => e.statut.toUpperCase() == 'EN_ATTENTE').toList(),
      EmplacementStatutFilter.enCours =>
        list.where((e) => e.statut.toUpperCase() == 'EN_COURS').toList(),
      EmplacementStatutFilter.termine =>
        list.where((e) => e.statut.toUpperCase() == 'TERMINE').toList(),
    };
  }

  Future<void> _openFilterSheet() async {
    final selected = await showModalBottomSheet<EmplacementStatutFilter>(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (ctx) => SafeArea(
        child: Padding(
          padding: const EdgeInsets.fromLTRB(16, 12, 16, 16),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Filtrer par statut',
                style: Theme.of(ctx).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w900),
              ),
              const SizedBox(height: 12),
              ...EmplacementStatutFilter.values.map(
                (f) => ListTile(
                  contentPadding: EdgeInsets.zero,
                  leading: Icon(
                    _filter == f ? Icons.radio_button_checked : Icons.radio_button_off,
                    color: const Color(0xFF5B66F5),
                  ),
                  title: Text(_filterLabel(f)),
                  onTap: () => Navigator.pop(ctx, f),
                ),
              ),
            ],
          ),
        ),
      ),
    );
    if (selected != null) setState(() => _filter = selected);
  }

  static String _filterLabel(EmplacementStatutFilter f) => switch (f) {
        EmplacementStatutFilter.tous => 'Tous',
        EmplacementStatutFilter.enAttente => 'En attente',
        EmplacementStatutFilter.enCours => 'En cours',
        EmplacementStatutFilter.termine => 'Terminé',
      };

  @override
  Widget build(BuildContext context) {
    final cs = Theme.of(context).colorScheme;
    final filterActive = _filter != EmplacementStatutFilter.tous;

    return AgentShell(
      currentIndex: 1,
      authRepository: widget.authRepository,
      prefs: widget.prefs,
      offlineSync: widget.offlineSync,
      onRefresh: _refresh,
      child: FutureBuilder<List<MesEmplacement>>(
        future: _future,
        builder: (context, snap) {
          final loading = snap.connectionState == ConnectionState.waiting;
          final error = snap.hasError ? snap.error : null;
          final all = snap.data ?? const <MesEmplacement>[];
          final emplacements = _applyFilter(all);
          final groups = InventaireEmplacementGroup.fromList(emplacements);
          final primaryGroup = groups.length == 1 ? groups.first : null;

          final total = emplacements.length;
          final done = emplacements.where((e) => e.statut.toUpperCase() == 'TERMINE').length;
          final progress = total == 0 ? 0.0 : (done / total).clamp(0.0, 1.0);

            return ListView(
              padding: const EdgeInsets.fromLTRB(16, 14, 16, 24),
              children: [
                if (widget.offlineSync != null) OfflineBanner(offlineSync: widget.offlineSync!),
                Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        primaryGroup?.intitule.isNotEmpty == true
                            ? primaryGroup!.intitule
                            : 'Inventaire Actuel',
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w900),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        primaryGroup?.zone.isNotEmpty == true
                            ? 'Zone : ${primaryGroup!.zone}'
                            : groups.length > 1
                                ? '${groups.length} inventaires actifs'
                                : 'Progression des emplacements',
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[600]),
                      ),
                    ],
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      Text(
                        '$done/$total',
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                              color: const Color(0xFF5B66F5),
                              fontWeight: FontWeight.w900,
                            ),
                      ),
                      Text(
                        'ZONES TRAITÉES',
                        style: Theme.of(context).textTheme.labelSmall?.copyWith(
                              color: Colors.blueGrey[500],
                              letterSpacing: 0.6,
                              fontWeight: FontWeight.w900,
                            ),
                      ),
                    ],
                  ),
                ],
              ),
              const SizedBox(height: 10),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: const Color(0xFFE6E9F3)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Performance en temps réel',
                      style: Theme.of(context).textTheme.labelMedium?.copyWith(
                            color: const Color(0xFF5B66F5),
                            fontWeight: FontWeight.w900,
                          ),
                    ),
                    const SizedBox(height: 10),
                    ClipRRect(
                      borderRadius: BorderRadius.circular(999),
                      child: LinearProgressIndicator(
                        value: loading ? null : progress,
                        minHeight: 10,
                        backgroundColor: const Color(0xFFF1F5FF),
                        color: const Color(0xFF5B66F5),
                      ),
                    ),
                    const SizedBox(height: 8),
                    Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        loading ? '—' : '${(progress * 100).round()}%',
                        style: Theme.of(context).textTheme.labelSmall?.copyWith(
                              color: Colors.blueGrey[600],
                              fontWeight: FontWeight.w900,
                            ),
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 14),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Row(
                    children: [
                      Icon(Icons.list_alt, size: 18, color: Colors.blueGrey[700]),
                      const SizedBox(width: 8),
                      Text(
                        'LISTE DES EMPLACEMENTS',
                        style: Theme.of(context).textTheme.labelMedium?.copyWith(
                              letterSpacing: 0.8,
                              fontWeight: FontWeight.w900,
                              color: Colors.blueGrey[700],
                            ),
                      ),
                    ],
                  ),
                  TextButton.icon(
                    onPressed: _openFilterSheet,
                    icon: Badge(
                      isLabelVisible: filterActive,
                      smallSize: 8,
                      child: const Icon(Icons.tune, size: 18),
                    ),
                    label: Text(filterActive ? _filterLabel(_filter) : 'Filtrer'),
                    style: TextButton.styleFrom(foregroundColor: const Color(0xFF5B66F5)),
                  ),
                ],
              ),
              if (filterActive)
                Padding(
                  padding: const EdgeInsets.only(bottom: 8),
                  child: Text(
                    '$total résultat${total > 1 ? 's' : ''} sur ${all.length}',
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[500], fontWeight: FontWeight.w600),
                  ),
                ),
              if (error != null)
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: cs.errorContainer,
                    borderRadius: BorderRadius.circular(14),
                  ),
                  child: Text(
                    'Erreur de chargement. Vérifiez la connexion et réessayez.',
                    style: TextStyle(color: cs.onErrorContainer, fontWeight: FontWeight.w700),
                  ),
                )
              else if (!loading && all.isEmpty)
                Container(
                  padding: const EdgeInsets.all(18),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: const Color(0xFFE6E9F3)),
                  ),
                  child: Text(
                    "Aucun emplacement assigné pour l'instant.",
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                          color: Colors.blueGrey[700],
                          fontWeight: FontWeight.w700,
                        ),
                  ),
                )
              else if (!loading && emplacements.isEmpty)
                Container(
                  padding: const EdgeInsets.all(18),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: const Color(0xFFE6E9F3)),
                  ),
                  child: Text(
                    'Aucun emplacement pour ce filtre.',
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                          color: Colors.blueGrey[700],
                          fontWeight: FontWeight.w700,
                        ),
                  ),
                )
              else
                ...groups.expand((group) sync* {
                  if (groups.length > 1) {
                    yield Padding(
                      padding: const EdgeInsets.only(bottom: 8, top: 4),
                      child: _InventaireSectionHeader(group: group),
                    );
                  }
                  for (final e in group.emplacements) {
                    yield _EmplacementCard(
                      e,
                      onTap: () => context.go('/scan/loop'),
                    );
                  }
                }),
            ],
          );
        },
      ),
    );
  }
}

class _InventaireSectionHeader extends StatelessWidget {
  const _InventaireSectionHeader({required this.group});

  final InventaireEmplacementGroup group;

  @override
  Widget build(BuildContext context) {
    final title = group.intitule.isNotEmpty ? group.intitule : group.reference;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: BoxDecoration(
        color: const Color(0xFFEEF2FF),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: const Color(0xFFDDE3FF)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title.isNotEmpty ? title : 'Inventaire #${group.inventaireId}',
            style: Theme.of(context).textTheme.labelLarge?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: const Color(0xFF4338CA),
                ),
          ),
          if (group.zone.isNotEmpty) ...[
            const SizedBox(height: 4),
            Text(
              'Zone : ${group.zone}',
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: const Color(0xFF6366F1),
                    fontWeight: FontWeight.w700,
                  ),
            ),
          ],
        ],
      ),
    );
  }
}

class _EmplacementCard extends StatelessWidget {
  const _EmplacementCard(this.e, {required this.onTap});

  final MesEmplacement e;
  final VoidCallback onTap;

  bool get isDone => e.statut.toUpperCase() == 'TERMINE';
  bool get isEnCours => e.statut.toUpperCase() == 'EN_COURS';

  @override
  Widget build(BuildContext context) {
    final statusText = isDone
        ? 'Terminé'
        : isEnCours
            ? 'En cours'
            : 'En attente';
    final statusBg = isDone
        ? const Color(0xFFDCFCE7)
        : isEnCours
            ? const Color(0xFFEEF2FF)
            : const Color(0xFFF1F5F9);
    final statusFg = isDone
        ? const Color(0xFF166534)
        : isEnCours
            ? const Color(0xFF4338CA)
            : const Color(0xFF475569);

    return Material(
      color: Colors.transparent,
      child: InkWell(
        borderRadius: BorderRadius.circular(16),
        onTap: isDone ? null : onTap,
        child: Container(
          margin: const EdgeInsets.only(bottom: 10),
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: const Color(0xFFE6E9F3)),
          ),
          child: Row(
            children: [
              Container(
                width: 38,
                height: 38,
                decoration: BoxDecoration(
                  color: isDone ? const Color(0xFFECFDF5) : const Color(0xFFF1F5FF),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(
                  isDone ? Icons.check_circle : Icons.place_outlined,
                  color: isDone ? const Color(0xFF22C55E) : const Color(0xFF5B66F5),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            e.displayTitle,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w900),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                          decoration: BoxDecoration(color: statusBg, borderRadius: BorderRadius.circular(999)),
                          child: Text(
                            statusText,
                            style: Theme.of(context)
                                .textTheme
                                .labelSmall
                                ?.copyWith(color: statusFg, fontWeight: FontWeight.w900),
                          ),
                        ),
                      ],
                    ),
                    if (e.displaySubtitle != null) ...[
                      const SizedBox(height: 4),
                      Text(
                        e.displaySubtitle!,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                              color: Colors.blueGrey[700],
                              fontWeight: FontWeight.w600,
                            ),
                      ),
                    ],
                    const SizedBox(height: 6),
                    Text(
                      isDone
                          ? 'Inventaire terminé'
                          : e.qteScannee > 0
                              ? '${e.qteScannee} unité${e.qteScannee > 1 ? 's' : ''} scannée${e.qteScannee > 1 ? 's' : ''}'
                              : 'Scannez le code emplacement puis les US',
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[600]),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
              Icon(Icons.chevron_right, color: Colors.blueGrey[300]),
            ],
          ),
        ),
      ),
    );
  }
}
