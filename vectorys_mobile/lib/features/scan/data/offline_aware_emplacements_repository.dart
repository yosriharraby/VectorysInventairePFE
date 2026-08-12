import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vectorys_mobile/core/utils/constants.dart';
import 'package:vectorys_mobile/features/offline_sync/data/local_database.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/emplacements_repository.dart';

/// Cache local toujours resynchronisé avec l'agent connecté.
/// Une liste vide API remplace aussi le cache (évite de réafficher
/// les emplacements d'un autre agent).
class OfflineAwareEmplacementsRepository implements EmplacementsRepository {
  OfflineAwareEmplacementsRepository({
    required EmplacementsRepository remote,
    required OfflineSyncService offlineSync,
    required LocalDatabase localDatabase,
    required SharedPreferences prefs,
  })  : _remote = remote,
        _offlineSync = offlineSync,
        _db = localDatabase,
        _prefs = prefs;

  final EmplacementsRepository _remote;
  final OfflineSyncService _offlineSync;
  final LocalDatabase _db;
  final SharedPreferences _prefs;

  int? get _currentUserId => _prefs.getInt(AppConstants.prefUserId);
  int? get _cacheOwnerUserId => _prefs.getInt(AppConstants.prefCacheOwnerUserId);

  @override
  Future<List<MesEmplacement>> getMesEmplacements() async {
    if (_offlineSync.isOnline) {
      try {
        final list = await _remote.getMesEmplacements();
        // Toujours remplacer — y compris liste vide — pour isoler chaque agent.
        await _db.replaceCachedEmplacements(list);
        final userId = _currentUserId;
        if (userId != null) {
          await _prefs.setInt(AppConstants.prefCacheOwnerUserId, userId);
        }
        return list;
      } on DioException catch (e) {
        if (!_offlineSync.isNetworkError(e)) rethrow;
      }
    }

    return _readCacheForCurrentUser();
  }

  Future<List<MesEmplacement>> _readCacheForCurrentUser() async {
    final userId = _currentUserId;
    final owner = _cacheOwnerUserId;
    if (userId == null || owner == null || owner != userId) {
      throw Exception(
        'Cache local indisponible pour cet agent. Connectez-vous au réseau.',
      );
    }

    final cached = await _db.getCachedEmplacements();
    // Liste vide légitime (agent sans assignation) — on la renvoie telle quelle.
    return cached;
  }
}
