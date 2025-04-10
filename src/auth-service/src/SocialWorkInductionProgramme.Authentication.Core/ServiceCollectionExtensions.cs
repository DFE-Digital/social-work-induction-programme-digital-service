using Microsoft.Extensions.DependencyInjection;

namespace SocialWorkInductionProgramme.Authentication.Core;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddEcfBaseServices(this IServiceCollection services)
    {
        return services.AddSingleton<IClock, Clock>();
    }
}
