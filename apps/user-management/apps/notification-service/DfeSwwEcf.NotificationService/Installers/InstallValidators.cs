using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DfeSwwEcf.NotificationService.Installers;

[ExcludeFromCodeCoverage]
public static class InstallValidators
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<NotificationRequest>, NotificationRequestValidator>();
    }
}
