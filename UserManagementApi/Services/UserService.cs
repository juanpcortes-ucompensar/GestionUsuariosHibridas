using UserManagementApi.Models;

namespace UserManagementApi.Services
{
    public class UserService
    {
        private static List<Usuario> users = new List<Usuario>();
        private static int nextId = 1;

        public List<Usuario> GetAll() => users;

        public Usuario? GetById(int id) => users.FirstOrDefault(u => u.Id == id);

        public Usuario Add(Usuario user)
        {
            user.Id = nextId++;
            users.Add(user);
            return user;
        }

        public bool Delete(int id)
        {
            var user = GetById(id);
            if (user == null) return false;
            users.Remove(user);
            return true;
        }

        public bool Update(Usuario updatedUser)
        {
            var user = GetById(updatedUser.Id);
            if (user == null) return false;

            user.NombreUsuario = updatedUser.NombreUsuario;
            user.CorreoElectronico = updatedUser.CorreoElectronico;
            user.Contraseña = updatedUser.Contraseña;
            return true;
        }
    }
}
