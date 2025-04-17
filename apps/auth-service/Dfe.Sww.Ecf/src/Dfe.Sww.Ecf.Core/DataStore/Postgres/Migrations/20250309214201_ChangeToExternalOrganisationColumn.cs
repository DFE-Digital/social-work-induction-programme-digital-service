using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToExternalOrganisationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "moodle_course_id",
                table: "organisations",
                newName: "external_organisation_id");

            migrationBuilder.RenameIndex(
                name: "ix_organisation_moodle_course_id",
                table: "organisations",
                newName: "ix_organisation_external_organisation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "external_organisation_id",
                table: "organisations",
                newName: "moodle_course_id");

            migrationBuilder.RenameIndex(
                name: "ix_organisation_external_organisation_id",
                table: "organisations",
                newName: "ix_organisation_moodle_course_id");
        }
    }
}
