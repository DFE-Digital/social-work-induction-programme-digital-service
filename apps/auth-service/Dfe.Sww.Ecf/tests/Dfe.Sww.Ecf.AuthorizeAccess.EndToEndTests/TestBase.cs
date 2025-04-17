using Dfe.Sww.Ecf.AuthorizeAccess.EndToEndTests.Infrastructure.Security;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;

namespace Dfe.Sww.Ecf.AuthorizeAccess.EndToEndTests;

public abstract class TestBase(HostFixture hostFixture)
{
    public HostFixture HostFixture { get; } = hostFixture;

    public IClock Clock => HostFixture.Services.GetRequiredService<IClock>();

    public TestData TestData => HostFixture.Services.GetRequiredService<TestData>();

    public virtual async Task<T> WithDbContext<T>(Func<EcfDbContext, Task<T>> action)
    {
        var dbContextFactory = HostFixture.Services.GetRequiredService<IDbContextFactory<EcfDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await action(dbContext);
    }

    public void SetCurrentOneLoginUser(OneLoginUserInfo user)
    {
        var currentUserProvider = HostFixture.Services.GetRequiredService<OneLoginCurrentUserProvider>();
        currentUserProvider.CurrentUser = user;
    }
}
