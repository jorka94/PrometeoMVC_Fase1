using System.Linq; 
using System.Web.Mvc;
using PrometeoMVC.Models;
using PrometeoMVC.Datos;

namespace PrometeoMVC.Controllers
{
    public class EstudianteController : Controller
    {
        ProyectosDatos _pDatos = new ProyectosDatos();
        AreasDatos _aDatos = new AreasDatos();

        public ActionResult Index()
        {
            // aqui vamos a simular que el estudiantes logueado es el ID 1
            int idUsuarioLogueado = 1;

            //  acaparamos la lista por ADO.NET
            var listaCompleta = _pDatos.ListarTodo();

            // aplicamos el LINQ (Filtramos por usuario)
            var misProyectos = listaCompleta.Where(p => p.UsuarioID == idUsuarioLogueado).ToList();

            // CArgamos las áreas para el dropdown
            ViewBag.Areas = _aDatos.Listar().Select(a => new SelectListItem
            {
                Text = a.Nombre,
                Value = a.AreaID.ToString()
            }).ToList();

            return View(misProyectos);
        }

        [HttpPost]
        public ActionResult Registrar(Proyecto modelo)
        {
            modelo.UsuarioID = 1; // Simulado
            if (_pDatos.GuardarPropuesta(modelo))
            {
                return RedirectToAction("Index");
            }
            return View("Index");
        }
    }
}