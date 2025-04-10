using System.Diagnostics.CodeAnalysis;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Models;
using SocialWorkInductionProgramme.Frontend.Mappers;
using SocialWorkInductionProgramme.Frontend.Models;

namespace SocialWorkInductionProgramme.Frontend.Installers;

/// <summary>
/// Install Mapper Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallMapper
{
    /// <summary>
    /// Add Mapper Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<IModelMapper<Person, Account>, AccountMapper>();
    }
}
