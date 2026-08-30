using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Infrastructure.Data;
using X.PagedList;
using X.PagedList.EF;
using X.PagedList.Extensions;

namespace Task_Management_API.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity).AsTask();

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync<TId>(TId id) where TId : notnull => await _dbSet.FindAsync(id);

        public async Task<IPagedList<T>> GetPagedAsync(int pageNumber, int pageSize) => 
            await _dbSet.AsNoTracking().ToPagedListAsync(pageNumber, pageSize);
        

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        public void Update(T entity) => _dbSet.Update(entity);
    }
}
