part of 'login_bloc.dart';

abstract class LoginEvent extends Equatable {
  const LoginEvent();

  @override
  List<Object?> get props => [];
}

class LoginRequested extends LoginEvent {
  const LoginRequested(this.codePersonnel);
  final String codePersonnel;

  @override
  List<Object?> get props => [codePersonnel];
}
