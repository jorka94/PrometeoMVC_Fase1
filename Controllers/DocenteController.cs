using PrometeoMVC.Datos;
using PrometeoMVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PrometeoMVC.Controllers
{
    public class DocenteController : Controller
    {
        Conexion con = new Conexion();

        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            string normalized = texto.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString();
        }

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
                    proyecto.NombreProyecto = NormalizarTexto(dr["NombreProyecto"].ToString());
                    proyecto.TituloProyecto = NormalizarTexto(dr["TituloProyecto"].ToString());
                    proyecto.Descripcion = NormalizarTexto(dr["Descripcion"].ToString());
                    proyecto.Estado = NormalizarTexto(dr["Estado"].ToString());

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
                if (estado == "Pendientes")
                {
                    lista = lista.Where(x => x.Estado == "Enviado" || x.Estado == "En revision").ToList();
                }
                else
                {
                    lista = lista.Where(x => x.Estado == estado).ToList();
                }
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                lista = lista.Where(x =>
                    x.NombreProyecto.ToLower().Contains(buscar.ToLower()) ||
                    x.TituloProyecto.ToLower().Contains(buscar.ToLower())
                ).ToList();
            }

            // Calcular contadores sobre TODOS los proyectos (no solo los filtrados)
            List<Proyecto> listaTodos = new List<Proyecto>();
            using (SqlConnection cnTodos = con.Conectar())
            {
                string queryTodos = @"
                    SELECT p.ProyectoID,
                           p.NombreProyecto,
                           p.TituloProyecto,
                           p.Descripcion,
                           p.FechaEnvio,
                           e.Nombre AS Estado
                    FROM Proyectos p
                    INNER JOIN EstadosProyecto e
                    ON p.EstadoID = e.EstadoID";

                SqlCommand cmdTodos = new SqlCommand(queryTodos, cnTodos);
                cnTodos.Open();

                SqlDataReader drTodos = cmdTodos.ExecuteReader();

                while (drTodos.Read())
                {
                    Proyecto proyecto = new Proyecto();

                    proyecto.ProyectoID = (int)drTodos["ProyectoID"];
                    proyecto.NombreProyecto = NormalizarTexto(drTodos["NombreProyecto"].ToString());
                    proyecto.TituloProyecto = NormalizarTexto(drTodos["TituloProyecto"].ToString());
                    proyecto.Descripcion = NormalizarTexto(drTodos["Descripcion"].ToString());
                    proyecto.Estado = NormalizarTexto(drTodos["Estado"].ToString());

                    if (drTodos["FechaEnvio"] != System.DBNull.Value)
                    {
                        proyecto.FechaEnvio = (System.DateTime)drTodos["FechaEnvio"];
                    }

                    listaTodos.Add(proyecto);
                }
            }

            ViewBag.Total = listaTodos.Count();
            ViewBag.Pendientes = listaTodos.Where(x => x.Estado == "Enviado" || x.Estado == "En revision").Count();
            ViewBag.Aprobados = listaTodos.Where(x => x.Estado == "Aprobado").Count();
            ViewBag.Rechazados = listaTodos.Where(x => x.Estado == "Rechazado").Count();
            ViewBag.Estado = estado;
            ViewBag.Buscar = buscar;

            // Debug: mostrar informacion de proyectos
            System.Diagnostics.Debug.WriteLine("Total proyectos: " + listaTodos.Count());
            foreach (var p in listaTodos)
            {
                System.Diagnostics.Debug.WriteLine("Proyecto: " + p.NombreProyecto + " - Estado: " + p.Estado);
            }

            return View(lista);
        }

        public ActionResult CerrarSesion()
        {
            Session["Docente"] = null;

            return RedirectToAction("Login", "Admin");
        }

        public ActionResult Observar(int id)
        {
            if (Session["Docente"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            Proyecto proyecto = new Proyecto();

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
                    ON p.EstadoID = e.EstadoID
                    WHERE p.ProyectoID = @ProyectoID";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@ProyectoID", id);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    proyecto.ProyectoID = (int)dr["ProyectoID"];
                    proyecto.NombreProyecto = dr["NombreProyecto"].ToString();
                    proyecto.TituloProyecto = dr["TituloProyecto"].ToString();
                    proyecto.Descripcion = dr["Descripcion"].ToString();
                    proyecto.Estado = dr["Estado"].ToString();

                    if (dr["FechaEnvio"] != System.DBNull.Value)
                    {
                        proyecto.FechaEnvio = (System.DateTime)dr["FechaEnvio"];
                    }
                }
            }

            return View(proyecto);
        }

        public ActionResult Aprobar(int id)
        {
            if (Session["Docente"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            using (SqlConnection cn = con.Conectar())
            {
                string query = "UPDATE Proyectos SET EstadoID = 4 WHERE ProyectoID = @ProyectoID";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@ProyectoID", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Rechazar(int id)
        {
            if (Session["Docente"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            using (SqlConnection cn = con.Conectar())
            {
                string query = "UPDATE Proyectos SET EstadoID = 5 WHERE ProyectoID = @ProyectoID";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@ProyectoID", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
