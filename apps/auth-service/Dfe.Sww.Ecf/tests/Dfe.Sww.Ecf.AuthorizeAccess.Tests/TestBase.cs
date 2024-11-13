using System.Security.Claims;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Dfe.Sww.Ecf.UiCommon.FormFlow.State;
using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests;

public abstract class TestBase : IDisposable
{
    private readonly TestScopedServices _testServices;

    protected TestBase(HostFixture hostFixture)
    {
        HostFixture = hostFixture;

        _testServices = TestScopedServices.Reset();

        HttpClient = hostFixture.CreateClient(new() { AllowAutoRedirect = false });
    }

    public HostFixture HostFixture { get; }

    public CaptureEventObserver EventObserver => _testServices.EventObserver;

    public TestableClock Clock => _testServices.Clock;

    public HttpClient HttpClient { get; }

    public TestData TestData => HostFixture.Services.GetRequiredService<TestData>();

    public async Task<JourneyInstance<SignInJourneyState>> CreateJourneyInstance(
        SignInJourneyState state
    )
    {
        await using var scope = HostFixture.Services.CreateAsyncScope();
        var stateProvider = scope.ServiceProvider.GetRequiredService<IUserInstanceStateProvider>();

        var journeyDescriptor = SignInJourneyState.JourneyDescriptor;

        var keysDict = new Dictionary<string, StringValues>
        {
            { Constants.UniqueKeyQueryParameterName, new StringValues(Guid.NewGuid().ToString()) },
        };

        var instanceId = new JourneyInstanceId(journeyDescriptor.JourneyName, keysDict);

        var stateType = typeof(SignInJourneyState);

        var instance = await stateProvider.CreateInstanceAsync(
            instanceId,
            stateType,
            state,
            properties: null
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

    public virtual void Dispose() { }

    public virtual async Task<T> WithDbContext<T>(Func<EcfDbContext, Task<T>> action)
    {
        var dbContextFactory = HostFixture.Services.GetRequiredService<
            IDbContextFactory<EcfDbContext>
        >();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await action(dbContext);
    }

    public virtual Task WithDbContext(Func<EcfDbContext, Task> action) =>
        WithDbContext(async dbContext =>
        {
            await action(dbContext);
            return 0;
        });

    public SignInJourneyHelper GetSignInJourneyHelper() =>
        HostFixture.Services.GetRequiredService<SignInJourneyHelper>();

    public AuthenticationTicket CreateOneLoginAuthenticationTicket(
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
        email ??= Faker.Internet.Email();

        var claims = new List<Claim>() { new("sub", sub), new("email", email) };

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

            firstName ??= Faker.Name.First();
            lastName ??= Faker.Name.Last();
            dateOfBirth ??= DateOnly.FromDateTime(Faker.Identification.DateOfBirth());

            var vc = TestData.CreateOneLoginCoreIdentityVc(firstName, lastName, dateOfBirth.Value);
            claims.Add(new Claim("vc", vc.RootElement.ToString(), "JSON"));
        }

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "OneLogin",
            nameType: "sub",
            roleType: null
        );

        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties();
        properties.SetVectorOfTrust(vtr);
        properties.StoreTokens(
            [
                new AuthenticationToken()
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = "dummy",
                },
            ]
        );

        return new AuthenticationTicket(principal, properties, authenticationScheme: "OneLogin");
    }

    public AuthenticationTicket CreateOneLoginAuthenticationTicket(string vtr, OneLoginUser user) =>
        CreateOneLoginAuthenticationTicket(
            vtr,
            user.Subject,
            user.Email,
            user.VerifiedNames?.First().First(),
            user.VerifiedNames?.First().Last(),
            user.VerifiedDatesOfBirth?.First()
        );

    public static SignInJourneyState CreateNewState(
        string redirectUri = "/",
        string? linkingToken = null
    ) => new(redirectUri, serviceName: "Test Service", serviceUrl: "https://service", linkingToken);
}
