using System.ComponentModel.DataAnnotations;

namespace PP_Dominio.Base
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Required]
        public bool EstaBorrado { get; set; }
    }
}
