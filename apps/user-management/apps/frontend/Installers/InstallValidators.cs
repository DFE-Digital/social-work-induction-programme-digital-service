using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
