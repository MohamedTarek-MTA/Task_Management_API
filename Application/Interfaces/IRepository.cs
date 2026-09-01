using System.Linq.Expressions;
using X.PagedList;

namespace Task_Management_API.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        //Read
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync<TId>(TId id) where TId : notnull;
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<IPagedList<T>> GetPagedAsync(int pageNumber, int pageSize);

        //Write, Edit, Delete
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        //Save changes
        Task<bool> SaveChangesAsync();
    }
}
