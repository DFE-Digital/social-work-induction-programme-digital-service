using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExternalOrgIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_organisation_external_organisation_id",
                table: "organisations");

            migrationBuilder.AlterColumn<long>(
                name: "external_organisation_id",
                table: "organisations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "external_organisation_id",
                table: "organisations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_organisation_external_organisation_id",
                table: "organisations",
                column: "external_organisation_id");
        }
    }
}
