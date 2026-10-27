import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vectorys_mobile/core/utils/constants.dart';
import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';

class AgentShell extends StatelessWidget {
  const AgentShell({
    super.key,
    required this.currentIndex,
    required this.authRepository,
    required this.prefs,
    required this.child,
    this.onRefresh,
    this.floatingActionButton,
    this.offlineSync,
  });

  final int currentIndex;
  final AuthRepository authRepository;
  final SharedPreferences prefs;
  final Widget child;
  final VoidCallback? onRefresh;
  final Widget? floatingActionButton;
  final OfflineSyncService? offlineSync;

  static const _routes = ['/dashboard', '/emplacements', '/profil', '/reglages'];

  @override
  Widget build(BuildContext context) {
    final nom = prefs.getString(AppConstants.prefNom) ?? 'Agent';

    Widget buildScaffold(bool isOnline, int pending) {
      final statusColor = isOnline ? const Color(0xFF22C55E) : const Color(0xFFEA580C);

      return Scaffold(
        backgroundColor: const Color(0xFFF6F7FB),
        appBar: AppBar(
          backgroundColor: Colors.white,
          surfaceTintColor: Colors.white,
          elevation: 0,
          leading: IconButton(
            tooltip: 'Menu',
            onPressed: () => Scaffold.of(context).openDrawer(),
            icon: const Icon(Icons.menu),
          ),
          titleSpacing: 0,
          title: SizedBox(
            height: 34,
            child: Image.asset('assets/brand/vectorys-logo.png', fit: BoxFit.contain),
          ),
          actions: [
            if (onRefresh != null)
              IconButton(
                tooltip: 'Rafraîchir',
                onPressed: onRefresh,
                icon: const Icon(Icons.refresh),
              ),
            if (pending > 0)
              IconButton(
                tooltip: 'Synchronisation',
                onPressed: () => context.go('/sync'),
                icon: Badge(
                  label: Text('$pending'),
                  child: const Icon(Icons.sync, size: 20),
                ),
              ),
            const SizedBox(width: 2),
            Container(
              width: 8,
              height: 8,
              decoration: BoxDecoration(color: statusColor, shape: BoxShape.circle),
            ),
            const SizedBox(width: 8),
            Icon(
              isOnline ? Icons.wifi : Icons.wifi_off,
              size: 18,
              color: isOnline ? Colors.blueGrey[600] : const Color(0xFFEA580C),
            ),
            const SizedBox(width: 10),
            CircleAvatar(
              radius: 16,
              backgroundColor: const Color(0xFFEAEAF2),
              child: Text(
                nom.isNotEmpty ? nom.characters.first.toUpperCase() : 'A',
                style: const TextStyle(fontWeight: FontWeight.w800, color: Color(0xFF334155)),
              ),
            ),
            const SizedBox(width: 10),
          ],
        ),
        drawer: Drawer(
          child: SafeArea(
            child: Column(
              children: [
                ListTile(
                  leading: const Icon(Icons.person),
                  title: Text(nom),
                  subtitle: Text(isOnline ? 'Agent — En ligne' : 'Agent — Hors ligne'),
                ),
                const Divider(),
                ListTile(
                  leading: const Icon(Icons.dashboard_outlined),
                  title: const Text('Dashboard'),
                  onTap: () {
                    Navigator.pop(context);
                    context.go('/dashboard');
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.map_outlined),
                  title: const Text('Zones'),
                  onTap: () {
                    Navigator.pop(context);
                    context.go('/emplacements');
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.cloud_sync_outlined),
                  title: const Text('Synchronisation'),
                  onTap: () {
                    Navigator.pop(context);
                    context.go('/sync');
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.logout),
                  title: const Text('Déconnexion'),
                  onTap: () async {
                    await authRepository.logout();
                    if (context.mounted) context.go('/login');
                  },
                ),
              ],
            ),
          ),
        ),
        floatingActionButton: floatingActionButton,
        bottomNavigationBar: BottomNavigationBar(
          currentIndex: currentIndex,
          type: BottomNavigationBarType.fixed,
          selectedItemColor: const Color(0xFF5B66F5),
          unselectedItemColor: Colors.blueGrey[400],
          onTap: (i) {
            if (i != currentIndex) context.go(_routes[i]);
          },
          items: const [
            BottomNavigationBarItem(icon: Icon(Icons.dashboard_outlined), label: 'Dashboard'),
            BottomNavigationBarItem(icon: Icon(Icons.map_outlined), label: 'Zones'),
            BottomNavigationBarItem(icon: Icon(Icons.person_outline), label: 'Profil'),
            BottomNavigationBarItem(icon: Icon(Icons.settings_outlined), label: 'Réglages'),
          ],
        ),
        body: SafeArea(child: child),
      );
    }

    if (offlineSync == null) {
      return buildScaffold(true, 0);
    }

    return AnimatedBuilder(
      animation: offlineSync!,
      builder: (context, _) {
        return FutureBuilder<int>(
          future: offlineSync!.totalPendingCount(),
          builder: (context, snap) {
            return buildScaffold(offlineSync!.isOnline, snap.data ?? 0);
          },
        );
      },
    );
  }
}
