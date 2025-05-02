using Microsoft.Extensions.DependencyInjection;
using PP_Dominio.Entidades;
using PP_Infraestructura;
using PP_Servicios;

namespace PP_Inyeccions
{
    public class Inyeccion
    {
        public static void ConfiguracionServicios(IServiceCollection services)
        {
            services.AddDbContext<DataContext>();

            services.AddTransient<IBilleteraServicio, BilleteraServicio>();
            services.AddTransient<IRepositorio<Billetera>, Repositorio<Billetera>>();

            services.AddTransient<IMovimientoServicio, MovimientoServicio>();
            services.AddTransient<IRepositorio<Movimiento>, Repositorio<Movimiento>>();
        }
    }
}
