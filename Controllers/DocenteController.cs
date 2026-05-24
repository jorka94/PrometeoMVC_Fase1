using PrometeoMVC.Datos;
using PrometeoMVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PrometeoMVC.Controllers
{
    public class DocenteController : Controller
    {
        Conexion con = new Conexion();

        public ActionResult Index(string estado, string buscar)
        {
            if (Session["Docente"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            List<Proyecto> lista = new List<Proyecto>();

            using (SqlConnection cn = con.Conectar())
            {
                string query = @"
                    SELECT p.ProyectoID,
                           p.NombreProyecto,
                           p.TituloProyecto,
                           p.Descripcion,
                           p.FechaEnvio,
                           e.Nombre AS Estado
                    FROM Proyectos p
                    INNER JOIN EstadosProyecto e
                    ON p.EstadoID = e.EstadoID";

                SqlCommand cmd = new SqlCommand(query, cn);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Proyecto proyecto = new Proyecto();

                    proyecto.ProyectoID = (int)dr["ProyectoID"];
                    proyecto.NombreProyecto = dr["NombreProyecto"].ToString();
                    proyecto.TituloProyecto = dr["TituloProyecto"].ToString();
                    proyecto.Descripcion = dr["Descripcion"].ToString();
                    proyecto.Estado = dr["Estado"].ToString();

                    if (dr["FechaEnvio"] != System.DBNull.Value)
                    {
                        proyecto.FechaEnvio = (System.DateTime)dr["FechaEnvio"];
                    }

                    lista.Add(proyecto);
                }
            }

            // Filtros con LINQ para la interfaz docente.
            if (!string.IsNullOrEmpty(estado))
            {
                lista = lista.Where(x => x.Estado == estado).ToList();
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                lista = lista.Where(x =>
                    x.NombreProyecto.ToLower().Contains(buscar.ToLower()) ||
                    x.TituloProyecto.ToLower().Contains(buscar.ToLower())
                ).ToList();
            }

            ViewBag.Total = lista.Count();
            ViewBag.Pendientes = lista.Where(x => x.Estado == "Pendiente").Count();
            ViewBag.Aprobados = lista.Where(x => x.Estado == "Aprobado").Count();
            ViewBag.Rechazados = lista.Where(x => x.Estado == "Rechazado").Count();
            ViewBag.Estado = estado;
            ViewBag.Buscar = buscar;

            return View(lista);
        }

        public ActionResult CerrarSesion()
        {
            Session["Docente"] = null;

            return RedirectToAction("Login", "Admin");
        }
    }
}
