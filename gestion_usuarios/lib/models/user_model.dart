class User {
  final int id;
  final String name;
  final String phone;
  final String email;
  final String username;
  final String? password;

  User({
    required this.id,
    required this.name,
    required this.phone,
    required this.email,
    required this.username,
    this.password,
  });

  // Mapear campos del backend a tus nombres actuales
  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'],
      name: json['nombre'], // "nombre" del backend → "name"
      phone: json['celular'], // "celular" → "phone"
      email: json['correoElectronico'], // etc.
      username: json['nombreUsuario'],
      password: json['contraseña'], // Solo si lo necesitas
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'nombre': name,
      'celular': phone,
      'correoElectronico': email,
      'nombreUsuario': username,
      'contraseña': password, // Solo si lo estás enviando
    };
  }

  User copyWith({
    String? name,
    String? phone,
    String? email,
    String? password,
  }) {
    return User(
      id: id,
      name: name ?? this.name,
      phone: phone ?? this.phone,
      email: email ?? this.email,
      username: username, // no se puede editar
      password: password ?? this.password,
    );
  }

}

