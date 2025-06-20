using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddEcswRegistrationQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "disability",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ethnic_group",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ethnic_group_asian",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ethnic_group_black",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ethnic_group_mixed",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ethnic_group_other",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ethnic_group_white",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "gender_matches_sex_at_birth",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "highest_qualification",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "other_ethnic_group_asian",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<string>(
                name: "other_ethnic_group_black",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<string>(
                name: "other_ethnic_group_mixed",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<string>(
                name: "other_ethnic_group_other",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<string>(
                name: "other_ethnic_group_white",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<string>(
                name: "other_gender_identity",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<string>(
                name: "other_route_into_social_work",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<int>(
                name: "route_into_social_work",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "social_work_england_registration_date",
                table: "persons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "social_work_qualification_end_year",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_sex",
                table: "persons",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "disability",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "ethnic_group",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "ethnic_group_asian",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "ethnic_group_black",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "ethnic_group_mixed",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "ethnic_group_other",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "ethnic_group_white",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "gender_matches_sex_at_birth",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "highest_qualification",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "other_ethnic_group_asian",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "other_ethnic_group_black",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "other_ethnic_group_mixed",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "other_ethnic_group_other",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "other_ethnic_group_white",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "other_gender_identity",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "other_route_into_social_work",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "route_into_social_work",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "social_work_england_registration_date",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "social_work_qualification_end_year",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "user_sex",
                table: "persons");
        }
    }
}
