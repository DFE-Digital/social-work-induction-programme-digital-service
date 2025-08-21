using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.Filters;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.Installers;
using Dfe.Sww.Ecf.Frontend.Routing;
using GovUk.Frontend.AspNetCore;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Protocols.Configuration;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

var featureFlagsConfiguration = builder.Configuration
    .GetRequiredSection("FeatureFlags");
builder.Services.Configure<FeatureFlags>(featureFlagsConfiguration);
var featureFlags = featureFlagsConfiguration.Get<FeatureFlags>();
if (featureFlags is null) throw new InvalidConfigurationException("Unable to parse feature flags configuration.");

// Need forwarded headers if using Azure Front Door. Note, this needs to be done
// early, before authentication
if (featureFlags.EnableForwardedHeaders)
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

// Add services to the container.
builder.Services.AddGovUkFrontend(options =>
{
    options.RegisterDateInputModelConverter(typeof(LocalDate), new LocalDateDateInputModelConverter());
    options.RegisterDateInputModelConverter(typeof(YearMonth), new YearMonthDateInputModelConverter());
    options.ErrorSummaryGeneration = ErrorSummaryGenerationOptions.PrependToFormElements;
    options.Rebrand = true;
});
builder.Services.AddCsp();
builder
    .Services
    .AddRazorPages(options =>
    {
        options.Conventions.AuthorizeFolder("/ManageAccounts", "ManageAccountsPolicy");
        options.Conventions.AuthorizeFolder("/ManageOrganisations", "ManageOrganisationsPolicy");
    })
    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false)
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.Add(
            new PageRouteTransformerConvention(new SlugifyRouteParameterTransformer())
        );
        options.Conventions.AuthorizeFolder("/ManageAccounts");
    });

builder.Services.AddControllersWithViews(options => { options.Filters.Add<UnauthorizedExceptionFilter>(); });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ManageAccountsPolicy", policy =>
        policy.RequireRole(nameof(RoleType.Coordinator), nameof(RoleType.Administrator)))
    .AddPolicy("ManageOrganisationsPolicy", policy =>
        policy.RequireRole(nameof(RoleType.Administrator)));

// Enable App Insights
builder.Services.AddApplicationInsightsTelemetry();

// Dependencies
builder.Services.AddValidators();
builder.Services.AddRepository();
builder.Services.AddJourneys();
builder.Services.AddClients();
builder.Services.AddServices();
builder.Services.AddMappers();

builder.Services.Configure<EmailTemplateOptions>(
    builder.Configuration.GetSection(nameof(EmailTemplateOptions))
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<EcfLinkGenerator, RoutingEcfLinkGenerator>();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddEcfAuthentication(
    builder.Configuration.GetRequiredSection(OidcConfiguration.ConfigurationKey)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (featureFlags.EnableDeveloperExceptionPage)
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/Home/Error");

if (featureFlags.EnableHttpStrictTransportSecurity)
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

app.UseCsp(csp =>
{
    var pageTemplateHelper = app.Services.GetRequiredService<PageTemplateHelper>();

    csp.ByDefaultAllow.FromSelf();

    csp.AllowScripts.FromSelf().From(pageTemplateHelper.GetCspScriptHashes()).AddNonce();

    csp.AllowStyles.FromSelf().From(pageTemplateHelper.GetCspScriptHashes()).AddNonce();

    // Ensure ASP.NET Core's auto refresh works
    // See https://github.com/dotnet/aspnetcore/issues/33068
    if (featureFlags.EnableContentSecurityPolicyWorkaround) csp.AllowConnections.ToAnywhere();
});

// Need forwarded headers if using Azure Front Door. Note, this needs to be done
// early, before authentication
if (featureFlags.EnableForwardedHeaders) app.UseForwardedHeaders();

app.UseGovUkFrontend();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
