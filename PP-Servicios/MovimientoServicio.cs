using PP_Dominio.Entidades;
using PP_Infraestructura;

namespace PP_Servicios
{
    public class MovimientoServicio : IMovimientoServicio
    {
        private readonly IRepositorio<Movimiento> repositorioMovimiento;
        private readonly IRepositorio<Billetera> repositorioBilletera;
        private DataContext dataContext;
        private IEnumerable<Billetera> billeteras;

        public MovimientoServicio(IRepositorio<Movimiento> repositorioMovimiento, IRepositorio<Billetera> repositorioBilletera, DataContext dataContext)
        {
            this.repositorioMovimiento = repositorioMovimiento;
            this.repositorioBilletera = repositorioBilletera;
            this.dataContext = dataContext;

            // Cargamos las billeteras
            GetAllBilleteras();
        }

        #region Metodos de busqueda

        public async Task<IEnumerable<Movimiento>> GetAll()
        {
            var movimientos = await repositorioMovimiento.GetAll();

            return movimientos.Select(x => new Movimiento
            {
                Id = x.Id,
                EstaBorrado = x.EstaBorrado,
                WalletId = x.WalletId,
                Amount = x.Amount,
                Type = x.Type,
                CreatedAt = x.CreatedAt,

            }).ToList();
        }

        #endregion

        #region Metodos de persistencia

        public async Task Create(Movimiento entity)
        {
            Movimiento movimiento = new Movimiento
            {
                WalletId = entity.WalletId,
                Amount = entity.Amount,
                Type = entity.Type,
                CreatedAt = DateTime.Now,

                EstaBorrado = false,
            };

            await repositorioMovimiento.Create(movimiento);
        }

        #endregion

        #region Metodos de verificacion

        public bool VerifyAmountGreaterThanOrEqualToBalance(string documentId, string name, decimal amount)
        {
            Billetera billetera = billeteras.FirstOrDefault(x => x.DocumentId == documentId && x.Name == name);

            return billetera.Balance >= amount;
        }

        public bool VerifyNegativeAmountOrZero(decimal amount)
        {
            return amount <= 0;
        }

        public bool VerifyWalletExist(string documentId, string name)
        {
            return billeteras.Any(x => x.DocumentId == documentId && x.Name == name);
        }

        #endregion

        #region Metodos privados

        /// <summary>
        /// Obtiene todas las billeteras desde BBDD
        /// </summary>
        private void GetAllBilleteras()
        {
            billeteras = dataContext.Billetera.ToList();
        }

        #endregion
    }

    public interface IMovimientoServicio
    {
        Task Create(Movimiento entity);
        Task<IEnumerable<Movimiento>> GetAll();
        bool VerifyWalletExist(string documentId, string name);
        bool VerifyNegativeAmountOrZero(decimal amount);
        bool VerifyAmountGreaterThanOrEqualToBalance(string documentId, string name, decimal amount);

    }
}
