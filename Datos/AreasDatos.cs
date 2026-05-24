using PrometeoMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PrometeoMVC.Datos
{
    public class AreasDatos
    {
        // Usamos la cadena de conexión que ya tienes en Conexion.cs
        public List<AreaInvestigacion> Listar()
        {
            List<AreaInvestigacion> lista = new List<AreaInvestigacion>();
            using (SqlConnection cn = new SqlConnection(Conexion.Cadena))
            {
                // Asegúrate que el nombre de la tabla sea igual al de tu script: AreasInvestigacion
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
            return lista;
        }
    }
}