import 'package:dio/dio.dart';
import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/emplacements_repository.dart';

class EmplacementsRepositoryImpl implements EmplacementsRepository {
  EmplacementsRepositoryImpl(this._dio);

  final Dio _dio;

  @override
  Future<List<MesEmplacement>> getMesEmplacements() async {
    final response = await _dio.get<List<dynamic>>('/emplacements/mes-emplacements');
    final data = response.data ?? const [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(MesEmplacement.fromJson)
        .where((e) => e.code.isNotEmpty)
        .toList(growable: false);
  }
}

