import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'package:vectorys_mobile/core/network/dio_client.dart';
import 'package:vectorys_mobile/core/utils/constants.dart';
import 'package:vectorys_mobile/features/auth/data/auth_repository_impl.dart';
import 'package:vectorys_mobile/features/auth/presentation/bloc/login_bloc.dart';
import 'package:vectorys_mobile/features/auth/presentation/pages/login_page.dart';
import 'package:vectorys_mobile/features/offline_sync/data/connectivity_service.dart';
import 'package:vectorys_mobile/features/offline_sync/data/local_database.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';
import 'package:vectorys_mobile/features/offline_sync/presentation/pages/sync_page.dart';
import 'package:vectorys_mobile/features/scan/data/emplacements_repository_impl.dart';
import 'package:vectorys_mobile/features/scan/data/offline_aware_emplacements_repository.dart';
import 'package:vectorys_mobile/features/scan/data/offline_aware_scan_repository.dart';
import 'package:vectorys_mobile/features/scan/data/scan_repository_impl.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/emplacements_repository.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/scan_repository.dart';
import 'package:vectorys_mobile/features/agent/presentation/pages/agent_dashboard_page.dart';
import 'package:vectorys_mobile/features/agent/presentation/pages/agent_profil_page.dart';
import 'package:vectorys_mobile/features/agent/presentation/pages/agent_reglages_page.dart';
import 'package:vectorys_mobile/features/scan/presentation/pages/barcode_scanner_page.dart';
import 'package:vectorys_mobile/features/scan/presentation/pages/emplacements_page.dart';
import 'package:vectorys_mobile/features/scan/presentation/pages/scan_loop_page.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final prefs = await SharedPreferences.getInstance();

  late final GoRouter appRouter;

  final localDatabase = LocalDatabase();
  await localDatabase.init();

  final dio = DioClient.create(
    prefs: prefs,
    onUnauthorized: () {
      // Fire-and-forget clear then redirect (évite fuite de cache entre agents).
      () async {
        await localDatabase.clearAll();
        await prefs.remove(AppConstants.prefToken);
        await prefs.remove(AppConstants.prefNom);
        await prefs.remove(AppConstants.prefCodePersonnel);
        await prefs.remove(AppConstants.prefUserId);
        await prefs.remove(AppConstants.prefCacheOwnerUserId);
      }();
      appRouter.go('/login');
    },
  );

  final connectivityService = ConnectivityService();
  final offlineSyncService = OfflineSyncService(
    dio: dio,
    localDatabase: localDatabase,
    connectivityService: connectivityService,
  );
  await offlineSyncService.init();

  final authRepository = AuthRepositoryImpl(dio, prefs, localDatabase: localDatabase);
  final EmplacementsRepository emplacementsRepository = OfflineAwareEmplacementsRepository(
    remote: EmplacementsRepositoryImpl(dio),
    offlineSync: offlineSyncService,
    localDatabase: localDatabase,
    prefs: prefs,
  );
  final ScanRepository scanRepository = OfflineAwareScanRepository(
    remote: ScanRepositoryImpl(dio),
    offlineSync: offlineSyncService,
    localDatabase: localDatabase,
  );

  appRouter = GoRouter(
    initialLocation: '/login',
    routes: [
      GoRoute(
        path: '/login',
        builder: (context, state) => BlocProvider(
          create: (_) => LoginBloc(authRepository),
          child: const LoginPage(),
        ),
      ),
      GoRoute(
        path: '/dashboard',
        builder: (context, state) => AgentDashboardPage(
          authRepository: authRepository,
          prefs: prefs,
          emplacementsRepository: emplacementsRepository,
          offlineSync: offlineSyncService,
        ),
      ),
      GoRoute(
        path: '/emplacements',
        builder: (context, state) => EmplacementsPage(
          authRepository: authRepository,
          prefs: prefs,
          emplacementsRepository: emplacementsRepository,
          offlineSync: offlineSyncService,
        ),
      ),
      GoRoute(
        path: '/profil',
        builder: (context, state) => AgentProfilPage(
          authRepository: authRepository,
          prefs: prefs,
          offlineSync: offlineSyncService,
        ),
      ),
      GoRoute(
        path: '/reglages',
        builder: (context, state) => AgentReglagesPage(
          authRepository: authRepository,
          prefs: prefs,
          offlineSync: offlineSyncService,
        ),
      ),
      GoRoute(
        path: '/sync',
        builder: (context, state) => SyncPage(
          authRepository: authRepository,
          prefs: prefs,
          offlineSync: offlineSyncService,
        ),
      ),
      GoRoute(
        path: '/scan/barcode',
        builder: (context, state) => const BarcodeScannerPage(),
      ),
      GoRoute(
        path: '/scan/loop',
        builder: (context, state) => ScanLoopPage(
          scanRepository: scanRepository,
          offlineSync: offlineSyncService,
        ),
      ),
    ],
  );

  runApp(VectorysApp(router: appRouter));
}

class VectorysApp extends StatelessWidget {
  const VectorysApp({super.key, required this.router});

  final GoRouter router;

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Vectorys Inventaire',
      routerConfig: router,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xFF2563EB)),
        useMaterial3: true,
      ),
    );
  }
}
