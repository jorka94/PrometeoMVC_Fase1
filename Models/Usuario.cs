using System;

namespace PrometeoMVC.Models
{
    public class Usuario
    {

        public int UsuarioID { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Correo { get; set; }

        public string Contrasena { get; set; }

        public string Rol { get; set; }

    }
}