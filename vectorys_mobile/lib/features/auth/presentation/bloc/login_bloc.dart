import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import 'package:vectorys_mobile/features/auth/domain/repositories/auth_repository.dart';

part 'login_event.dart';
part 'login_state.dart';

class LoginBloc extends Bloc<LoginEvent, LoginState> {
  LoginBloc(this._repository) : super(const LoginInitial()) {
    on<LoginRequested>(_onLoginRequested);
  }

  final AuthRepository _repository;

  Future<void> _onLoginRequested(LoginRequested event, Emitter<LoginState> emit) async {
    emit(const LoginLoading());
    final result = await _repository.login(event.codePersonnel);
    result.fold(
      (f) => emit(LoginFailure(f.message)),
      (_) => emit(const LoginSuccess()),
    );
  }
}
