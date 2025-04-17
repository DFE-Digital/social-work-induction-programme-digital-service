using Microsoft.Extensions.DependencyInjection;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;

namespace Dfe.Sww.Ecf.TestCommon;

public class DbFixture(DbHelper dbHelper, IServiceProvider serviceProvider)
{
    public DbHelper DbHelper { get; } = dbHelper;

    public IServiceProvider Services { get; } = serviceProvider;

    public EcfDbContext GetDbContext() => Services.GetRequiredService<EcfDbContext>();

    public IDbContextFactory<EcfDbContext> GetDbContextFactory() => Services.GetRequiredService<IDbContextFactory<EcfDbContext>>();

    public virtual async Task<T> WithDbContext<T>(Func<EcfDbContext, Task<T>> action)
    {
        await using var dbContext = await GetDbContextFactory().CreateDbContextAsync();
        return await action(dbContext);
    }

    public virtual Task WithDbContext(Func<EcfDbContext, Task> action) =>
        WithDbContext(async dbContext =>
        {
            await action(dbContext);
            return 0;
        });
}
