using System;
using System.Threading.Tasks;

public interface IRepositorio<T> where T
{
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(T entity);
}
