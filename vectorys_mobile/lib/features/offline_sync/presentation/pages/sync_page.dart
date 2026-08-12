import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vectorys_mobile/features/agent/presentation/widgets/agent_shell.dart';
import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';

class SyncPage extends StatefulWidget {
  const SyncPage({
    super.key,
    required this.authRepository,
    required this.prefs,
    required this.offlineSync,
  });

  final AuthRepository authRepository;
  final SharedPreferences prefs;
  final OfflineSyncService offlineSync;

  @override
  State<SyncPage> createState() => _SyncPageState();
}

class _SyncPageState extends State<SyncPage> {
  int _pendingScans = 0;
  int _pendingFin = 0;
  double _progress = 0;

  @override
  void initState() {
    super.initState();
    widget.offlineSync.addListener(_onSyncChanged);
    _loadCounts();
  }

  @override
  void dispose() {
    widget.offlineSync.removeListener(_onSyncChanged);
    super.dispose();
  }

  void _onSyncChanged() {
    if (mounted) {
      setState(() {
        _progress = widget.offlineSync.isSyncing ? 0.45 : 1.0;
      });
      _loadCounts();
    }
  }

  Future<void> _loadCounts() async {
    final scans = await widget.offlineSync.pendingScansCount();
    final fin = await widget.offlineSync.pendingFinComptageCount();
    if (!mounted) return;
    setState(() {
      _pendingScans = scans;
      _pendingFin = fin;
    });
  }

  Future<void> _sync() async {
    setState(() => _progress = 0.2);
    await widget.offlineSync.syncPending();
    await _loadCounts();
    if (!mounted) return;
    setState(() => _progress = 1.0);
    final msg = widget.offlineSync.lastSyncMessage ?? 'Terminé';
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
  }

  @override
  Widget build(BuildContext context) {
    final total = _pendingScans + _pendingFin;
    final online = widget.offlineSync.isOnline;

    return AgentShell(
      currentIndex: 3,
      authRepository: widget.authRepository,
      prefs: widget.prefs,
      offlineSync: widget.offlineSync,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(16, 14, 16, 24),
        children: [
          Text(
            'Synchronisation',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w900),
          ),
          const SizedBox(height: 4),
          Text(
            'Transfert des données collectées hors ligne',
            style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[600]),
          ),
          const SizedBox(height: 16),
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: const Color(0xFFE6E9F3)),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Icon(
                      online ? Icons.wifi : Icons.wifi_off,
                      color: online ? const Color(0xFF22C55E) : const Color(0xFFEA580C),
                    ),
                    const SizedBox(width: 10),
                    Text(
                      online ? 'Connecté' : 'Hors ligne',
                      style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w900),
                    ),
                  ],
                ),
                const SizedBox(height: 14),
                _StatRow(label: 'Scans en attente', value: '$_pendingScans'),
                _StatRow(label: 'Fin de comptage en attente', value: '$_pendingFin'),
                const SizedBox(height: 12),
                ClipRRect(
                  borderRadius: BorderRadius.circular(999),
                  child: LinearProgressIndicator(
                    value: widget.offlineSync.isSyncing ? null : (total == 0 ? 1.0 : _progress),
                    minHeight: 10,
                    backgroundColor: const Color(0xFFF1F5FF),
                    color: const Color(0xFF5B66F5),
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  widget.offlineSync.isSyncing
                      ? 'Envoi vers le serveur…'
                      : total == 0
                          ? 'Tout est synchronisé'
                          : '$total élément${total > 1 ? 's' : ''} à envoyer',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Colors.blueGrey[600],
                        fontWeight: FontWeight.w700,
                      ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          FilledButton.icon(
            onPressed: !online || widget.offlineSync.isSyncing || total == 0 ? null : _sync,
            icon: const Icon(Icons.cloud_upload_outlined),
            label: Text(widget.offlineSync.isSyncing ? 'Synchronisation…' : 'Lancer la synchronisation'),
            style: FilledButton.styleFrom(
              backgroundColor: const Color(0xFF5B66F5),
              minimumSize: const Size.fromHeight(48),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
            ),
          ),
          const SizedBox(height: 10),
          OutlinedButton.icon(
            onPressed: () => context.go('/emplacements'),
            icon: const Icon(Icons.map_outlined),
            label: const Text('Retour aux emplacements'),
            style: OutlinedButton.styleFrom(
              minimumSize: const Size.fromHeight(48),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
            ),
          ),
        ],
      ),
    );
  }
}

class _StatRow extends StatelessWidget {
  const _StatRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: Theme.of(context).textTheme.bodyMedium?.copyWith(color: Colors.blueGrey[700])),
          Text(value, style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w900, color: const Color(0xFF5B66F5))),
        ],
      ),
    );
  }
}
