using System.Diagnostics.CodeAnalysis;
using DfeSwwEcf.SweApiSimulator.Models.Config;
using DfeSwwEcf.SweApiSimulator.Services;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DfeSwwEcf.SweApiSimulator.Installers;

[ExcludeFromCodeCoverage]
public static class InstallOptions
{
    public static void AddAppConfigOptions(this IServiceCollection services)
    {
        services
            .AddOptions<CsvFileOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(CsvFileOptions)).Bind(settings));
    }
}
