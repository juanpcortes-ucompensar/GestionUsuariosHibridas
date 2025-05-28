import 'dart:convert';
import 'package:http/http.dart'
    as http; //>>reemplazar http://<TU_IP>:<PUERTO> por la dirección de tu backend en .NET (por ejemplo: http://192.168.1.10:5000).
import '../models/user_model.dart';
import '../models/ApiException.dart';

class ApiService {
  static const String baseUrl = 'http://localhost:5143/api/v1';

  // Dentro de ApiService
  Future<List<User>> getUsersPaginated({int page = 1, int pageSize = 10}) async {
    final url = Uri.parse('$baseUrl/usuarios?page=$page&pageSize=$pageSize');

    final response = await http.get(url);

    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);
      final List<dynamic> userData = decoded['data'];
      return userData.map((json) => User.fromJson(json)).toList();
    } else {
      throw Exception('Error al obtener usuarios paginados');
    }
  }


  // Login: POST /auth/login
  static Future<bool> login(String nombreUsuario, String contrasena) async {
    final url = Uri.parse('$baseUrl/auth/login');

    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'nombreUsuario': nombreUsuario,
        'contraseña': contrasena,
      }),
    );

    if (response.statusCode == 200) {
      // Login exitoso
      return true;
    } else if (response.statusCode == 401) {
      // Credenciales inválidas
      return false;
    } else {
      // Otro error, puedes lanzar excepción o manejarlo
      throw Exception('Error en la conexión: ${response.statusCode}');
    }
  }

  Future<bool> logout() async {
    final url = Uri.parse('$baseUrl/auth/logout');

    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      return true;
    } else {
      return false;
    }
  }

  Future<void> registerUser(User user) async {
    final url = Uri.parse('$baseUrl/usuarios/registro');

    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(user.toJson()),
    );

    if (response.statusCode == 201) {
      // Registro exitoso, no hace falta retornar nada
      return;
    } else {
      // Parsear mensaje de error detallado del backend
      final Map<String, dynamic> body = jsonDecode(response.body);
      String errorMessage = body['message'] ?? 'Error desconocido';

      if (body.containsKey('errors')) {
        if (body['errors'] is List) {
          // Unir todos los errores en una cadena
          errorMessage += '\n' + (body['errors'] as List).join('\n');
        }
      }

      throw ApiException(errorMessage);
    }
  }



  Future<void> updateUser(User user) async {
    final url = Uri.parse('$baseUrl/usuarios/${user.id}');

    final response = await http.put(
      url,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        "id": user.id,
        "nombre": user.name,
        "celular": user.phone,
        "correoElectronico": user.email,
        "contraseña": user.password!.isNotEmpty ? user.password : null,
      }),
    );

    if (response.statusCode == 200) {
      return;
    } else {
      final Map<String, dynamic> body = jsonDecode(response.body);
      String errorMessage = body['message'] ?? 'Error desconocido al actualizar';

      if (body.containsKey('errors')) {
        errorMessage += '\n' + (body['errors'] as List).join('\n');
      }

      throw Exception(errorMessage);
    }
  }



  Future<bool> deleteUser(int id) async {
    final url = Uri.parse('$baseUrl/usuarios/$id');

    final response = await http.delete(url);

    if (response.statusCode == 200) {
      // Opcional: podrías parsear y chequear success en el JSON de respuesta
      final Map<String, dynamic> body = jsonDecode(response.body);
      return body['success'] == true;
    } else {
      print('Error al eliminar usuario: ${response.body}');
      return false;
    }
  }

}
