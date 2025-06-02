namespace PP_Api.MovimientoMessage
{
    public static class MovimientoMessageError
    {
        private static string _messageError = string.Empty;

        public static string MessageWalletExist() => _messageError = "No existe la billetera a la que quiere transferir.";
        public static string MessageAmountGreaterThanOrEqualToBalance() => _messageError = "El monto a transferir supera el que tiene en la billetera.";
        public static string MessageNegativeAmountOrZero() => _messageError = "El monto a transferir no puede ser cero o negativo.";
        public static string MessageCollectionEmpty() => _messageError = "No se encontro nada.";
    }
}
