using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.Installers;
using Dfe.Sww.Ecf.Frontend.Routing;
using GovUk.Frontend.AspNetCore;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGovUkFrontend();
builder.Services.AddCsp(nonceByteAmount: 32);
builder
    .Services.AddRazorPages()
    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false)
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.Add(
            new PageRouteTransformerConvention(new SlugifyRouteParameterTransformer())
        );
        options.Conventions.AuthorizeFolder("/ManageAccounts");
    });

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
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCsp(csp =>
{
    var pageTemplateHelper = app.Services.GetRequiredService<PageTemplateHelper>();

    csp.ByDefaultAllow.FromSelf();

    csp.AllowScripts.FromSelf().From(pageTemplateHelper.GetCspScriptHashes()).AddNonce();

    csp.AllowStyles.FromSelf().From(pageTemplateHelper.GetCspScriptHashes()).AddNonce();

    // Ensure ASP.NET Core's auto refresh works
    // See https://github.com/dotnet/aspnetcore/issues/33068
    if (builder.Environment.IsDevelopment())
    {
        csp.AllowConnections.ToAnywhere();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
