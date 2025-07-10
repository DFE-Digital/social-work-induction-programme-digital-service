using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministratorRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_roles_role_name",
                table: "roles");

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "role_name" },
                values: new object[] { 1000, "Administrator" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_roles_role_name",
                table: "roles",
                sql: "role_name in ('EarlyCareerSocialWorker', 'Assessor', 'Coordinator', 'Administrator')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_roles_role_name",
                table: "roles");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: 1000);

            migrationBuilder.AddCheckConstraint(
                name: "ck_roles_role_name",
                table: "roles",
                sql: "role_name in ('EarlyCareerSocialWorker', 'Assessor', 'Coordinator')");
        }
    }
}
