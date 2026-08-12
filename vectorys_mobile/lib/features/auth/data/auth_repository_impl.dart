import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'package:vectorys_mobile/core/utils/constants.dart';
import 'package:vectorys_mobile/features/auth/domain/entities/auth_response.dart';
import 'package:vectorys_mobile/features/auth/domain/failures/failure.dart';
import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';
import 'package:vectorys_mobile/features/offline_sync/data/local_database.dart';

class AuthRepositoryImpl implements AuthRepository {
  AuthRepositoryImpl(this._dio, this._prefs, {LocalDatabase? localDatabase})
      : _localDatabase = localDatabase;

  final Dio _dio;
  final SharedPreferences _prefs;
  final LocalDatabase? _localDatabase;

  @override
  Future<Either<Failure, AuthResponse>> login(String codePersonnel) async {
    try {
      final response = await _dio.post<Map<String, dynamic>>(
        '/auth/login',
        data: {'codePersonnel': codePersonnel.trim()},
        options: Options(receiveTimeout: const Duration(seconds: 12), sendTimeout: const Duration(seconds: 12)),
      );
      final data = response.data;
      if (data == null) {
        return Left(Failure('Réponse vide.'));
      }
      final auth = AuthResponse.fromJson(data);

      final previousUserId = _prefs.getInt(AppConstants.prefUserId);
      if (previousUserId != null && previousUserId != auth.userId) {
        await _clearAgentLocalData();
      }

      await _prefs.setString(AppConstants.prefToken, auth.token);
      await _prefs.setString(AppConstants.prefNom, auth.nom);
      await _prefs.setString(AppConstants.prefCodePersonnel, codePersonnel.trim());
      await _prefs.setInt(AppConstants.prefUserId, auth.userId);
      return Right(auth);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return Left(Failure('Identifiants invalides.'));
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.sendTimeout) {
        return Left(Failure('Délai dépassé. Vérifiez que l’API tourne sur le port 5000.'));
      }
      if (e.type == DioExceptionType.connectionError) {
        return Left(Failure('Impossible de joindre l’API. Lancez l’API puis réessayez.'));
      }
      final apiMessage = e.response?.data;
      if (apiMessage is Map && apiMessage['message'] is String) {
        return Left(Failure(apiMessage['message'] as String));
      }
      return Left(Failure(e.message ?? 'Erreur réseau.'));
    } catch (e) {
      return Left(Failure(e.toString()));
    }
  }

  @override
  Future<void> logout() async {
    await _clearAgentLocalData();
    await _prefs.remove(AppConstants.prefToken);
    await _prefs.remove(AppConstants.prefNom);
    await _prefs.remove(AppConstants.prefCodePersonnel);
    await _prefs.remove(AppConstants.prefUserId);
    await _prefs.remove(AppConstants.prefCacheOwnerUserId);
  }

  Future<void> _clearAgentLocalData() async {
    final db = _localDatabase;
    if (db != null) {
      await db.clearAll();
    }
    await _prefs.remove(AppConstants.prefCacheOwnerUserId);
  }
}
