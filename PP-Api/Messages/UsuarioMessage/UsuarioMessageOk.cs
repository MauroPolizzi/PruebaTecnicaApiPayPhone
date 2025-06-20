namespace PP_Api.Messages.UsuarioMessage
{
    public static class UsuarioMessageOk
    {
        private static string _messageOk = string.Empty;

        public static string MessageCreateOk() => _messageOk = "Usuario creado con exito.";
        public static string MessageUpdateOk() => _messageOk = "Usuario actualizado con exito.";
        public static string MessageDeleteOk() => _messageOk = "Usuario eliminado con exito.";
    }
}
