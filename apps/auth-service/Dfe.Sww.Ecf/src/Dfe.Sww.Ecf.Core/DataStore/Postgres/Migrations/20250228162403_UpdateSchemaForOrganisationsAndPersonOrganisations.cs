using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaForOrganisationsAndPersonOrganisations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "person_employments");

            migrationBuilder.DropTable(
                name: "establishments");

            migrationBuilder.CreateTable(
                name: "organisations",
                columns: table => new
                {
                    organisation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    moodle_course_id = table.Column<long>(type: "bigint", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organisations", x => x.organisation_id);
                });

            migrationBuilder.CreateTable(
                name: "person_organisations",
                columns: table => new
                {
                    person_organisation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_organisations", x => x.person_organisation_id);
                    table.ForeignKey(
                        name: "fk_person_organisations_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "organisations",
                        principalColumn: "organisation_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_organisations_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_organisation_moodle_course_id",
                table: "organisations",
                column: "moodle_course_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_organisation_organisation_id",
                table: "person_organisations",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_organisation_person_id",
                table: "person_organisations",
                column: "person_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "person_organisations");

            migrationBuilder.DropTable(
                name: "organisations");

            migrationBuilder.CreateTable(
                name: "establishments",
                columns: table => new
                {
                    establishment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address3 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    county = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    establishment_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, collation: "case_insensitive"),
                    establishment_number = table.Column<string>(type: "character(4)", fixedLength: true, maxLength: 4, nullable: true),
                    establishment_status_code = table.Column<int>(type: "integer", nullable: true),
                    establishment_status_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    establishment_type_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    establishment_type_group_code = table.Column<int>(type: "integer", nullable: true),
                    establishment_type_group_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    establishment_type_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    la_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    la_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "case_insensitive"),
                    locality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    postcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, collation: "case_insensitive"),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    town = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    urn = table.Column<int>(type: "integer", fixedLength: true, maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_establishments", x => x.establishment_id);
                });

            migrationBuilder.CreateTable(
                name: "person_employments",
                columns: table => new
                {
                    person_employment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    employment_type = table.Column<int>(type: "integer", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    establishment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_extract_date = table.Column<DateOnly>(type: "date", nullable: false),
                    last_known_employed_date = table.Column<DateOnly>(type: "date", nullable: false),
                    national_insurance_number = table.Column<string>(type: "character(9)", fixedLength: true, maxLength: 9, nullable: true),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_postcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_employments", x => x.person_employment_id);
                    table.ForeignKey(
                        name: "fk_person_employments_establishment_id",
                        column: x => x.establishment_id,
                        principalTable: "establishments",
                        principalColumn: "establishment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_employments_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_establishment_la_code_establishment_number",
                table: "establishments",
                columns: new[] { "la_code", "establishment_number" });

            migrationBuilder.CreateIndex(
                name: "ix_establishment_urn",
                table: "establishments",
                column: "urn");

            migrationBuilder.CreateIndex(
                name: "ix_person_employments_establishment_id",
                table: "person_employments",
                column: "establishment_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_employments_key",
                table: "person_employments",
                column: "key");

            migrationBuilder.CreateIndex(
                name: "ix_person_employments_person_id",
                table: "person_employments",
                column: "person_id");
        }
    }
}
