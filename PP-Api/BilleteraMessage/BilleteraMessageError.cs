namespace PP_Api.BilleteraMessage
{
    public static class BilleteraMessageError
    {
        public static string _messageError = string.Empty;

        public static string MessageDocumentOrNameExists() => _messageError = "Existe una billetera con el mismo DocumentId o Name. Por favor cambiarlo.";
        public static string MessageExists() => _messageError = "Billetera inexistente.";
        public static string MessageBalanceNegative() => _messageError = "El saldo no puede ser negativo.";
        public static string MessageCollectionEmpty() => _messageError = "No se encontro nada.";
    }
}
