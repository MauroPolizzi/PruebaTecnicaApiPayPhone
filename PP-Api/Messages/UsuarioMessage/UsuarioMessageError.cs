namespace PP_Api.Messages.UsuarioMessage
{
    public static class UsuarioMessageError
    {
        private static string _messageError = string.Empty;

        public static string MessageExists() => _messageError = "Usuario inexistente.";
        public static string MessageUserNameExists() => _messageError = "UserName existente";
        public static string MessageCollectionEmpty() => _messageError = "No se encontro nada.";
    }
}
