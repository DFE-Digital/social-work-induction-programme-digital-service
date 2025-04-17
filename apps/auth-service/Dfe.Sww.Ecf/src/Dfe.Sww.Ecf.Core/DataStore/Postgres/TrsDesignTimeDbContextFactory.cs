using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

public class TrsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<EcfDbContext>
{
    public EcfDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TrsDesignTimeDbContextFactory>(optional: true)  // Optional for CI
            .Build();

        var connectionString = configuration.GetRequiredConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<EcfDbContext>();
        EcfDbContext.ConfigureOptions(optionsBuilder, connectionString);

        return new EcfDbContext(optionsBuilder.Options);
    }
}
