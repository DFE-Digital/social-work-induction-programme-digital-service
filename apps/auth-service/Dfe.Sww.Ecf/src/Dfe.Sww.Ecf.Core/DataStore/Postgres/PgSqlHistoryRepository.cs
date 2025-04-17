using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

#pragma warning disable EF1001 // Intentionally extending internal class
internal class SnakeCaseNpgsqlHistoryRepository : NpgsqlHistoryRepository
{
    public SnakeCaseNpgsqlHistoryRepository(HistoryRepositoryDependencies dependencies)
        : base(dependencies) { }
#pragma warning restore EF1001
    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
    {
        base.ConfigureTable(history);
        history.Property(h => h.MigrationId).HasColumnName("migration_id");
        history.Property(h => h.ProductVersion).HasColumnName("product_version");
    }
}
