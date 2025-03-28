using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class CleanupRedundantAuthCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "api_keys");

            migrationBuilder.DropTable(name: "oidc_scopes");

            migrationBuilder.DropTable(name: "oidc_tokens");

            migrationBuilder.DropTable(name: "oidc_authorizations");

            migrationBuilder.DropTable(name: "oidc_applications");

            migrationBuilder.DropIndex(name: "ix_users_azure_ad_user_id", table: "users");

            migrationBuilder.DropIndex(name: "ix_users_client_id", table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_one_login_authentication_scheme_name",
                table: "users"
            );

            migrationBuilder.DropColumn(name: "api_roles", table: "users");

            migrationBuilder.DropColumn(name: "azure_ad_user_id", table: "users");

            migrationBuilder.DropColumn(name: "client_id", table: "users");

            migrationBuilder.DropColumn(name: "client_secret", table: "users");

            migrationBuilder.DropColumn(name: "dqt_user_id", table: "users");

            migrationBuilder.DropColumn(name: "is_oidc_client", table: "users");

            migrationBuilder.DropColumn(
                name: "one_login_authentication_scheme_name",
                table: "users"
            );

            migrationBuilder.DropColumn(name: "one_login_client_id", table: "users");

            migrationBuilder.DropColumn(
                name: "one_login_post_logout_redirect_uri_path",
                table: "users"
            );

            migrationBuilder.DropColumn(name: "one_login_private_key_pem", table: "users");

            migrationBuilder.DropColumn(name: "one_login_redirect_uri_path", table: "users");

            migrationBuilder.DropColumn(name: "post_logout_redirect_uris", table: "users");

            migrationBuilder.DropColumn(name: "redirect_uris", table: "users");

            migrationBuilder.DropColumn(name: "dqt_created_on", table: "persons");

            migrationBuilder.DropColumn(name: "dqt_first_name", table: "persons");

            migrationBuilder.DropColumn(name: "dqt_first_sync", table: "persons");

            migrationBuilder.DropColumn(name: "dqt_last_name", table: "persons");

            migrationBuilder.DropColumn(name: "dqt_last_sync", table: "persons");

            migrationBuilder.DropColumn(name: "dqt_middle_name", table: "persons");

            migrationBuilder.DropColumn(name: "dqt_modified_on", table: "persons");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("a81394d1-a498-46d8-af3e-e077596ab303"),
                column: "user_type",
                value: 2
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("a81394d1-a498-46d8-af3e-e077596ab303")
            );

            migrationBuilder.AddColumn<string[]>(
                name: "api_roles",
                table: "users",
                type: "varchar[]",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "azure_ad_user_id",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "client_id",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "client_secret",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true
            );

            migrationBuilder.AddColumn<Guid>(
                name: "dqt_user_id",
                table: "users",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_oidc_client",
                table: "users",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "one_login_authentication_scheme_name",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "one_login_client_id",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "one_login_post_logout_redirect_uri_path",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "one_login_private_key_pem",
                table: "users",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "one_login_redirect_uri_path",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.AddColumn<List<string>>(
                name: "post_logout_redirect_uris",
                table: "users",
                type: "varchar[]",
                nullable: true
            );

            migrationBuilder.AddColumn<List<string>>(
                name: "redirect_uris",
                table: "users",
                type: "varchar[]",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "dqt_created_on",
                table: "persons",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "dqt_first_name",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive"
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "dqt_first_sync",
                table: "persons",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "dqt_last_name",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive"
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "dqt_last_sync",
                table: "persons",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "dqt_middle_name",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                collation: "case_insensitive"
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "dqt_modified_on",
                table: "persons",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    api_key_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    expires = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    key = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    updated_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_keys", x => x.api_key_id);
                    table.ForeignKey(
                        name: "fk_api_key_application_user",
                        column: x => x.application_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "oidc_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_type = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    client_id = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    client_secret = table.Column<string>(type: "text", nullable: true),
                    client_type = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    concurrency_token = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    consent_type = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    json_web_key_set = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirect_uris = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    settings = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_applications", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "oidc_scopes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    concurrency_token = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_scopes", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "oidc_authorizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    concurrency_token = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    creation_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    subject = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    type = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_oidc_authorizations_oidc_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "oidc_applications",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "oidc_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    authorization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    concurrency_token = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    creation_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    expiration_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemption_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    reference_id = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    status = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    subject = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: true
                    ),
                    type = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_oidc_tokens_oidc_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "oidc_applications",
                        principalColumn: "id"
                    );
                    table.ForeignKey(
                        name: "fk_oidc_tokens_oidc_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "oidc_authorizations",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "active", "name", "user_type" },
                values: new object[]
                {
                    new Guid("a81394d1-a498-46d8-af3e-e077596ab303"),
                    true,
                    "System",
                    3,
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_azure_ad_user_id",
                table: "users",
                column: "azure_ad_user_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_client_id",
                table: "users",
                column: "client_id",
                unique: true,
                filter: "client_id is not null"
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_one_login_authentication_scheme_name",
                table: "users",
                column: "one_login_authentication_scheme_name",
                unique: true,
                filter: "one_login_authentication_scheme_name is not null"
            );

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_application_user_id",
                table: "api_keys",
                column: "application_user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_key",
                table: "api_keys",
                column: "key",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_oidc_applications_client_id",
                table: "oidc_applications",
                column: "client_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_oidc_authorizations_application_id_status_subject_type",
                table: "oidc_authorizations",
                columns: new[] { "application_id", "status", "subject", "type" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_oidc_scopes_name",
                table: "oidc_scopes",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_oidc_tokens_application_id_status_subject_type",
                table: "oidc_tokens",
                columns: new[] { "application_id", "status", "subject", "type" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_oidc_tokens_reference_id",
                table: "oidc_tokens",
                column: "reference_id",
                unique: true
            );
        }
    }
}
