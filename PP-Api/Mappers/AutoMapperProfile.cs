using AutoMapper;
using PP_Api.Modelos;
using PP_Dominio.Entidades;

namespace PP_Api.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BilleteraModel, Billetera>().ReverseMap();
            CreateMap<MovimientoModel, Movimiento>().ReverseMap();
        }
    }
}
