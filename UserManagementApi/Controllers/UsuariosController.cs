using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.Models;

namespace UserManagementApi.Controllers
{
    [ApiController]
    [Route("api/v1/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // POST /usuarios/registro
        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Datos de entrada inválidos",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var errores = new List<string>();

            if (await _context.Usuarios.AnyAsync(u => u.CorreoElectronico == usuario.CorreoElectronico))
                errores.Add("El correo electrónico ya está registrado");

            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario))
                errores.Add("El nombre de usuario ya existe");

            if (errores.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Datos de entrada inválidos",
                    errors = errores
                });
            }

            usuario.FechaCreacion = DateTime.UtcNow;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Created("", new
            {
                success = true,
                message = "Usuario registrado exitosamente",
                data = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    celular = usuario.Celular,
                    correoElectronico = usuario.CorreoElectronico,
                    nombreUsuario = usuario.NombreUsuario,
                    fechaCreacion = usuario.FechaCreacion.ToString("o")
                }
            });
        }

        // GET /usuarios?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalItems = await _context.Usuarios.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    id = u.Id,
                    nombre = u.Nombre,
                    celular = u.Celular,
                    correoElectronico = u.CorreoElectronico,
                    nombreUsuario = u.NombreUsuario,
                    fechaCreacion = u.FechaCreacion.ToString("o")
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Lista de usuarios obtenida exitosamente",
                data = usuarios,
                pagination = new
                {
                    currentPage = page,
                    totalPages,
                    totalItems,
                    pageSize
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuarioPorId(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Usuario no encontrado"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Usuario obtenido exitosamente",
                data = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    celular = usuario.Celular,
                    correoElectronico = usuario.CorreoElectronico,
                    nombreUsuario = usuario.NombreUsuario,
                    fechaCreacion = usuario.FechaCreacion.ToString("o"),
                    fechaActualizacion = usuario.FechaActualizacion // Asumiendo que tienes este campo como DateTime o string
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuarioActualizado)
        {

            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Usuario no encontrado"
                });
            }

            // Validar que el correo electrónico no esté registrado por otro usuario
            bool correoDuplicado = await _context.Usuarios
                .AnyAsync(u => u.CorreoElectronico == usuarioActualizado.CorreoElectronico && u.Id != id);
            if (correoDuplicado)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Datos de entrada inválidos",
                    errors = new[] { "El correo electrónico ya está registrado por otro usuario" }
                });
            }

            // Actualizar campos (excepto NombreUsuario y FechaCreacion)
            usuarioExistente.Nombre = usuarioActualizado.Nombre;
            usuarioExistente.Celular = usuarioActualizado.Celular;
            usuarioExistente.CorreoElectronico = usuarioActualizado.CorreoElectronico;

            // Actualizar contraseña solo si viene en el request y no está vacía
            if (!string.IsNullOrWhiteSpace(usuarioActualizado.Contraseña))
            {
                usuarioExistente.Contraseña = usuarioActualizado.Contraseña;
            }

            usuarioExistente.FechaActualizacion = DateTime.UtcNow;

            _context.Usuarios.Update(usuarioExistente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Usuario actualizado exitosamente",
                data = new
                {
                    id = usuarioExistente.Id,
                    nombre = usuarioExistente.Nombre,
                    celular = usuarioExistente.Celular,
                    correoElectronico = usuarioExistente.CorreoElectronico,
                    nombreUsuario = usuarioExistente.NombreUsuario,
                    fechaCreacion = usuarioExistente.FechaCreacion.ToString("o"),
                    fechaActualizacion = usuarioExistente.FechaActualizacion.ToString("o")
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Usuario no encontrado"
                });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Usuario eliminado exitosamente"
            });
        }
    }
}
