using Microsoft.Extensions.DependencyInjection;

namespace TeachingRecordSystem.Core;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrsBaseServices(this IServiceCollection services)
    {
        return services.AddSingleton<IClock, Clock>();
    }
}
