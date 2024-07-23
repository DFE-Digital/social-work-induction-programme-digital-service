using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Establishment = Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Establishment;
using User = Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.User;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

public class EcfDbContext : DbContext
{
    public EcfDbContext(DbContextOptions<EcfDbContext> options)
        : base(options)
    {
    }

    public static EcfDbContext Create(string connectionString, int? commandTimeout = null) =>
        new EcfDbContext(CreateOptions(connectionString, commandTimeout));

    public DbSet<Event> Events => Set<Event>();

    public DbSet<JourneyState> JourneyStates => Set<JourneyState>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Person> Persons => Set<Person>();

    public DbSet<OneLoginUser> OneLoginUsers => Set<OneLoginUser>();

    public DbSet<NameSynonyms> NameSynonyms => Set<NameSynonyms>();

    public DbSet<PersonSearchAttribute> PersonSearchAttributes => Set<PersonSearchAttribute>();

    public DbSet<Establishment> Establishments => Set<Establishment>();

    public DbSet<PersonEmployment> PersonEmployments => Set<PersonEmployment>();

    public DbSet<SupportTask> SupportTasks => Set<SupportTask>();

    public static void ConfigureOptions(DbContextOptionsBuilder optionsBuilder, string connectionString,
        int? commandTimeout = null)
    {
        optionsBuilder
            .UseNpgsql(connectionString, Options)
            .UseSnakeCaseNamingConvention();
        return;

        void Options(NpgsqlDbContextOptionsBuilder o) => o.CommandTimeout(commandTimeout);
    }

    public void AddEvent(EventBase @event, DateTime? inserted = null)
    {
        Events.Add(Event.FromEventBase(@event, inserted));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove<ForeignKeyIndexConvention>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcfDbContext).Assembly);
    }

    private static DbContextOptions<EcfDbContext> CreateOptions(string connectionString, int? commandTimeout)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EcfDbContext>();
        ConfigureOptions(optionsBuilder, connectionString, commandTimeout);
        return optionsBuilder.Options;
    }
}
