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
        public int EstudianteID { get; set; }
        public int UsuarioID { get; set; }

        public int Tipo { get; set; }

        public int EstadoID { get; set; }

        public string NombreProyecto { get; set; }

        public string Objetivo { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [Display(Name = "Título del Proyecto")]
        public string TituloProyecto { get; set; }

        public string Descripcion { get; set; }

        [Display(Name = "Area de Investigacion")]
        public int AreaID { get; set; }

 
        public string Estado { get; set; }

        [Display(Name = "Fecha de Envío")]
        public DateTime FechaEnvio { get; set; }
    }
}