using System.Data.SqlClient;
using System.Configuration;

namespace ProyectoFinal2
{
    public class ConexionBD
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
        private static SqlConnection connection;

        public static SqlConnection GetConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            else if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            return connection;
        }

        public static void CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
