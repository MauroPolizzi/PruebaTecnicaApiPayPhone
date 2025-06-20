namespace PP_Api.Messages.BilleteraMessage
{
    public static class BilleteraMessageOk
    {
        private static string _messageOk = string.Empty;

        public static string MessageCreateOK() => _messageOk = "Billetera creada correctamente.";
        public static string MessageUpdateOK() => _messageOk = "Billetera actualizada correctamente.";
        public static string MessageDeleteOK() => _messageOk = "Billetera eliminada correctamente.";
    }
}
