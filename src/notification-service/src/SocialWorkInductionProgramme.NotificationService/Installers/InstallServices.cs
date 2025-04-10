using System.Diagnostics.CodeAnalysis;
using SocialWorkInductionProgramme.NotificationService.Services;
using SocialWorkInductionProgramme.NotificationService.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace SocialWorkInductionProgramme.NotificationService.Installers;

[ExcludeFromCodeCoverage]
public static class InstallServices
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<INotificationCommand, EmailNotificationCommand>();
    }
}
