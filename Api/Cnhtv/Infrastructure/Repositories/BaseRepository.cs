namespace Cnhtv.Infrastructure.Repositories;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cnhtv.Application.Repositories;

public class BaseRepository<TEntity, TId, TContext> : IRepository<TEntity, TId>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly TContext context;
    protected readonly DbSet<TEntity> entities;

    public BaseRepository(TContext context)
    {
        this.context = context;
        entities = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query()
    {
        return entities;
    }

    public Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return entities.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public void Insert(TEntity entity)
    {
        entities.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        entities.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
