using System.Diagnostics.CodeAnalysis;
using DfeSwwEcf.NotificationService.Services;
using DfeSwwEcf.NotificationService.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DfeSwwEcf.NotificationService.Installers;

[ExcludeFromCodeCoverage]
public static class InstallServices
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<INotificationCommand, EmailNotificationCommand>();
        services.AddTransient<HealthCheckFunction>();
    }
}
