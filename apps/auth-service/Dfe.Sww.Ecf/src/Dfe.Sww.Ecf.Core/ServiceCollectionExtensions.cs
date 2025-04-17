using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Sww.Ecf.Core;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddEcfBaseServices(this IServiceCollection services)
    {
        return services.AddSingleton<IClock, Clock>();
    }
}
