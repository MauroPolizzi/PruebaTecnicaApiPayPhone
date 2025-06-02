namespace PP_Api.MovimientoMessage
{
    public static class MovimientoMessageOK
    {
        private static string _messageOK = string.Empty;

        public static string MessageCreateOk() => _messageOK = "Movimiento creado correctamente.";
        public static string MessageCreateTransferOk() => _messageOK = "Transferencia realizada correctamente.";
    }
}
