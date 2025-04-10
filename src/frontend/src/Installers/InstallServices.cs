using System.Diagnostics.CodeAnalysis;
using SocialWorkInductionProgramme.Frontend.Services;
using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.NameMatch;
using SocialWorkInductionProgramme.Frontend.Services.NameMatch.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Installers;

/// <summary>
/// Install Service Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallServices
{
    /// <summary>
    /// Add Service Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<ISocialWorkEnglandService, SocialWorkEnglandService>();
        services.AddTransient<ISocialWorkerValidatorService, SocialWorkerValidatorService>();
    }
}
