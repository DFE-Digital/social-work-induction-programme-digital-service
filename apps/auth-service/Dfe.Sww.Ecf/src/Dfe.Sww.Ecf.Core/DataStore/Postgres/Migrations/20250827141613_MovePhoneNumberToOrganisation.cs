using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MovePhoneNumberToOrganisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "persons");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "organisations",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                collation: "case_insensitive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "organisations");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "persons",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                collation: "case_insensitive");
        }
    }
}
