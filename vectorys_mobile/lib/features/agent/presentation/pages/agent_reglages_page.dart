import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vectorys_mobile/core/utils/constants.dart';
import 'package:vectorys_mobile/features/agent/presentation/widgets/agent_shell.dart';
import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';

class AgentReglagesPage extends StatelessWidget {
  const AgentReglagesPage({
    super.key,
    required this.authRepository,
    required this.prefs,
    this.offlineSync,
  });

  final AuthRepository authRepository;
  final SharedPreferences prefs;
  final OfflineSyncService? offlineSync;

  @override
  Widget build(BuildContext context) {
    return AgentShell(
      currentIndex: 3,
      authRepository: authRepository,
      prefs: prefs,
      offlineSync: offlineSync,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(16, 14, 16, 24),
        children: [
          Text('Réglages', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w900)),
          const SizedBox(height: 4),
          Text(
            'Configuration de l\'application agent',
            style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.blueGrey[600]),
          ),
          const SizedBox(height: 16),
          _SettingTile(
            icon: Icons.cloud_outlined,
            label: 'URL API',
            value: AppConstants.baseUrl,
          ),
          _SettingTile(
            icon: offlineSync?.isOnline == false ? Icons.wifi_off : Icons.wifi,
            label: 'Mode réseau',
            value: offlineSync?.isOnline == false ? 'Hors ligne' : 'En ligne',
          ),
          _SettingTile(
            icon: Icons.info_outline,
            label: 'Version',
            value: 'v2.4.0 — Sprint 4',
          ),
          const SizedBox(height: 12),
          FilledButton.icon(
            onPressed: () => context.go('/sync'),
            icon: const Icon(Icons.cloud_sync_outlined),
            label: const Text('Centre de synchronisation'),
            style: FilledButton.styleFrom(
              backgroundColor: const Color(0xFF5B66F5),
              minimumSize: const Size.fromHeight(48),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
            ),
          ),
          const SizedBox(height: 8),
          OutlinedButton.icon(
            onPressed: () => context.go('/emplacements'),
            icon: const Icon(Icons.refresh),
            label: const Text('Rafraîchir les emplacements'),
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

class _SettingTile extends StatelessWidget {
  const _SettingTile({required this.icon, required this.label, required this.value});

  final IconData icon;
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFE6E9F3)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: const Color(0xFF5B66F5)),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: Theme.of(context).textTheme.labelSmall?.copyWith(color: Colors.blueGrey[500], fontWeight: FontWeight.w700)),
                const SizedBox(height: 2),
                Text(value, style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w700)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
