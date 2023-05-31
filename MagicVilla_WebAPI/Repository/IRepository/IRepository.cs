using MagicVilla_WebAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_WebAPI.Repository.IRepository
{
    //Genric repository
    // wheree T will be a class 
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, int pageSize = 3, int pageNo = 1);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
        Task<List<T>> SortbyParam(string sortBy, string sortOrder);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
