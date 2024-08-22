using Dfe.Sww.Ecf.Frontend.Installers;
using Dfe.Sww.Ecf.Frontend.Routing;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGovUkFrontend();
builder
    .Services.AddRazorPages()
    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false)
    .AddRazorPagesOptions(options =>
        options.Conventions.Add(
            new PageRouteTransformerConvention(new SlugifyRouteParameterTransformer())
        )
    );

// Dependencies
builder.Services.AddValidators();
builder.Services.AddRepository();
builder.Services.AddJourneys();
builder.Services.AddClients();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<EcfLinkGenerator, RoutingEcfLinkGenerator>();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
