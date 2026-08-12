import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../utils/constants.dart';

class AuthInterceptor extends Interceptor {
  AuthInterceptor({required this.prefs, required this.onUnauthorized});

  final SharedPreferences prefs;
  final void Function() onUnauthorized;

  @override
  Future<void> onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    final token = prefs.getString(AppConstants.prefToken);
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  Future<void> onError(DioException err, ErrorInterceptorHandler handler) async {
    if (err.response?.statusCode == 401) {
      await prefs.remove(AppConstants.prefToken);
      await prefs.remove(AppConstants.prefNom);
      onUnauthorized();
    }
    handler.next(err);
  }
}
