using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Validation;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Installers;

/// <summary>
/// Install Validator Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallValidators
{
    /// <summary>
    /// Add Validator Dependencies.
    /// </summary>
    /// <param name="services"></param>
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddUserDetailsModel>, AddUserDetailsModelValidator>();
    }
}
