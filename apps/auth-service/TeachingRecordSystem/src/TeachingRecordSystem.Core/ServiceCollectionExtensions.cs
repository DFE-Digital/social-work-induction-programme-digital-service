using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TeachingRecordSystem.Core;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrsBaseServices(this IServiceCollection services)
    {
        return services.AddSingleton<IClock, Clock>();
    }

    public static IServiceCollection AddAccessYourTeachingQualificationsOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (!environment.IsUnitTests() && !environment.IsEndToEndTests())
        {
            services.AddOptions<AccessYourTeachingQualificationsOptions>()
                .Bind(configuration.GetSection("AccessYourTeachingQualifications"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services;
    }
}
