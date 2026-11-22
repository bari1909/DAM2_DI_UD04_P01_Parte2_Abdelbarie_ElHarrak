using System.Collections.Generic;
using Veterinario_MVVM.Models;

namespace Veterinario_MVVM.Services
{
    // AuthService.cs: Valida las credenciales contra una lista de usuarios autorizados
    public class AuthService
    {
        private readonly List<Usuario> _usuarios = new List<Usuario>
        {
            new Usuario { Username = "admin", Password = "123", NombreCompleto = "Laura Martínez Soler", Rol = "Administrador Clínico" },
            new Usuario { Username = "veterinario", Password = "123", NombreCompleto = "Carlos Ruiz Pérez", Rol = "Veterinario Colegiado" }
        };

        public Usuario? ValidarLogin(string username, string password)
        {
            foreach (Usuario usuario in _usuarios)
            {
                if (usuario.Username == username && usuario.Password == password)
                {
                    return usuario;
                }
            }

            return null;
        }
    }
}
