using System.Linq.Expressions;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

public static class DbSetExtensions
{
    public static async Task<TEntity> AddIfNotExistsAsync<TEntity>(
        this DbSet<TEntity> dbSet,
        Expression<Func<TEntity, bool>> predicate,
        Func<TEntity> entityFactory,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        var existingEntity = await dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        if (existingEntity != null)
        {
            return existingEntity;
        }

        var entity = entityFactory();
        await dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }
}
