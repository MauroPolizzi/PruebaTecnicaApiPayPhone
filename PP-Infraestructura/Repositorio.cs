
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace PP_Infraestructura
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        #region Persistecia
        public async Task Create(T entity)
        {
            using (var context = new DataContext())
            {
                await context.Set<T>().AddAsync(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task Update(T entity)
        {
            using (var context = new DataContext())
            {
                context.Entry(entity).State = EntityState.Modified;

                context.Set<T>().Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(T entity)
        {
            using (var context = new DataContext())
            {
                context.Entry(entity).State = EntityState.Deleted;

                context.Set<T>().Remove(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool enabledTraking = true)
        {
            using (var context = new DataContext())
            {
                IQueryable<T> query = context.Set<T>();

                if (enabledTraking)
                {
                    query = query.AsNoTracking();
                }

                if (include != null)
                {
                    query = include(query);
                }


                return orderBy != null
                    ? await orderBy(query).ToListAsync()
                    : await query.ToListAsync();
            }
        }
        #endregion
    }
}
