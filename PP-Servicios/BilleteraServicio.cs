using Microsoft.EntityFrameworkCore;
using PP_Dominio.Entidades;
using PP_Infraestructura;

namespace PP_Servicios
{
    public class BilleteraServicio : IBilleteraServicio
    {
        private readonly IRepositorio<Billetera> repositorio;
        private DataContext dataContext;

        public BilleteraServicio(IRepositorio<Billetera> repositorio, DataContext dataContext)
        {
            this.repositorio = repositorio; 
            this.dataContext = dataContext;
        }

        #region Metodos de busqueda

        public async Task<IEnumerable<Billetera>> GetAll()
        {
            var billeteras = await repositorio.GetAll();

            return billeteras.Select(x => new Billetera
            {
                Id = x.Id,
                EstaBorrado = x.EstaBorrado,
                DocumentId = x.DocumentId,
                Name = x.Name,
                Balance = x.Balance,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,

            }).ToList();
        }

        #endregion

        #region Metodos de persistencia

        public async Task Create(Billetera entity)
        {
            Billetera billetera = new Billetera
            {
                DocumentId = entity.DocumentId,
                Name = entity.Name,
                Balance = entity.Balance,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

                EstaBorrado = false,
            };

            await repositorio.Create(billetera);
        }

        public async Task Update(int id, Billetera entity)
        {
            Billetera billetera = dataContext.Billetera.FirstOrDefault(x => x.Id == id);

            billetera.Balance = entity.Balance;
            billetera.UpdatedAt = DateTime.Now;

            await repositorio.Update(billetera);
        }

        public async Task UpdateWalletForMovimiento(Movimiento movimiento)
        {
            Billetera billetera = dataContext.Billetera.FirstOrDefault(x => x.Id == movimiento.WalletId);

            billetera.Balance = 
                movimiento.Type == TypeOperation.Credito 
                ? billetera.Balance + movimiento.Amount
                : billetera.Balance - movimiento.Amount;
            
            billetera.UpdatedAt = DateTime.Now;

            await repositorio.Update(billetera);
        }

        public async Task Delete(int id)
        {
            Billetera billetera = dataContext.Billetera.FirstOrDefault(x => x.Id == id);

            await repositorio.Delete(billetera);
        }

        #endregion

        #region Metodos de verificacion

        /// <summary>
        /// Verifica si una billetera existe con el mismo DocumentId o Name
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool VerifyDocumentOrNameExists(string documentId, string name)
        {
            return dataContext.Billetera
                .Any(x => x.DocumentId.Equals(documentId) || x.Name.Equals(name));
        }

        public bool VerifyBalanceNegative(decimal balance)
        {
            return balance < 0;
        }

        public bool VerifyIdExist(int id)
        {
            return dataContext.Billetera.Any(x => x.Id == id);
        }

        #endregion
    }

    public interface IBilleteraServicio
    {
        Task Create(Billetera entity);
        Task Update(int id, Billetera entity);
        Task UpdateWalletForMovimiento(Movimiento movimiento);
        Task Delete(int id);
        Task<IEnumerable<Billetera>> GetAll();
        bool VerifyDocumentOrNameExists(string documentId, string name);
        bool VerifyBalanceNegative(decimal balance);
        bool VerifyIdExist(int id);
    }
}
