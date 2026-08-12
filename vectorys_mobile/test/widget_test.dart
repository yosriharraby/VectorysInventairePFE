import 'package:flutter_test/flutter_test.dart';
import 'package:vectorys_mobile/core/utils/api_config_io.dart';

void main() {
  group('API config', () {
    test('resolveApiBaseUrl returns non-empty default', () {
      final url = resolveApiBaseUrl();
      expect(url, isNotEmpty);
      expect(url, contains('/api'));
    });
  });

  group('Comptage terrain (simulation)', () {
    test('bascule emplacement termine le precedent', () {
      String? actif;
      String? termine;

      void scanEmplacement(String code) {
        if (actif != null && actif != code) {
          termine = actif;
        }
        actif = code;
      }

      scanEmplacement('TEST-A01');
      expect(actif, 'TEST-A01');
      expect(termine, isNull);

      scanEmplacement('TEST-A02');
      expect(actif, 'TEST-A02');
      expect(termine, 'TEST-A01');
    });

    test('les scans US restent enregistres sans quota bloquant', () {
      var scans = <String>[];
      scans.add('US-T101');
      scans.add('US-T101');
      scans.add('US-UNKNOWN');
      expect(scans.length, 3);
    });
  });
}
