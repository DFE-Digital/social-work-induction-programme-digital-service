using System;
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
            migrationBuilder.AddColumn<Guid>(
                name: "primary_coordinator_id",
                table: "organisations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_organisations_persons_primary_coordinator_id",
                table: "organisations",
                column: "primary_coordinator_id",
                principalTable: "persons",
                principalColumn: "person_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organisations_persons_primary_coordinator_id",
                table: "organisations");

            migrationBuilder.DropColumn(
                name: "primary_coordinator_id",
                table: "organisations");
        }
    }
}
