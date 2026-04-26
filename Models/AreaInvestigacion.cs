using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrometeoMVC.Models
{
    public class AreaInvestigacion
    {
        public int AreaID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre del Área")]
        public string Nombre { get; set; }
    }
}