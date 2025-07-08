using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using OpenIddict.EntityFrameworkCore.Models;
using User = Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.User;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

public class EcfDbContext(DbContextOptions<EcfDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();

    public DbSet<JourneyState> JourneyStates => Set<JourneyState>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Person> Persons => Set<Person>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<PersonRole> PersonRoles => Set<PersonRole>();

    public DbSet<OneLoginUser> OneLoginUsers => Set<OneLoginUser>();

    public DbSet<NameSynonyms> NameSynonyms => Set<NameSynonyms>();

    public DbSet<PersonSearchAttribute> PersonSearchAttributes => Set<PersonSearchAttribute>();

    public DbSet<Organisation> Organisations => Set<Organisation>();

    public DbSet<PersonOrganisation> PersonOrganisations => Set<PersonOrganisation>();

    public DbSet<SupportTask> SupportTasks => Set<SupportTask>();

    public static EcfDbContext Create(string connectionString, int? commandTimeout = null)
    {
        return new EcfDbContext(CreateOptions(connectionString, commandTimeout));
    }

    public static void ConfigureOptions(
        DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        int? commandTimeout = null,
        DatabaseSeedOptions? seedOptions = null
    )
    {
        optionsBuilder
            .UseNpgsql(connectionString, Options)
            .UseSnakeCaseNamingConvention()
            .ReplaceService<IHistoryRepository, SnakeCaseNpgsqlHistoryRepository>()
            .UseOpenIddict<Guid>();
        if (seedOptions != null)
        {
            optionsBuilder.UseDatabaseSeeding(seedOptions);
        }

        return;

        void Options(NpgsqlDbContextOptionsBuilder o)
        {
            o.CommandTimeout(commandTimeout);
        }
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
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcfDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (clrType.Assembly == typeof(OpenIddictEntityFrameworkCoreApplication).Assembly)
            {
                entityType.SetTableName(
                    clrType.Name.Split("`")[0] switch
                    {
                        nameof(OpenIddictEntityFrameworkCoreApplication) => "oidc_applications",
                        nameof(OpenIddictEntityFrameworkCoreAuthorization) => "oidc_authorizations",
                        nameof(OpenIddictEntityFrameworkCoreScope) => "oidc_scopes",
                        nameof(OpenIddictEntityFrameworkCoreToken) => "oidc_tokens",
                        _ => throw new NotSupportedException(
                            $"Cannot configure table name for {clrType.Name}."
                        )
                    }
                );
            }
        }

        SeedDatabase(modelBuilder);
    }

    private static void SeedDatabase(ModelBuilder modelBuilder)
    {
        var roles = Enum.GetValues(typeof(RoleType))
            .Cast<RoleType>()
            .Select(roleType => new Role { RoleId = (int)roleType, RoleName = roleType })
            .ToArray();

        // Seed roles into the Roles table
        modelBuilder.Entity<Role>().HasData(roles);
    }

    private static DbContextOptions<EcfDbContext> CreateOptions(
        string connectionString,
        int? commandTimeout
    )
    {
        var optionsBuilder = new DbContextOptionsBuilder<EcfDbContext>();
        ConfigureOptions(optionsBuilder, connectionString, commandTimeout);
        return optionsBuilder.Options;
    }
}
