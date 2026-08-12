import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:pretty_dio_logger/pretty_dio_logger.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../utils/constants.dart';
import 'interceptors/auth_interceptor.dart';

class DioClient {
  DioClient._();

  static Dio create({
    required SharedPreferences prefs,
    required void Function() onUnauthorized,
  }) {
    final dio = Dio(
      BaseOptions(
        baseUrl: AppConstants.baseUrl,
        connectTimeout: const Duration(seconds: 30),
        receiveTimeout: const Duration(seconds: 30),
        headers: {'Accept': 'application/json', 'Content-Type': 'application/json'},
      ),
    );

    dio.interceptors.add(AuthInterceptor(prefs: prefs, onUnauthorized: onUnauthorized));

    if (kDebugMode) {
      dio.interceptors.add(
        PrettyDioLogger(requestHeader: true, requestBody: true, responseHeader: false),
      );
    }

    return dio;
  }
}
