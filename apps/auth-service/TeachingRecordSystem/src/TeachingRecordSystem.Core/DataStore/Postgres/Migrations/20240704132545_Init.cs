using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TeachingRecordSystem.Core.DataStore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("create collation case_insensitive (provider = icu, locale = 'und-u-ks-level2', deterministic = false);");

            migrationBuilder.CreateTable(
                name: "establishments",
                columns: table => new
                {
                    establishment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    urn = table.Column<int>(type: "integer", fixedLength: true, maxLength: 6, nullable: true),
                    la_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    la_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "case_insensitive"),
                    establishment_number = table.Column<string>(type: "character(4)", fixedLength: true, maxLength: 4, nullable: true),
                    establishment_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, collation: "case_insensitive"),
                    establishment_type_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    establishment_type_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    establishment_type_group_code = table.Column<int>(type: "integer", nullable: true),
                    establishment_type_group_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    establishment_status_code = table.Column<int>(type: "integer", nullable: true),
                    establishment_status_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    locality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    address3 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    town = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    county = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    postcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, collation: "case_insensitive")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_establishments", x => x.establishment_id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inserted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    published = table.Column<bool>(type: "boolean", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "journey_states",
                columns: table => new
                {
                    instance_id = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    user_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journey_states", x => x.instance_id);
                });

            migrationBuilder.CreateTable(
                name: "name_synonyms",
                columns: table => new
                {
                    name_synonyms_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_insensitive"),
                    synonyms = table.Column<string[]>(type: "text[]", nullable: false, collation: "case_insensitive")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_name_synonyms", x => x.name_synonyms_id);
                });

            migrationBuilder.CreateTable(
                name: "oidc_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    client_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    client_secret = table.Column<string>(type: "text", nullable: true),
                    client_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    consent_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    json_web_key_set = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirect_uris = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    settings = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_applications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "oidc_scopes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_scopes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person_search_attributes",
                columns: table => new
                {
                    person_search_attribute_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "case_insensitive"),
                    attribute_value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false, collation: "case_insensitive"),
                    tags = table.Column<string[]>(type: "text[]", nullable: false),
                    attribute_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "case_insensitive")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_search_attributes", x => x.person_search_attribute_id);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    trn = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_insensitive"),
                    middle_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_insensitive"),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_insensitive"),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    email_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    national_insurance_number = table.Column<string>(type: "character(9)", fixedLength: true, maxLength: 9, nullable: true),
                    dqt_first_sync = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dqt_last_sync = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dqt_created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dqt_modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dqt_first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    dqt_middle_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive"),
                    dqt_last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_insensitive")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_persons", x => x.person_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    user_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    api_roles = table.Column<string[]>(type: "varchar[]", nullable: true),
                    is_oidc_client = table.Column<bool>(type: "boolean", nullable: true),
                    client_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    client_secret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    redirect_uris = table.Column<List<string>>(type: "varchar[]", nullable: true),
                    post_logout_redirect_uris = table.Column<List<string>>(type: "varchar[]", nullable: true),
                    one_login_client_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    one_login_private_key_pem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    one_login_authentication_scheme_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    one_login_redirect_uri_path = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    one_login_post_logout_redirect_uri_path = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, collation: "case_insensitive"),
                    azure_ad_user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    roles = table.Column<string[]>(type: "varchar[]", nullable: true),
                    dqt_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "oidc_authorizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_oidc_authorizations_oidc_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "oidc_applications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "one_login_users",
                columns: table => new
                {
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    first_one_login_sign_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_one_login_sign_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    first_sign_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_sign_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    verification_route = table.Column<int>(type: "integer", nullable: true),
                    verified_names = table.Column<string>(type: "jsonb", nullable: true),
                    verified_dates_of_birth = table.Column<string>(type: "jsonb", nullable: true),
                    last_core_identity_vc = table.Column<string>(type: "jsonb", nullable: true),
                    match_route = table.Column<int>(type: "integer", nullable: false),
                    matched_attributes = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_one_login_users", x => x.subject);
                    table.ForeignKey(
                        name: "fk_one_login_users_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "person_id");
                });

            migrationBuilder.CreateTable(
                name: "person_employments",
                columns: table => new
                {
                    person_employment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    establishment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    last_known_employed_date = table.Column<DateOnly>(type: "date", nullable: false),
                    last_extract_date = table.Column<DateOnly>(type: "date", nullable: false),
                    employment_type = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    national_insurance_number = table.Column<string>(type: "character(9)", fixedLength: true, maxLength: 9, nullable: true),
                    person_postcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    api_key_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    application_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_keys", x => x.api_key_id);
                    table.ForeignKey(
                        name: "fk_api_key_application_user",
                        column: x => x.application_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "oidc_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    authorization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemption_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oidc_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_oidc_tokens_oidc_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "oidc_applications",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_oidc_tokens_oidc_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "oidc_authorizations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "support_tasks",
                columns: table => new
                {
                    support_task_reference = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    support_task_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    one_login_user_subject = table.Column<string>(type: "character varying(255)", nullable: true),
                    person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    data = table.Column<JsonDocument>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_support_tasks", x => x.support_task_reference);
                    table.ForeignKey(
                        name: "fk_support_tasks_one_login_user",
                        column: x => x.one_login_user_subject,
                        principalTable: "one_login_users",
                        principalColumn: "subject");
                    table.ForeignKey(
                        name: "fk_support_tasks_person",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "person_id");
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "active", "name", "user_type" },
                values: new object[] { new Guid("a81394d1-a498-46d8-af3e-e077596ab303"), true, "System", 3 });

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_application_user_id",
                table: "api_keys",
                column: "application_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_key",
                table: "api_keys",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_establishment_la_code_establishment_number",
                table: "establishments",
                columns: new[] { "la_code", "establishment_number" });

            migrationBuilder.CreateIndex(
                name: "ix_establishment_urn",
                table: "establishments",
                column: "urn");

            migrationBuilder.CreateIndex(
                name: "ix_events_key",
                table: "events",
                column: "key",
                unique: true,
                filter: "key is not null");

            migrationBuilder.CreateIndex(
                name: "ix_events_payload",
                table: "events",
                column: "payload")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "ix_events_person_id_event_name",
                table: "events",
                columns: new[] { "person_id", "event_name" },
                filter: "person_id is not null")
                .Annotation("Npgsql:IndexInclude", new[] { "payload" });

            migrationBuilder.CreateIndex(
                name: "ix_name_synonyms_name",
                table: "name_synonyms",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_oidc_applications_client_id",
                table: "oidc_applications",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_oidc_authorizations_application_id_status_subject_type",
                table: "oidc_authorizations",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_oidc_scopes_name",
                table: "oidc_scopes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_oidc_tokens_application_id_status_subject_type",
                table: "oidc_tokens",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_oidc_tokens_reference_id",
                table: "oidc_tokens",
                column: "reference_id",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "ix_person_search_attributes_attribute_type_and_value",
                table: "person_search_attributes",
                columns: new[] { "attribute_type", "attribute_value" });

            migrationBuilder.CreateIndex(
                name: "ix_person_search_attributes_person_id",
                table: "person_search_attributes",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_trn",
                table: "persons",
                column: "trn",
                unique: true,
                filter: "trn is not null");

            migrationBuilder.CreateIndex(
                name: "ix_support_tasks_one_login_user_subject",
                table: "support_tasks",
                column: "one_login_user_subject");

            migrationBuilder.CreateIndex(
                name: "ix_support_tasks_person_id",
                table: "support_tasks",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_azure_ad_user_id",
                table: "users",
                column: "azure_ad_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_client_id",
                table: "users",
                column: "client_id",
                unique: true,
                filter: "client_id is not null");

            migrationBuilder.CreateIndex(
                name: "ix_users_one_login_authentication_scheme_name",
                table: "users",
                column: "one_login_authentication_scheme_name",
                unique: true,
                filter: "one_login_authentication_scheme_name is not null");


            migrationBuilder.Procedure("fn_insert_person_search_attributes_v1.sql");
            migrationBuilder.Procedure("fn_update_person_search_attributes_v3.sql");
            migrationBuilder.Procedure("fn_delete_person_search_attributes_v1.sql");
            migrationBuilder.Procedure("p_refresh_person_search_attributes_v3.sql");

            migrationBuilder.Procedure("fn_insert_person_employments_person_search_attributes_v1.sql");
            migrationBuilder.Procedure("fn_update_person_employments_person_search_attributes_v2.sql");
            migrationBuilder.Procedure("fn_delete_person_employments_person_search_attributes_v1.sql");
            migrationBuilder.Procedure("p_refresh_person_employments_person_search_attributes_v1.sql");

            migrationBuilder.Procedure("p_refresh_name_person_search_attributes_v1.sql");

            migrationBuilder.Trigger("trg_insert_person_employments_person_search_attributes_v1.sql");
            migrationBuilder.Trigger("trg_delete_person_employments_person_search_attributes_v1.sql");
            migrationBuilder.Trigger("trg_update_person_employments_person_search_attributes_v1.sql");

            migrationBuilder.Trigger("trg_insert_person_search_attributes_v1.sql");
            migrationBuilder.Trigger("trg_update_person_search_attributes_v2.sql");
            migrationBuilder.Trigger("trg_delete_person_search_attributes_v1.sql");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE fn_insert_person_search_attributes_v1");
            migrationBuilder.Sql("DROP PROCEDURE fn_update_person_search_attributes_v3");
            migrationBuilder.Sql("DROP PROCEDURE fn_delete_person_search_attributes_v1");
            migrationBuilder.Sql("DROP PROCEDURE p_refresh_person_search_attributes_v3");

            migrationBuilder.Sql("DROP PROCEDURE fn_insert_person_employments_person_search_attributes_v1");
            migrationBuilder.Sql("DROP PROCEDURE fn_update_person_employments_person_search_attributes_v2");
            migrationBuilder.Sql("DROP PROCEDURE fn_delete_person_employments_person_search_attributes_v1");
            migrationBuilder.Sql("DROP PROCEDURE p_refresh_person_employments_person_search_attributes_v1");

            migrationBuilder.Sql("DROP PROCEDURE p_refresh_name_person_search_attributes_v1");

            migrationBuilder.Sql("DROP TRIGGER trg_insert_person_employments_person_search_attributes_v1");
            migrationBuilder.Sql("DROP TRIGGER trg_delete_person_employments_person_search_attributes_v1");
            migrationBuilder.Sql("DROP TRIGGER trg_update_person_employments_person_search_attributes_v1");

            migrationBuilder.Sql("DROP TRIGGER trg_insert_person_search_attributes_v1");
            migrationBuilder.Sql("DROP TRIGGER trg_update_person_search_attributes_v2");
            migrationBuilder.Sql("DROP TRIGGER trg_delete_person_search_attributes_v1");

            migrationBuilder.DropTable(
                name: "api_keys");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "journey_states");

            migrationBuilder.DropTable(
                name: "name_synonyms");

            migrationBuilder.DropTable(
                name: "oidc_scopes");

            migrationBuilder.DropTable(
                name: "oidc_tokens");

            migrationBuilder.DropTable(
                name: "person_employments");

            migrationBuilder.DropTable(
                name: "person_search_attributes");

            migrationBuilder.DropTable(
                name: "support_tasks");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "oidc_authorizations");

            migrationBuilder.DropTable(
                name: "establishments");

            migrationBuilder.DropTable(
                name: "one_login_users");

            migrationBuilder.DropTable(
                name: "oidc_applications");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.Sql("drop collation case_insensitive;");
        }
    }
}
