using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PrometeoMVC.Datos
{
    public class Conexion
    {

      
        public static string Cadena = ConfigurationManager.ConnectionStrings["CadenaPrometeo"].ConnectionString;

        public SqlConnection Conectar()
        {
            return new SqlConnection(Cadena);

        }

    }
}