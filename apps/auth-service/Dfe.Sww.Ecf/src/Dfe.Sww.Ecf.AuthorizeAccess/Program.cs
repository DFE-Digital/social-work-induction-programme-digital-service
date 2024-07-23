using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Dfe.Analytics;
using Dfe.Analytics.AspNetCore;
using GovUk.Frontend.AspNetCore;
using GovUk.OneLogin.AspNetCore;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.IdentityModel.Tokens;
using Dfe.Sww.Ecf;
using Dfe.Sww.Ecf.AuthorizeAccess;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Filters;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Logging;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;
using Dfe.Sww.Ecf.AuthorizeAccess.TagHelpers;
using Dfe.Sww.Ecf.Core.Infrastructure;
using Dfe.Sww.Ecf.Core.Services.Files;
using Dfe.Sww.Ecf.Core.Services.PersonMatching;
using Dfe.Sww.Ecf.ServiceDefaults;
using Dfe.Sww.Ecf.SupportUi.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.UiCommon.Filters;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Dfe.Sww.Ecf.UiCommon.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.AddServiceDefaults(dataProtectionBlobName: "AuthorizeAccess");

builder.ConfigureLogging();

builder.Services.AddGovUkFrontend();
builder.Services.AddCsp(nonceByteAmount: 32);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = AuthenticationSchemes.MatchToTeachingRecord;

    options.AddScheme(AuthenticationSchemes.FormFlowJourney,
        scheme => { scheme.HandlerType = typeof(FormFlowJourneySignInHandler); });

    options.AddScheme(AuthenticationSchemes.MatchToTeachingRecord,
        scheme => { scheme.HandlerType = typeof(MatchToTeachingRecordAuthenticationHandler); });
}).AddOneLogin(options =>
{
    options.SignInScheme = AuthenticationSchemes.FormFlowJourney;

    options.Events.OnRedirectToIdentityProviderForSignOut = context =>
    {
        // The standard sign out process will call Authenticate() on SignInScheme then try to extract the id_token from the Principal.
        // That won't work in our case most of the time since sign out journeys won't have the FormFlow instance around that has the AuthenticationTicket.
        // Instead, we'll get it passed to us in explicitly in AuthenticationProperties.Items.

        if (context.ProtocolMessage.IdTokenHint is null &&
            context.Properties.Parameters.TryGetValue(OpenIdConnectParameterNames.IdToken, out var idToken) &&
            idToken is string idTokenString)
        {
            context.ProtocolMessage.IdTokenHint = idTokenString;
        }

        return Task.CompletedTask;
    };

    options.Events.OnAccessDenied = async context =>
    {
        // This handles the scenario where we've requested ID verification but One Login couldn't do it.

        if (context.Properties!.TryGetVectorOfTrust(out var vtr) &&
            vtr == SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr &&
            TryGetJourneyInstanceId(context.Properties, out var journeyInstanceId))
        {
            context.HandleResponse();

            var signInJourneyHelper = context.HttpContext.RequestServices.GetRequiredService<SignInJourneyHelper>();
            var journeyInstance =
                (await signInJourneyHelper.UserInstanceStateProvider.GetSignInJourneyInstanceAsync(context.HttpContext,
                    journeyInstanceId))!;

            var result = await signInJourneyHelper.OnVerificationFailed(journeyInstance);
            await result.ExecuteAsync(context.HttpContext);
        }
    };

    options.MetadataAddress = "https://oidc.integration.account.gov.uk/.well-known/openid-configuration";
    options.ClientAssertionJwtAudience = "https://oidc.integration.account.gov.uk/token";

    using (var rsa = RSA.Create())
    {
        var privateKeyPem = builder.Configuration["OneLogin:PrivateKeyPem"];
        rsa.ImportFromPem(privateKeyPem);

        options.ClientAuthenticationCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true)),
            SecurityAlgorithms.RsaSha256);
    }

    options.ClientId = builder.Configuration["OneLogin:ClientId"] ??
                       throw new InvalidOperationException("Missing OneLogin:ClientId");
    options.CallbackPath = "/_onelogin/callback";
    options.SignedOutCallbackPath = "/_onelogin/logout-callback";

    options.CorrelationCookie.Name = "onelogin-correlation.";
    options.NonceCookie.Name = "onelogin-nonce.";
    return;

    static bool TryGetJourneyInstanceId(
        AuthenticationProperties? properties,
        [NotNullWhen(true)] out JourneyInstanceId? journeyInstanceId)
    {
        if (properties?.Items.TryGetValue(FormFlowJourneySignInHandler.PropertyKeys.JourneyInstanceId,
                out var serializedInstanceId) == true
            && serializedInstanceId is not null)
        {
            journeyInstanceId = JourneyInstanceId.Deserialize(serializedInstanceId);
            return true;
        }

        journeyInstanceId = default;
        return false;
    }
});

