import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vectorys_mobile/features/agent/presentation/widgets/agent_shell.dart';
import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';
import 'package:vectorys_mobile/features/offline_sync/presentation/widgets/offline_banner.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/emplacements_repository.dart';

class AgentDashboardPage extends StatefulWidget {
  const AgentDashboardPage({
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
  State<AgentDashboardPage> createState() => _AgentDashboardPageState();
}

class _AgentDashboardPageState extends State<AgentDashboardPage> {
  late Future<List<MesEmplacement>> _future;

  @override
  void initState() {
    super.initState();
    _future = widget.emplacementsRepository.getMesEmplacements();
  }

  void _refresh() => setState(() => _future = widget.emplacementsRepository.getMesEmplacements());

  @override
  Widget build(BuildContext context) {
    return AgentShell(
      currentIndex: 0,
      authRepository: widget.authRepository,
      prefs: widget.prefs,
      offlineSync: widget.offlineSync,
      onRefresh: _refresh,
      child: FutureBuilder<List<MesEmplacement>>(
        future: _future,
        builder: (context, snap) {
          final loading = snap.connectionState == ConnectionState.waiting;
          final list = snap.data ?? const <MesEmplacement>[];
          final groups = InventaireEmplacementGroup.fromList(list);

          final total = list.length;
          final done = list.where((e) => e.isTermine).length;
          final enCours = list.where((e) => e.isEnCours).length;
          final enAttente = total - done - enCours;
          final progress = total == 0 ? 0.0 : (done / total).clamp(0.0, 1.0);
          final unitsScanned = list.fold<int>(0, (s, e) => s + e.qteScannee);
          final unitsExpected = list.fold<int>(0, (s, e) => s + e.qteAttendue);

          return ListView(
            padding: const EdgeInsets.fromLTRB(16, 14, 16, 24),
            children: [
              if (widget.offlineSync != null) OfflineBanner(offlineSync: widget.offlineSync!),
              Text(
                'Tableau de bord',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w900),
              ),
              const SizedBox(height: 4),
              Text(
                'Vue synthétique de vos inventaires en cours',
                style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[600]),
              ),
              const SizedBox(height: 16),
              if (loading)
                const Center(child: Padding(padding: EdgeInsets.all(24), child: CircularProgressIndicator()))
              else if (snap.hasError)
                _ErrorCard(message: 'Impossible de charger les statistiques.')
              else ...[
                Row(
                  children: [
                    Expanded(child: _StatCard(label: 'Emplacements', value: '$total', icon: Icons.place_outlined)),
                    const SizedBox(width: 10),
                    Expanded(child: _StatCard(label: 'Terminés', value: '$done', icon: Icons.check_circle_outline, color: const Color(0xFF22C55E))),
                  ],
                ),
                const SizedBox(height: 10),
                Row(
                  children: [
                    Expanded(child: _StatCard(label: 'En cours', value: '$enCours', icon: Icons.play_circle_outline, color: const Color(0xFF5B66F5))),
                    const SizedBox(width: 10),
                    Expanded(child: _StatCard(label: 'En attente', value: '$enAttente', icon: Icons.schedule, color: Colors.orange)),
                  ],
                ),
                const SizedBox(height: 14),
                Container(
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: const Color(0xFFE6E9F3)),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Progression globale', style: Theme.of(context).textTheme.labelLarge?.copyWith(fontWeight: FontWeight.w900)),
                      const SizedBox(height: 10),
                      ClipRRect(
                        borderRadius: BorderRadius.circular(999),
                        child: LinearProgressIndicator(
                          value: progress,
                          minHeight: 10,
                          backgroundColor: const Color(0xFFF1F5FF),
                          color: const Color(0xFF5B66F5),
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        '${(progress * 100).round()}% — $done/$total emplacements',
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[600], fontWeight: FontWeight.w700),
                      ),
                      const SizedBox(height: 12),
                      Text(
                        '$unitsScanned / $unitsExpected unités scannées',
                        style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w800, color: const Color(0xFF4338CA)),
                      ),
                    ],
                  ),
                ),
                if (groups.isNotEmpty) ...[
                  const SizedBox(height: 16),
                  Text('Inventaires actifs', style: Theme.of(context).textTheme.labelLarge?.copyWith(fontWeight: FontWeight.w900)),
                  const SizedBox(height: 8),
                  ...groups.map((g) {
                    final gDone = g.emplacements.where((e) => e.isTermine).length;
                    final gTotal = g.emplacements.length;
                    final title = g.intitule.isNotEmpty ? g.intitule : (g.reference.isNotEmpty ? g.reference : 'Inventaire #${g.inventaireId}');
                    return Container(
                      margin: const EdgeInsets.only(bottom: 8),
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(14),
                        border: Border.all(color: const Color(0xFFE6E9F3)),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(title, style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w900)),
                          if (g.zone.isNotEmpty)
                            Text('Zone : ${g.zone}', style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[600])),
                          const SizedBox(height: 4),
                          Text('$gDone/$gTotal emplacements terminés', style: Theme.of(context).textTheme.labelMedium?.copyWith(color: const Color(0xFF5B66F5), fontWeight: FontWeight.w800)),
                          ...g.sections.map((section) => Padding(
                                padding: const EdgeInsets.only(top: 6),
                                child: Text(
                                  '${section.title} — ${section.items.where((e) => e.isTermine).length}/${section.items.length}',
                                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                        color: Colors.blueGrey[700],
                                        fontWeight: FontWeight.w700,
                                      ),
                                ),
                              )),
                        ],
                      ),
                    );
                  }),
                ],
                const SizedBox(height: 16),
                FilledButton.icon(
                  onPressed: () => context.go('/emplacements'),
                  icon: const Icon(Icons.map_outlined),
                  label: const Text('Voir mes emplacements'),
                  style: FilledButton.styleFrom(
                    backgroundColor: const Color(0xFF5B66F5),
                    minimumSize: const Size.fromHeight(48),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                  ),
                ),
              ],
            ],
          );
        },
      ),
    );
  }
}

class _StatCard extends StatelessWidget {
  const _StatCard({required this.label, required this.value, required this.icon, this.color});

  final String label;
  final String value;
  final IconData icon;
  final Color? color;

  @override
  Widget build(BuildContext context) {
    final c = color ?? const Color(0xFF5B66F5);
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFE6E9F3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: c, size: 22),
          const SizedBox(height: 8),
          Text(value, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w900, color: c)),
          Text(label, style: Theme.of(context).textTheme.labelSmall?.copyWith(color: Colors.blueGrey[600], fontWeight: FontWeight.w700)),
        ],
      ),
    );
  }
}

class _ErrorCard extends StatelessWidget {
  const _ErrorCard({required this.message});
  final String message;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.errorContainer,
        borderRadius: BorderRadius.circular(14),
      ),
      child: Text(message, style: TextStyle(color: Theme.of(context).colorScheme.onErrorContainer, fontWeight: FontWeight.w700)),
    );
  }
}
