import 'api_config.dart';

class AppConstants {
  AppConstants._();

  /// Android émulateur : 10.0.2.2 | Windows / iOS / desktop : localhost:5000
  /// Surcharge : flutter run --dart-define=API_BASE_URL=http://192.168.x.x:5000/api
  static String get baseUrl => resolveApiBaseUrl();

  static const String prefToken = 'vectorys_jwt';
  static const String prefNom = 'vectorys_nom';
  static const String prefCodePersonnel = 'vectorys_code_personnel';
  static const String prefUserId = 'vectorys_user_id';
  static const String prefCacheOwnerUserId = 'vectorys_cache_owner_user_id';
}
