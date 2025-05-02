using PP_Dominio.Base;

namespace PP_Dominio.Entidades
{
    public class Billetera : EntityBase
    {
        public string DocumentId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
