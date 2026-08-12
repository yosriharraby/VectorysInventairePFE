import 'dart:async';

import 'package:connectivity_plus/connectivity_plus.dart';

class ConnectivityService {
  ConnectivityService() : _connectivity = Connectivity();

  final Connectivity _connectivity;
  final _controller = StreamController<bool>.broadcast();
  bool _isOnline = true;

  bool get isOnline => _isOnline;
  Stream<bool> get onConnectivityChanged => _controller.stream;

  Future<void> init() async {
    final result = await _connectivity.checkConnectivity();
    _isOnline = _hasNetwork(result);
    _controller.add(_isOnline);

    _connectivity.onConnectivityChanged.listen((results) {
      final online = _hasNetwork(results);
      if (online != _isOnline) {
        _isOnline = online;
        _controller.add(_isOnline);
      }
    });
  }

  bool _hasNetwork(List<ConnectivityResult> results) {
    return results.any((r) => r != ConnectivityResult.none);
  }

  void dispose() => _controller.close();
}
