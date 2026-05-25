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

            // Actualizar automaticamente proyectos de Borrador a En revision
            using (SqlConnection cn = con.Conectar())
            {
                string query = "UPDATE Proyectos SET EstadoID = 3 WHERE EstadoID = 1";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                cmd.ExecuteNonQuery();
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

                string query = @" SELECT *
                 FROM Usuarios
                 WHERE Correo = @Correo
                 AND Contraseña = @Contrasena
                 AND (Rol = 'Administrador'
                 OR Rol = 'Docente'
                 OR Rol = 'Estudiante')";

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

                    if (rol == "Estudiante")
                    {
                        Session["Estudiante"] = dr["Nombres"].ToString();
                        int usuarioID = (int)dr["UsuarioID"];
                        dr.Close(); // cerramos el reader

                        SqlCommand cmdEst = new SqlCommand(
                            "SELECT EstudianteID FROM Estudiantes WHERE UsuarioID = @uid", cn);
                        cmdEst.Parameters.AddWithValue("@uid", usuarioID);
                        object result = cmdEst.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            Session["EstudianteID"] = (int)result;
                        }
                        else
                        {
                            ViewBag.Error = "El usuario estudiante no tiene un registro en la tabla Estudiantes. Contacte al administrador.";
                            return View("Login");
                        }

                        return RedirectToAction("Index", "Estudiante");
                    }

                    // guardamos sesion del admin
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
                if (estado == "Pendientes")
                {
                    lista = lista.Where(x => x.Estado == "Enviado" || x.Estado == "En revision").ToList();
                }
                else
                {
                    lista = lista.Where(x => x.Estado == estado).ToList();
                }
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

                // Si el rol es Estudiante, crear registro en tabla Estudiantes
                if (modelo.Rol == "Estudiante")
                {
                    cn.Close();
                    cn.Open();

                    // Obtener el UsuarioID recién creado
                    string getIDQuery = "SELECT TOP 1 UsuarioID FROM Usuarios WHERE Correo = @Correo ORDER BY UsuarioID DESC";
                    SqlCommand getIDCmd = new SqlCommand(getIDQuery, cn);
                    getIDCmd.Parameters.AddWithValue("@Correo", modelo.Correo);
                    int nuevoUsuarioID = (int)getIDCmd.ExecuteScalar();

                    // Crear registro en Estudiantes con valores por defecto
                    string insertEstudiante = @"
                        INSERT INTO Estudiantes (UsuarioID, Nombres, Apellidos, Carnet, CarreraID, CicloID)
                        VALUES (@UsuarioID, @Nombres, @Apellidos, 'PENDIENTE', 1, 1)";
                    
                    SqlCommand estCmd = new SqlCommand(insertEstudiante, cn);
                    estCmd.Parameters.AddWithValue("@UsuarioID", nuevoUsuarioID);
                    estCmd.Parameters.AddWithValue("@Nombres", modelo.Nombres);
                    estCmd.Parameters.AddWithValue("@Apellidos", modelo.Apellidos);
                    estCmd.ExecuteNonQuery();
                }

            }

            return RedirectToAction("ListaUsuarios");

        }

    }
}
