using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_WebAPI.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _context;

        // we need to pass context to the Repository as it expects context
        public VillaRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
        //public async Task CreateAsync(Villa entity)
        //{
        //    await _context.AddAsync(entity);
        //    await SaveAsync();
        //}

        //public async Task<Villa> GetAsync(Expression<Func<Villa, bool>> filter = null, bool tracked = true)
        //{
        //    IQueryable<Villa> query = _context.Villas;

        //    if (tracked != true)
        //    {
        //        query = query.AsNoTracking();
        //    }
        //    if (filter != null)
        //    {
        //        query = query.Where(filter);
        //    }

        //    // At this point query will be executed
        //    return await query.FirstOrDefaultAsync();
        //}

        //public async Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filter = null)
        //{
        //    IQueryable<Villa> query = _context.Villas;
        //    if (filter != null)
        //    {
        //        query = query.Where(filter);
        //    }

        //    // At this point query will be executed
        //    return await query.ToListAsync();

        //}

        //public async Task RemoveAsync(Villa entity)
        //{
        //    _context.Remove(entity);
        //    await SaveAsync();
        //}

        //public async Task SaveAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}
        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;

        }


    }
}
