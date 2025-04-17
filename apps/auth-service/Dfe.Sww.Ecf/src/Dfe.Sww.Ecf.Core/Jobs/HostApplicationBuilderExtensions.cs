using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dfe.Sww.Ecf.Core.Jobs;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddBackgroundJobs(this IHostApplicationBuilder builder)
    {
        if (!builder.Environment.IsUnitTests() && !builder.Environment.IsEndToEndTests())
        {
            if (builder.Configuration.GetValue<bool>("RecurringJobs:Enabled"))
            {
                builder.Services.AddHttpClient<PopulateNameSynonymsJob>();
            }

            builder.Services.AddStartupTask(sp =>
            {
                var recurringJobManager = sp.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate<PopulateNameSynonymsJob>(
                    nameof(PopulateNameSynonymsJob),
                    job => job.Execute(CancellationToken.None),
                    Cron.Never);

                recurringJobManager.AddOrUpdate<PopulateAllPersonsSearchAttributesJob>(
                    nameof(PopulateAllPersonsSearchAttributesJob),
                    job => job.Execute(CancellationToken.None),
                    Cron.Never);

                return Task.CompletedTask;
            });
        }

        return builder;
    }
}
