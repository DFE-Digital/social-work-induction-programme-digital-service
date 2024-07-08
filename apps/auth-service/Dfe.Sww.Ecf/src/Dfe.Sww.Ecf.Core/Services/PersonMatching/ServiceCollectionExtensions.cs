using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Sww.Ecf.Core.Services.PersonMatching;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersonMatching(this IServiceCollection services)
    {
        services.AddTransient<IPersonMatchingService, PersonMatchingService>();
        return services;
    }
}
