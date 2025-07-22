using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Installers;

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
        services.AddTransient<IRegisterSocialWorkerJourneyService, RegisterSocialWorkerJourneyService>();
        services.AddTransient<ICreateOrganisationJourneyService, CreateOrganisationJourneyService>();
        services.AddTransient<IEditOrganisationJourneyService, EditOrganisationJourneyService>();
    }
}
