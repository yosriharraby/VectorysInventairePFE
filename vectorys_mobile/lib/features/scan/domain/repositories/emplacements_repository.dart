import 'package:vectorys_mobile/features/scan/domain/entities/mes_emplacement.dart';

abstract class EmplacementsRepository {
  Future<List<MesEmplacement>> getMesEmplacements();
}

