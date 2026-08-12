import 'dart:io';

/// URL API selon la plateforme (surcharge possible via --dart-define=API_BASE_URL=...).
String resolveApiBaseUrl() {
  const fromEnv = String.fromEnvironment('API_BASE_URL');
  if (fromEnv.isNotEmpty) {
    return fromEnv;
  }
  if (Platform.isAndroid) {
    return 'http://10.0.2.2:5000/api';
  }
  return 'http://localhost:5000/api';
}
