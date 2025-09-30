using System.Security.Claims;
using Bogus;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Dfe.Sww.Ecf.UiCommon.FormFlow.State;
using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests;

public abstract class TestBase(HostFixture hostFixture) : IDisposable
{
    private readonly TestScopedServices _testServices = TestScopedServices.Reset();
    protected readonly Faker Faker = new("en_GB");

    protected HostFixture HostFixture { get; } = hostFixture;

    protected TestableClock Clock => _testServices.Clock;

    protected HttpClient HttpClient { get; } = hostFixture.CreateClient(new WebApplicationFactoryClientOptions
        { AllowAutoRedirect = false });

    protected TestData TestData => HostFixture.Services.GetRequiredService<TestData>();

    public virtual void Dispose()
    {
        HttpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    protected async Task<JourneyInstance<SignInJourneyState>> CreateJourneyInstance(
        SignInJourneyState state
    )
    {
        await using var scope = HostFixture.Services.CreateAsyncScope();
        var stateProvider = scope.ServiceProvider.GetRequiredService<IUserInstanceStateProvider>();

        var journeyDescriptor = SignInJourneyState.JourneyDescriptor;

        var keysDict = new Dictionary<string, StringValues>
        {
            { Constants.UniqueKeyQueryParameterName, new StringValues(Guid.NewGuid().ToString()) }
        };

        var instanceId = new JourneyInstanceId(journeyDescriptor.JourneyName, keysDict);

        var stateType = typeof(SignInJourneyState);

        var instance = await stateProvider.CreateInstanceAsync(
            instanceId,
            stateType,
            state,
            null
        );
        return (JourneyInstance<SignInJourneyState>)instance;
    }

    public async Task<JourneyInstance<SignInJourneyState>> ReloadJourneyInstance(
        JourneyInstance<SignInJourneyState> journeyInstance
    )
    {
        await using var scope = HostFixture.Services.CreateAsyncScope();
        var stateProvider = scope.ServiceProvider.GetRequiredService<IUserInstanceStateProvider>();
        var reloadedInstance = await stateProvider.GetInstanceAsync(
            journeyInstance.InstanceId,
            typeof(SignInJourneyState)
        );
        return (JourneyInstance<SignInJourneyState>)reloadedInstance!;
    }

    protected virtual async Task<T> WithDbContext<T>(Func<EcfDbContext, Task<T>> action)
    {
        var dbContextFactory = HostFixture.Services.GetRequiredService<
            IDbContextFactory<EcfDbContext>
        >();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await action(dbContext);
    }

    protected Task WithDbContext(Func<EcfDbContext, Task> action)
    {
        return WithDbContext(async dbContext =>
        {
            await action(dbContext);
            return 0;
        });
    }

    protected SignInJourneyHelper GetSignInJourneyHelper()
    {
        return HostFixture.Services.GetRequiredService<SignInJourneyHelper>();
    }

    protected AuthenticationTicket CreateOneLoginAuthenticationTicket(
        string vtr,
        string? sub = null,
        string? email = null,
        string? firstName = null,
        string? lastName = null,
        DateOnly? dateOfBirth = null,
        bool? createCoreIdentityVc = null
    )
    {
        sub ??= TestData.CreateOneLoginUserSubject();
        email ??= Faker.Person.Email;

        var claims = new List<Claim> { new("sub", sub), new("email", email) };

        createCoreIdentityVc ??=
            vtr == SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr;

        if (createCoreIdentityVc == true)
        {
            if (vtr == SignInJourneyHelper.AuthenticationOnlyVtr)
            {
                throw new ArgumentException(
                    "Cannot assign core identity VC with authentication-only vtr.",
                    nameof(vtr)
                );
            }

            firstName ??= Faker.Person.FirstName;
            lastName ??= Faker.Person.LastName;
            dateOfBirth ??= DateOnly.FromDateTime(Faker.Person.DateOfBirth);

            var vc = TestData.CreateOneLoginCoreIdentityVc(firstName, lastName, dateOfBirth.Value);
            claims.Add(new Claim("vc", vc.RootElement.ToString(), "JSON"));
        }

        var identity = new ClaimsIdentity(
            claims,
            "OneLogin",
            "sub",
            null
        );

        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties();
        properties.SetVectorOfTrust(vtr);
        properties.StoreTokens(
            [
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = "dummy"
                }
            ]
        );

        return new AuthenticationTicket(principal, properties, "OneLogin");
    }

    protected AuthenticationTicket CreateOneLoginAuthenticationTicket(string vtr, OneLoginUser user)
    {
        return CreateOneLoginAuthenticationTicket(
            vtr,
            user.Subject,
            user.Email,
            user.VerifiedNames?.First().First(),
            user.VerifiedNames?.First().Last(),
            user.VerifiedDatesOfBirth?.First()
        );
    }

    protected static SignInJourneyState CreateNewState(
        string redirectUri = "/",
        string? linkingToken = null
    )
    {
        return new SignInJourneyState(redirectUri, "Test Service", "https://service", linkingToken);
    }
}
