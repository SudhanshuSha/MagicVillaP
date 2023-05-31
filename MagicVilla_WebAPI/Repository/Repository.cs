using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_WebAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            // will be assign dbset to genric type
            this.dbSet = _context.Set<T>();
        }
        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (tracked != true)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }



            // At this point query will be executed
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> SortbyParam(string sortBy, string sortOrder)
        {
            IQueryable<T> query = dbSet;

            var type = typeof(T);
            // it will fetch the property example Id
            var property = type.GetProperty(sortBy);
            // it will set t as expession
            var expression2 = Expression.Parameter(type, "t");
            // it will  t.id
            var expression1 = Expression.MakeMemberAccess(expression2, property);
            // t=> t.tid
            var lambda = Expression.Lambda(expression1, expression2);
      
            var result = Expression.Call(
             typeof(Queryable),
             sortOrder == "desc" ? "OrderByDescending" : "OrderBy",
             new Type[] { type, property.PropertyType },
             query.Expression,
             Expression.Quote(lambda));

            var data= query.Provider.CreateQuery<T>(result);

            return await data.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, int pageSize = 0, int pageNo = 1)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                //paeno=2 || page size=5
                //skip(5).takr(5) skip first take second
                query = query.Skip(pageSize * (pageNo - 1)).Take(pageSize);

            }
            // At this point query will be executed
            return await query.ToListAsync();

        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
