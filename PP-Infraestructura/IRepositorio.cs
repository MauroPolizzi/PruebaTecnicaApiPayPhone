using Microsoft.EntityFrameworkCore.Query;

namespace PP_Infraestructura
{
    public interface IRepositorio<T> where T : class
    {
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<IEnumerable<T>> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
          bool enabledTraking = true);
    }
}
