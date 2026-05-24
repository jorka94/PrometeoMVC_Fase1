using PrometeoMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PrometeoMVC.Datos
{
    public class ProyectosDatos
    {
        //  metodo pa´ guardar la propuesta
        public bool GuardarPropuesta(Proyecto m)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.Cadena))
            {
                string sql = @"INSERT INTO Proyectos (UsuarioID, NombreProyecto, TituloProyecto, Descripcion, Objetivo, AreaID, Tipo, EstadoID, FechaEnvio) 
                               VALUES (@uid, @nom, @tit, @des, @obj, @aid, @tip, 1, GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@uid", m.UsuarioID);
                cmd.Parameters.AddWithValue("@nom", m.NombreProyecto);
                cmd.Parameters.AddWithValue("@tit", m.TituloProyecto);
                cmd.Parameters.AddWithValue("@des", m.Descripcion ?? "");
                cmd.Parameters.AddWithValue("@obj", m.Objetivo ?? "");
                cmd.Parameters.AddWithValue("@aid", m.AreaID);
                cmd.Parameters.AddWithValue("@tip", m.Tipo);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // metodo pa´ listar (esto se trae todo ya para que despues filtremos con LINQ en el controlador)
        public List<Proyecto> ListarTodo()
        {
            var lista = new List<Proyecto>();
            using (SqlConnection cn = new SqlConnection(Conexion.Cadena))
            {
                string sql = "SELECT p.*, e.Nombre as EstadoNombre FROM Proyectos p INNER JOIN EstadosProyecto e ON p.EstadoID = e.EstadoID";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Proyecto
                    {
                        ProyectoID = (int)dr["ProyectoID"],
                        UsuarioID = (int)dr["EstudianteID"],
                        NombreProyecto = dr["NombreProyecto"].ToString(),
                        Estado = dr["EstadoNombre"].ToString(),
                        FechaEnvio = (DateTime)dr["FechaEnvio"]
                    });
                }
            }
            return lista;
        }
    }
}