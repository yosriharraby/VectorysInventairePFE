import 'package:dartz/dartz.dart';

import '../entities/auth_response.dart';
import '../failures/failure.dart';

abstract class AuthRepository {
  Future<Either<Failure, AuthResponse>> login(String codePersonnel);
  Future<void> logout();
}
