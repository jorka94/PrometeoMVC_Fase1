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
            if (Session["EstudianteID"] == null)
                return RedirectToAction("Login", "Admin");

            int idUsuarioLogueado = (int)Session["EstudianteID"];

            var listaCompleta = _pDatos.ListarTodo();
            var misProyectos = listaCompleta.Where(p => p.EstudianteID == idUsuarioLogueado).ToList();

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
            if (Session["EstudianteID"] == null)
                return RedirectToAction("Login", "Admin");

            modelo.EstudianteID = (int)Session["EstudianteID"];

            if (_pDatos.GuardarPropuesta(modelo))
            {
                TempData["Exito"] = "Proyecto registrado exitosamente!";
                return RedirectToAction("Index");
            }

            return View("Index");
        }



    }
}