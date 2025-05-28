using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.Models;

namespace UserManagementApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST /auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos" });

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario
                                       && u.Contraseña == request.Contraseña);

            if (usuario == null)
                return Unauthorized(new { success = false, message = "Credenciales inválidas" });

            return Ok(new
            {
                success = true,
                message = "Inicio de sesión exitoso",
                data = new
                {
                    usuario = new
                    {
                        id = usuario.Id,
                        nombre = usuario.Nombre,
                        nombreUsuario = usuario.NombreUsuario,
                        correoElectronico = usuario.CorreoElectronico
                    }
                }
            });
        }

        // POST /auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Aquí solo devuelves éxito, ya que aún no manejas JWT real
            return Ok(new
            {
                success = true,
                message = "Sesión cerrada exitosamente"
            });
        }
    }
}
