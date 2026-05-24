using PrometeoMVC.Datos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrometeoMVC.Models;

namespace PrometeoMVC.Controllers
{
    public class AdminController : Controller
    {

        Conexion con = new Conexion();

        // GET: Admin
        public ActionResult Index()
        {

            // ar-alxrm: validamos si existe sesion admin
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string correo, string contrasena)
        {

            using (SqlConnection cn = con.Conectar())
            {

                string query = @"SELECT *
                 FROM Usuarios
                 WHERE Correo = @Correo
                 AND Contraseña = @Contrasena
                 AND (Rol = 'Administrador'
                 OR Rol = 'Docente')";

                SqlCommand cmd = new SqlCommand(query, cn);

                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Contrasena", contrasena);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {

                    string rol = dr["Rol"].ToString();

                    if (rol == "Docente")
                    {
                        Session["Docente"] = dr["Nombres"].ToString();

                        return RedirectToAction("Index", "Docente");
                    }

                    // ar-alxrm: guardamos sesion del admin
                    Session["Admin"] = dr["Nombres"].ToString();

                    return RedirectToAction("Index");

                }

            }

            ViewBag.Error = "Correo o contraseña incorrectos";

            return View();

        }

        public ActionResult CerrarSesion()
        {

            Session.Clear();

            return RedirectToAction("Login");

        }

        public ActionResult ListaProyectos(string estado, string buscar)
        {

            // ar-alxrm: validamos sesion admin
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            List<Proyecto> lista = new List<Proyecto>();

            using (SqlConnection cn = con.Conectar())
            {

                string query = @"
                        SELECT p.ProyectoID,
                               p.NombreProyecto,
                               p.TituloProyecto,
                               p.Descripcion,
                               e.Nombre AS Estado
                        FROM Proyectos p
                        INNER JOIN EstadosProyecto e
                        ON p.EstadoID = e.EstadoID";

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
                        Estado = dr["Estado"].ToString()

                    });

                }

            }

            // ar-alxrm: aqui aplicamos filtro LINQ por estado
            if (!string.IsNullOrEmpty(estado))
            {

                lista = lista
                        .Where(x => x.Estado == estado)
                        .ToList();

            }

            // ar-alxrm: filtro LINQ por nombre del proyecto
            if (!string.IsNullOrEmpty(buscar))
            {

                lista = lista
                        .Where(x => x.NombreProyecto.ToLower()
                        .Contains(buscar.ToLower()))
                        .ToList();

            }

            return View(lista);


        }

        public ActionResult ListaUsuarios()
        {

            // ar-alxrm: validamos sesion admin
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection cn = con.Conectar())
            {

                string query = @"
                SELECT UsuarioID,
                       Nombres,
                       Apellidos,
                       Correo,
                       Rol
                FROM Usuarios";

                SqlCommand cmd = new SqlCommand(query, cn);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {

                    lista.Add(new Usuario
                    {

                        UsuarioID = (int)dr["UsuarioID"],
                        Nombres = dr["Nombres"].ToString(),
                        Apellidos = dr["Apellidos"].ToString(),
                        Correo = dr["Correo"].ToString(),
                        Rol = dr["Rol"].ToString()

                    });

                }

            }

            return View(lista);

        }

        public ActionResult CrearUsuario()
        {

            // ar-alxrm: validamos sesion admin
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();

        }

        [HttpPost]
        public ActionResult CrearUsuario(Usuario modelo)
        {

            using (SqlConnection cn = con.Conectar())
            {

                // ar-alxrm: validamos si el correo ya existe
                string validar = @"
                SELECT COUNT(*)
                FROM Usuarios
                WHERE Correo = @Correo";

                SqlCommand validarCmd =
                    new SqlCommand(validar, cn);

                validarCmd.Parameters.AddWithValue(
                    "@Correo",
                    modelo.Correo);

                cn.Open();

                int existe =
                    (int)validarCmd.ExecuteScalar();

                // ar-alxrm: si el correo existe mostramos error
                if (existe > 0)
                {

                    ViewBag.Error =
                        "El correo ya existe en el sistema";

                    return View();

                }

                cn.Close();

                // ar-alxrm: insertamos nuevo usuario
                string query = @"
                INSERT INTO Usuarios
                (
                    Nombres,
                    Apellidos,
                    Correo,
                    Contraseña,
                    Rol
                )
                VALUES
                (
                    @Nombres,
                    @Apellidos,
                    @Correo,
                    @Contrasena,
                    @Rol
                )";

                SqlCommand cmd = new SqlCommand(query, cn);

                cmd.Parameters.AddWithValue(
                    "@Nombres",
                    modelo.Nombres);

                cmd.Parameters.AddWithValue(
                    "@Apellidos",
                    modelo.Apellidos);

                cmd.Parameters.AddWithValue(
                    "@Correo",
                    modelo.Correo);

                cmd.Parameters.AddWithValue(
                    "@Contrasena",
                    modelo.Contrasena);

                cmd.Parameters.AddWithValue(
                    "@Rol",
                    modelo.Rol);

                cn.Open();

                cmd.ExecuteNonQuery();

            }

            return RedirectToAction("ListaUsuarios");

        }

    }
}
