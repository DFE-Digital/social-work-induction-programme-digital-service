using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Installers;

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
        services.AddSingleton<IModelMapper<Person, User>, UserMapper>();
    }
}
