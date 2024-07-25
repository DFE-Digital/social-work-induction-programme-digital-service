using System.Diagnostics.CodeAnalysis;
using DfeSwwEcf.SweApiSimulator.Services;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Installers;

[ExcludeFromCodeCoverage]
public static class InstallServices
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<ISocialWorkerService, SocialWorkerService>();
        services.AddTransient<ISocialWorkerDataService, SocialWorkerDataService>();
    }
}
