using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationPrimaryCoordinator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "primary_coordinator_id",
                table: "organisations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "primary_coordinator_id",
                table: "organisations");
        }
    }
}
