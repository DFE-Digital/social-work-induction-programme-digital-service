using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkingToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "linking_token",
                columns: table => new
                {
                    linking_token_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expiration_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() + interval '3 days'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_linking_token", x => x.linking_token_id);
                    table.ForeignKey(
                        name: "fk_linking_token_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "linking_token");
        }
    }
}
