using System.Diagnostics.CodeAnalysis;
using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.Journeys;
using SocialWorkInductionProgramme.Frontend.Services.Journeys.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Installers;

/// <summary>
/// Install Journey Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallJourneys
{
    /// <summary>
    /// Add Journey Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddJourneys(this IServiceCollection services)
    {
        services.AddTransient<ICreateAccountJourneyService, CreateAccountJourneyService>();
        services.AddTransient<IEditAccountJourneyService, EditAccountJourneyService>();
    }
}
