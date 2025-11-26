using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.NameMatch;
using Dfe.Sww.Ecf.Frontend.Services.NameMatch.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Installers;

/// <summary>
///     Install Service Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallServices
{
    /// <summary>
    ///     Add Service Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<ISocialWorkEnglandService, SocialWorkEnglandService>();
        services.AddTransient<ISocialWorkerValidatorService, SocialWorkerValidatorService>();
        services.AddTransient<IOrganisationService, OrganisationService>();
        services.AddTransient<IEmailService, EmailService>();
    }
}
