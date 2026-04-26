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
    public class AreasController : Controller
    {

        Conexion con = new Conexion();

      
        public ActionResult Index()
        {
            List<AreaInvestigacion> lista = new List<AreaInvestigacion>();
            using (SqlConnection cn = con.Conectar())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM AreasInvestigacion", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new AreaInvestigacion
                    {
                        AreaID = (int)dr["AreaID"],
                        Nombre = dr["Nombre"].ToString()
                    });
                }
            }
            return View(lista);
        }

        public ActionResult Crear()
        {
            return View();
        }

    
        [HttpPost]
        public ActionResult Crear(AreaInvestigacion modelo)
        {
         
            if (ModelState.IsValid)
            {
                using (SqlConnection cn = con.Conectar())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO AreasInvestigacion(Nombre) VALUES(@nom)", cn);
                    cmd.Parameters.AddWithValue("@nom", modelo.Nombre);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(modelo); 
        }


    }
}