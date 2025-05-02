namespace PP_Conexion
{
    public class ConexionBBDD
    {
        private const string Servidor = @"BOOK3-MAURO";
        private const string BaseDatos = "PayPhoneBBDD";

        public static string ObtenerCadenaConexionWin => $"Data Source={Servidor}; " +
                                                      $"Initial Catalog={BaseDatos}; " +
                                                      $"Integrated Security = true; " +
                                                      $"TrustServerCertificate = true; ";
    }
}
