using System.Diagnostics.CodeAnalysis;
using SocialWorkInductionProgramme.NotificationService.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SocialWorkInductionProgramme.NotificationService.Validation;

namespace SocialWorkInductionProgramme.NotificationService.Installers;

[ExcludeFromCodeCoverage]
public static class InstallValidators
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<NotificationRequest>, NotificationRequestValidator>();
    }
}
