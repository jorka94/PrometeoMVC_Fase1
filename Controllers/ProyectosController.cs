using PrometeoMVC.Datos;
using PrometeoMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrometeoMVC.Controllers
{
    public class ProyectosController : Controller
    {

        Conexion con = new Conexion();

 
        public ActionResult Index()
        {
            List<Proyecto> lista = new List<Proyecto>();
            using (SqlConnection cn = con.Conectar())
            {
                string query = @"
                               SELECT   a.ProyectoID, a.NombreProyecto,a.TituloProyecto, a.Descripcion, b.nombre
                               FROM Proyectos a
                               INNER JOIN  EstadosProyecto b on b.EstadoID  = a.EstadoID";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Proyecto
                    {
                        ProyectoID = (int)dr["ProyectoID"],
                        NombreProyecto = dr["NombreProyecto"].ToString(),
                        TituloProyecto = dr["TituloProyecto"].ToString(),
                        Descripcion = dr["Descripcion"].ToString(),
                        Estado = dr["Nombre"].ToString()
                    });
                }
            }
            return View(lista);
        }

     
        public ActionResult Crear()
        {
    
            ViewBag.Areas = ListarAreas();
            return View();
        }

        [HttpPost]
        public ActionResult Crear(Proyecto modelo)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection cn = con.Conectar())
                {
                    string sql = "INSERT INTO Proyectos (UsuarioID, NombreProyecto, TituloProyecto, Descripcion, Objetivo, AreaID, Tipo, EstadoID)" +
                        "VALUES(@UsuarioID, @NombreProyecto, @TituloProyecto, @Descripcion,  @Objetivo, @AreaID, @Tipo , @EstadoID)";
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@UsuarioID", modelo.UsuarioID);
                    cmd.Parameters.AddWithValue("@NombreProyecto", modelo.NombreProyecto);
                    cmd.Parameters.AddWithValue("@TituloProyecto", modelo.TituloProyecto);
                    cmd.Parameters.AddWithValue("@Descripcion", modelo.Descripcion);
                    cmd.Parameters.AddWithValue("@Objetivo", modelo.Objetivo);
                    cmd.Parameters.AddWithValue("@AreaID", modelo.AreaID);
                    cmd.Parameters.AddWithValue("@Tipo", modelo.Tipo);
                    cmd.Parameters.AddWithValue("@EstadoID", modelo.EstadoID);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            ViewBag.Areas = ListarAreas(); 
            return View(modelo);
        }

      
        private List<SelectListItem> ListarAreas()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            using (SqlConnection cn = con.Conectar())
            {
                SqlCommand cmd = new SqlCommand("SELECT AreaID, Nombre FROM AreasInvestigacion", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new SelectListItem
                    {
                        Text = dr["Nombre"].ToString(),
                        Value = dr["AreaID"].ToString()
                    });
                }
            }
            return lista;

        }
    }
}