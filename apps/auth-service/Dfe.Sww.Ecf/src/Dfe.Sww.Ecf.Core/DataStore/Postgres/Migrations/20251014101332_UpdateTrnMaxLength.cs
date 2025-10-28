using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrnMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "trn",
                table: "persons",
                type: "character(8)",
                fixedLength: true,
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(7)",
                oldFixedLength: true,
                oldMaxLength: 7,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "trn",
                table: "persons",
                type: "character(7)",
                fixedLength: true,
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(8)",
                oldFixedLength: true,
                oldMaxLength: 8,
                oldNullable: true);
        }
    }
}
