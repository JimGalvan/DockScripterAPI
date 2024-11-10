using System.Linq.Expressions;
using DockScripter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DockScripter.Domain.Models;
using DockScripter.Entities;
using DockScripter.Repositories.Contexts;

namespace DockScripter.Repositories;

public abstract class BaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DataContext _context;
    public DbSet<TEntity>? objects;

    public BaseRepository(DataContext context)
    {
        _context = context;
        objects = _context.Set<TEntity>();
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await _context.Set<TEntity>()
            .Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _context.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<bool> DeleteById(Guid id, CancellationToken cancellationToken)
    {
        TEntity? exist = await _context.Set<TEntity>()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (exist == null)
            return false;

        _context.Remove(exist);
        return true;
    }

    public virtual async Task<TEntity?> SelectById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<TEntity>()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<List<TEntity>> SelectAll(CancellationToken cancellationToken)
    {
        return await _context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<TEntity>> SelectAll(DateTimeOffset fromDateTimeUtc, DateTimeOffset toDateTimeUtc,
        int page, int limit, CancellationToken cancellationToken)
    {
        return await _context.Set<TEntity>()
            .Where(x => x.LastUpdatedDateTimeUtc >= fromDateTimeUtc && x.LastUpdatedDateTimeUtc <= toDateTimeUtc)
            .OrderBy(x => x.LastUpdatedDateTimeUtc)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedListDomain<T>> GetPaginatedList<T>(
        IOrderedQueryable<T> queryable,
        int page,
        int limit,
        CancellationToken cancellationToken) where T : class
    {
        var pagedQueryable = await queryable
            .Select(x => new
            {
                TotalCount = queryable.Count(),
                User = x
            })
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedListDomain<T>
        {
            TotalCount = pagedQueryable.FirstOrDefault()?.TotalCount ?? 0,
            Items = pagedQueryable.Select(x => x.User)
        };

        return paginatedList;
    }
}