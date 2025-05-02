Para inicializar el proyecto, se debe de crear una base datos en Sql Server, y cambiar los valores de 'Servidor' y 'BaseDatos' en la class ConexionBBDD del proyecto 'PP-Conexion'.

namespace PP_Conexion
{
    public class ConexionBBDD
    {
        private const string Servidor = @""; // Cambiar por el servidor de Sql
        private const string BaseDatos = ""; // Cambiar por el nombre de la base de datos

        public static string ObtenerCadenaConexionWin => $"Data Source={Servidor}; " +
                                                      $"Initial Catalog={BaseDatos}; " +
                                                      $"Integrated Security = true; " +
                                                      $"TrustServerCertificate = true; ";
    }
}