builder.Services.AddDfeAnalytics()
    .AddAspNetCoreIntegration(options =>
    {
        options.UserIdClaimType = ClaimTypes.Subject;

        options.RequestFilter = ctx =>
            ctx.Request.Path != "/status" &&
            ctx.Request.Path != "/health" &&
            ctx.Features.Any(f => f.Key == typeof(IEndpointFeature));
    });

builder.Services
    .AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.Filters.Add(new NoCachePageFilter());
        options.Filters.Add(new AssignViewDataFromFormFlowJourneyResultFilterFactory());
    });

if (!builder.Environment.IsUnitTests() && !builder.Environment.IsEndToEndTests())
{
    builder.Services.AddDbContext<IdDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetRequiredConnectionString("Id")));
}

builder.AddBlobStorage();

builder.Services
    .AddEcfBaseServices()
    .AddTransient<AuthorizeAccessLinkGenerator, RoutingAuthorizeAccessLinkGenerator>()
    .AddTransient<FormFlowJourneySignInHandler>()
    .AddTransient<MatchToTeachingRecordAuthenticationHandler>()
    .AddHttpContextAccessor()
    .AddSingleton<IStartupFilter, FormFlowSessionMiddlewareStartupFilter>()
    .AddFormFlow(options => { options.JourneyRegistry.RegisterJourney(SignInJourneyState.JourneyDescriptor); })
    .AddSingleton<ICurrentUserIdProvider, FormFlowSessionCurrentUserIdProvider>()
    .AddTransient<SignInJourneyHelper>()
    .AddSingleton<ITagHelperInitializer<FormTagHelper>, FormTagHelperInitializer>()
    .AddFileService()
    .AddPersonMatching();

builder.Services.AddOptions<AuthorizeAccessOptions>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.AddTestApp();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/oauth2"),
    a =>
    {
        if (app.Environment.IsDevelopment())
        {
            a.UseDeveloperExceptionPage();
            a.UseMigrationsEndPoint();
        }
        else if (!app.Environment.IsUnitTests())
        {
            a.UseExceptionHandler("/error");
            a.UseStatusCodePagesWithReExecute("/error", "?code={0}");
        }
    });

app.UseCsp(csp =>
{
    var pageTemplateHelper = app.Services.GetRequiredService<PageTemplateHelper>();

    csp.ByDefaultAllow
        .FromSelf();

    csp.AllowScripts
        .FromSelf()
        .From(pageTemplateHelper.GetCspScriptHashes())
        .AddNonce();

    csp.AllowStyles
        .FromSelf()
        .AddNonce();

    // Ensure ASP.NET Core's auto refresh works
    // See https://github.com/dotnet/aspnetcore/issues/33068
    if (builder.Environment.IsDevelopment())
    {
        csp.AllowConnections
            .ToAnywhere();
    }
});

app.UseMiddleware<AppendSecurityResponseHeadersMiddleware>();

app.UseStaticFiles();

if (builder.Environment.IsProduction())
{
    app.UseDfeAnalytics();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

if (builder.Configuration["RootRedirect"] is { } rootRedirect)
{
    app.MapGet("/", ctx =>
    {
        ctx.Response.Redirect(rootRedirect);
        return Task.CompletedTask;
    });
}

app.Run();

public partial class Program
{
}
