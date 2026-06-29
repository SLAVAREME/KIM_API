using KIM.DAL.Entities;
using System.Linq.Expressions;

namespace KIM.DAL.RepositoryBase.Contracts;

public interface IRepositoryAsync<T, in TKey>
    where T : BasicModel
{
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<T>> FindAllAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IRepositoryAsync<T> : IRepositoryAsync<T, Guid>
    where T : BasicModel
{
}