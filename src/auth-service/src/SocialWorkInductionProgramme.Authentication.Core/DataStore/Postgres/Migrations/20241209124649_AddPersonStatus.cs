using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWorkInductionProgramme.Authentication.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "persons",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "persons");
        }
    }
}
