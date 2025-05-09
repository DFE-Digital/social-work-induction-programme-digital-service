using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dfe.Sww.Ecf.Core.Services.NameSynonyms;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddNameSynonyms(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<INameSynonymProvider, NameSynonymProvider>();

        return builder;
    }
}
