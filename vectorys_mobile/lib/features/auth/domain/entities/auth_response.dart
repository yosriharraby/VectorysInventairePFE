class AuthResponse {
  AuthResponse({
    required this.token,
    required this.role,
    required this.nom,
    required this.userId,
  });

  final String token;
  final String role;
  final String nom;
  final int userId;

  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    return AuthResponse(
      token: json['token'] as String,
      role: json['role'] as String,
      nom: json['nom'] as String,
      userId: (json['userId'] as num).toInt(),
    );
  }
}
