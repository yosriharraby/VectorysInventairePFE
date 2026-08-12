import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vectorys_mobile/core/utils/constants.dart';
import 'package:vectorys_mobile/features/agent/presentation/widgets/agent_shell.dart';
import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';

class AgentProfilPage extends StatelessWidget {
  const AgentProfilPage({
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
    final nom = prefs.getString(AppConstants.prefNom) ?? 'Agent';
    final code = prefs.getString(AppConstants.prefCodePersonnel) ?? '—';

    return AgentShell(
      currentIndex: 2,
      authRepository: authRepository,
      prefs: prefs,
      offlineSync: offlineSync,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(16, 14, 16, 24),
        children: [
          Text('Mon profil', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w900)),
          const SizedBox(height: 16),
          Center(
            child: CircleAvatar(
              radius: 40,
              backgroundColor: const Color(0xFFEEF2FF),
              child: Text(
                nom.isNotEmpty ? nom.characters.first.toUpperCase() : 'A',
                style: const TextStyle(fontSize: 32, fontWeight: FontWeight.w900, color: Color(0xFF4338CA)),
              ),
            ),
          ),
          const SizedBox(height: 16),
          _InfoTile(icon: Icons.person, label: 'Nom', value: nom),
          _InfoTile(icon: Icons.badge_outlined, label: 'Code personnel', value: code),
          _InfoTile(icon: Icons.work_outline, label: 'Rôle', value: 'Agent terrain'),
          const SizedBox(height: 24),
          OutlinedButton.icon(
            onPressed: () async {
              final confirm = await showDialog<bool>(
                context: context,
                builder: (ctx) => AlertDialog(
                  title: const Text('Déconnexion'),
                  content: const Text('Voulez-vous vous déconnecter ?'),
                  actions: [
                    TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Annuler')),
                    FilledButton(onPressed: () => Navigator.pop(ctx, true), child: const Text('Déconnexion')),
                  ],
                ),
              );
              if (confirm == true) {
                await authRepository.logout();
                if (context.mounted) context.go('/login');
              }
            },
            icon: const Icon(Icons.logout),
            label: const Text('Se déconnecter'),
            style: OutlinedButton.styleFrom(
              foregroundColor: Colors.red[700],
              minimumSize: const Size.fromHeight(48),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
            ),
          ),
        ],
      ),
    );
  }
}

class _InfoTile extends StatelessWidget {
  const _InfoTile({required this.icon, required this.label, required this.value});

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
        children: [
          Icon(icon, color: const Color(0xFF5B66F5)),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: Theme.of(context).textTheme.labelSmall?.copyWith(color: Colors.blueGrey[500], fontWeight: FontWeight.w700)),
                Text(value, style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w800)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
