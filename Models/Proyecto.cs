using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrometeoMVC.Models
{
    public class Proyecto
    {
        public int ProyectoID { get; set; }
        public int UsuarioID { get; set; } // FK a la tabla Usuarios

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreProyecto { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string TituloProyecto { get; set; }

        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public int AreaID { get; set; }
        public string Tipo { get; set; } 
        public int EstadoID { get; set; }

        // esto para mostrar los nombres en las tabls (JOINs)
        public string Estado { get; set; }
        public string AreaNombre { get; set; }
        public DateTime FechaEnvio { get; set; }
    }
}