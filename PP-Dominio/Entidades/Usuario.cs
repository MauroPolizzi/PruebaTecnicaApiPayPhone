using PP_Dominio.Base;

namespace PP_Dominio.Entidades
{
    public class Usuario : EntityBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
