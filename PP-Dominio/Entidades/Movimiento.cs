using PP_Dominio.Base;

namespace PP_Dominio.Entidades
{
    public class Movimiento : EntityBase
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public TypeOperation Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum TypeOperation
    {
        None = 0,
        Debito = 1,
        Credito = 2
    }
}
