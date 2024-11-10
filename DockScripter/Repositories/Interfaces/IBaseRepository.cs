using System.Linq.Expressions;
using DockScripter.Domain.Entities;
using DockScripter.Domain.Models;

namespace DockScripter.Repositories.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task<bool> DeleteById(Guid id, CancellationToken cancellationToken);
    Task<TEntity?> SelectById(Guid id, CancellationToken cancellationToken);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<List<TEntity>> SelectAll(CancellationToken cancellationToken);

    Task<List<TEntity>> SelectAll(DateTimeOffset fromDateTimeUtc, DateTimeOffset toDateTimeUtc, int page, int limit,
        CancellationToken cancellationToken);

    Task<PaginatedListDomain<T>> GetPaginatedList<T>(IOrderedQueryable<T> queryable, int page, int limit,
        CancellationToken cancellationToken) where T : class;
}