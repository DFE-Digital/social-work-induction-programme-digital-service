using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Installers;

[ExcludeFromCodeCoverage]
public static class InstallValidators
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Account>, AccountValidator>();
    }
}
