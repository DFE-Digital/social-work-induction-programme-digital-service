using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notify.Client;
using Notify.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace DfeSwwEcf.NotificationService.Installers;

[ExcludeFromCodeCoverage]
public static class InstallHttpClients
{
    public static void AddHttpClients(this IServiceCollection services)
    {
        // Gov Notify
        var config = services.BuildServiceProvider().GetService<IConfiguration>();
        var apiKey = config?.GetValue<string>("GovNotify:ApiKey");
        services.AddTransient<IAsyncNotificationClient>(x =>
        {
            var notificationClient = new NotificationClient(apiKey);
            return notificationClient;
        });
    }
}
