using Microsoft.EntityFrameworkCore;
using PP_Dominio.Entidades;
using static PP_Conexion.ConexionBBDD;

namespace PP_Infraestructura
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ObtenerCadenaConexionWin);

            base.OnConfiguring(optionsBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entidad in ChangeTracker.Entries()
               .Where(x => x.State == EntityState.Deleted
                           && x.OriginalValues.Properties
                               .Any(p => p.Name.Contains("EstaBorrado"))))
            {
                entidad.State = EntityState.Unchanged;
                entidad.CurrentValues["EstaBorrado"] = true;
            }

            return base.SaveChangesAsync();
        }

        public virtual DbSet<Billetera> Billetera { get; set; }
        public virtual DbSet<Movimiento> Movimiento { get; set; }
    }
}
