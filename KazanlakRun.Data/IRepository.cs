using System.Linq.Expressions;

namespace KazanlakRun.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync<TId>(TId id) where TId : notnull;
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        Task AddAsync(TEntity entity);
        Task SaveChangesAsync();
        void Update(TEntity entity);
        void Remove(TEntity entity);

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null);
    }
}
