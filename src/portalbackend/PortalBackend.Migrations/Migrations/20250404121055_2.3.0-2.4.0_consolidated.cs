using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Org.Eclipse.TractusX.Portal.Backend.PortalBackend.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class _230240_consolidated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region 20240919142711_2.3.0-alpha.1
            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() CASCADE;");

            migrationBuilder.DropForeignKey(
                name: "fk_offers_companies_provider_company_id",
                schema: "portal",
                table: "offers");

            migrationBuilder.Sql("DELETE from portal.process_steps WHERE process_step_type_id = 15");

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.Sql("DELETE from portal.process_steps WHERE process_step_type_id = 503");

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 503);

            // migrationBuilder.DropColumn(
            //     name: "provider",
            //     schema: "portal",
            //     table: "offers");

            // migrationBuilder.DropColumn(
            //     name: "organisation_name",
            //     schema: "portal",
            //     table: "company_invitations");

            migrationBuilder.AlterColumn<Guid>(
                name: "provider_company_id",
                schema: "portal",
                table: "offers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "application_id",
                schema: "portal",
                table: "company_invitations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            // migrationBuilder.CreateTable(
            //     name: "audit_offer20240911",
            //     schema: "portal",
            //     columns: table => new
            //     {
            //         audit_v1id = table.Column<Guid>(type: "uuid", nullable: false),
            //         id = table.Column<Guid>(type: "uuid", nullable: false),
            //         name = table.Column<string>(type: "text", nullable: true),
            //         date_created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            //         date_released = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            //         marketing_url = table.Column<string>(type: "text", nullable: true),
            //         contact_email = table.Column<string>(type: "text", nullable: true),
            //         contact_number = table.Column<string>(type: "text", nullable: true),
            //         offer_type_id = table.Column<int>(type: "integer", nullable: true),
            //         sales_manager_id = table.Column<Guid>(type: "uuid", nullable: true),
            //         provider_company_id = table.Column<Guid>(type: "uuid", nullable: true),
            //         offer_status_id = table.Column<int>(type: "integer", nullable: true),
            //         license_type_id = table.Column<int>(type: "integer", nullable: true),
            //         date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            //         last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
            //         audit_v1last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
            //         audit_v1operation_id = table.Column<int>(type: "integer", nullable: false),
            //         audit_v1date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("pk_audit_offer20240911", x => x.audit_v1id);
            //     });

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 1,
                column: "label",
                value: "MANUAL_VERIFY_REGISTRATION");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 9,
                column: "label",
                value: "AWAIT_CLEARING_HOUSE_RESPONSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 16,
                column: "label",
                value: "MANUAL_TRIGGER_OVERRIDE_CLEARING_HOUSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 18,
                column: "label",
                value: "AWAIT_SELF_DESCRIPTION_LP_RESPONSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 19,
                column: "label",
                value: "MANUAL_DECLINE_APPLICATION");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 21,
                column: "label",
                value: "AWAIT_DIM_RESPONSE_RESPONSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 26,
                column: "label",
                value: "AWAIT_BPN_CREDENTIAL_RESPONSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 28,
                column: "label",
                value: "AWAIT_MEMBERSHIP_CREDENTIAL_RESPONSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 101,
                column: "label",
                value: "AWAIT_START_AUTOSETUP");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 111,
                column: "label",
                value: "MANUAL_TRIGGER_ACTIVATE_SUBSCRIPTION");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 505,
                column: "label",
                value: "AWAIT_DELETE_DIM_TECHNICAL_USER_RESPONSE");

            migrationBuilder.InsertData(
                schema: "portal",
                table: "process_step_types",
                columns: new[] { "id", "label" },
                values: new object[,]
                {
                    { 31, "RETRIGGER_REQUEST_BPN_CREDENTIAL" },
                    { 32, "RETRIGGER_REQUEST_MEMBERSHIP_CREDENTIAL" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_offers_companies_provider_company_id",
                schema: "portal",
                table: "offers",
                column: "provider_company_id",
                principalSchema: "portal",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20240911\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_OFFER AFTER INSERT\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20240911\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_OFFER AFTER UPDATE\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"();");
            #endregion

            #region 20241021154921_2.3.0-alpha.3
            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() CASCADE;");

            // drop foreign keys AppInstanceAssignedCompanyServiceAccount

            migrationBuilder.DropForeignKey(
                name: "fk_app_instance_assigned_service_accounts_app_instances_app_in",
                schema: "portal",
                table: "app_instance_assigned_service_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_app_instance_assigned_service_accounts_company_service_acco",
                schema: "portal",
                table: "app_instance_assigned_service_accounts");

            // drop foreign keys CompanyServiceAccount

            migrationBuilder.DropForeignKey(
                name: "fk_company_service_accounts_company_service_account_kindes_com",
                schema: "portal",
                table: "company_service_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_company_service_accounts_company_service_account_types_comp",
                schema: "portal",
                table: "company_service_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_company_service_accounts_identities_id",
                schema: "portal",
                table: "company_service_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_company_service_accounts_offer_subscriptions_offer_subscrip",
                schema: "portal",
                table: "company_service_accounts");

            // drop foreign keys Connector

            migrationBuilder.DropForeignKey(
                name: "fk_connectors_company_service_accounts_company_service_account",
                schema: "portal",
                table: "connectors");

            // drop foreign keys DimCompanyServiceAccount

            migrationBuilder.DropForeignKey(
                name: "fk_dim_company_service_accounts_company_service_accounts_id",
                schema: "portal",
                table: "dim_company_service_accounts");

            // drop foreign keys DimUserCreationData

            migrationBuilder.DropForeignKey(
                name: "fk_dim_user_creation_data_processes_process_id",
                schema: "portal",
                table: "dim_user_creation_data");

            migrationBuilder.DropForeignKey(
                name: "fk_dim_user_creation_data_company_service_accounts_service_acc",
                schema: "portal",
                table: "dim_user_creation_data");

            // as company_linked_technical_users is a view the autocreated constraint ain't work
            // the respective navigational property nevertheless works without.
            // the autocreated statement is left here for documentation purpose.

            // migrationBuilder.DropForeignKey(
            //     name: "fk_company_linked_service_accounts_company_service_accounts_co",
            //     schema: "portal",
            //     table: "company_linked_service_accounts");

            // AppInstanceAssignedCompanyServiceAccount -> AppInstanceAssignedTechnicalUser

            migrationBuilder.DropPrimaryKey(
                name: "pk_app_instance_assigned_service_accounts",
                table: "app_instance_assigned_service_accounts",
                schema: "portal");

            migrationBuilder.DropIndex(
                name: "ix_app_instance_assigned_service_accounts_company_service_acco",
                table: "app_instance_assigned_service_accounts",
                schema: "portal");

            migrationBuilder.RenameTable(
                name: "app_instance_assigned_service_accounts",
                schema: "portal",
                newName: "app_instance_assigned_technical_users");

            migrationBuilder.RenameColumn(
                name: "company_service_account_id",
                table: "app_instance_assigned_technical_users",
                schema: "portal",
                newName: "technical_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_app_instance_assigned_technical_users",
                table: "app_instance_assigned_technical_users",
                schema: "portal",
                columns: ["app_instance_id", "technical_user_id"]);

            migrationBuilder.CreateIndex(
                name: "ix_app_instance_assigned_technical_users_technical_user_id",
                table: "app_instance_assigned_technical_users",
                schema: "portal",
                column: "technical_user_id");

            // CompanyServiceAccount -> TechnicalUser

            migrationBuilder.DropIndex(
                name: "ix_company_service_accounts_client_client_id",
                table: "company_service_accounts",
                schema: "portal");

            migrationBuilder.DropIndex(
                name: "ix_company_service_accounts_company_service_account_kind_id",
                table: "company_service_accounts",
                schema: "portal");

            migrationBuilder.DropIndex(
                name: "ix_company_service_accounts_company_service_account_type_id",
                table: "company_service_accounts",
                schema: "portal");

            migrationBuilder.RenameTable(
                name: "company_service_accounts",
                schema: "portal",
                newName: "technical_users");

            migrationBuilder.RenameColumn(
                name: "company_service_account_kind_id",
                table: "technical_users",
                schema: "portal",
                newName: "technical_user_kind_id");

            migrationBuilder.RenameColumn(
                name: "company_service_account_type_id",
                table: "technical_users",
                schema: "portal",
                newName: "technical_user_type_id");

            migrationBuilder.RenameIndex(
                name: "pk_company_service_accounts",
                table: "technical_users",
                schema: "portal",
                newName: "pk_technical_users");

            migrationBuilder.CreateIndex(
                name: "ix_technical_users_client_client_id",
                table: "technical_users",
                schema: "portal",
                column: "client_client_id",
                filter: "client_client_id is not null AND technical_user_kind_id = 1");

            migrationBuilder.RenameIndex(
                name: "ix_company_service_accounts_offer_subscription_id",
                table: "technical_users",
                schema: "portal",
                newName: "ix_technical_users_offer_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_technical_users_technical_user_kind_id",
                table: "technical_users",
                schema: "portal",
                column: "technical_user_kind_id");

            migrationBuilder.CreateIndex(
                name: "ix_technical_users_technical_user_type_id",
                table: "technical_users",
                schema: "portal",
                column: "technical_user_type_id");

            // CompanyServiceAccountKind -> TechnicalUserKind

            migrationBuilder.RenameTable(
                name: "company_service_account_kindes",
                schema: "portal",
                newName: "technical_user_kinds");

            migrationBuilder.RenameIndex(
                name: "pk_company_service_account_kindes",
                table: "technical_user_kinds",
                schema: "portal",
                newName: "pk_technical_user_kinds");

            // CompanyServiceAccountType -> TechnicalUserType

            migrationBuilder.RenameTable(
                name: "company_service_account_types",
                schema: "portal",
                newName: "technical_user_types");

            migrationBuilder.RenameIndex(
                name: "pk_company_service_account_types",
                table: "technical_user_types",
                schema: "portal",
                newName: "pk_technical_user_types");

            // Connector

            migrationBuilder.DropIndex(
                name: "ix_connectors_company_service_account_id",
                table: "connectors",
                schema: "portal");

            migrationBuilder.RenameColumn(
                name: "company_service_account_id",
                schema: "portal",
                table: "connectors",
                newName: "technical_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_connectors_technical_user_id",
                table: "connectors",
                schema: "portal",
                column: "technical_user_id");

            // DimCompanyServiceAccount -> ExternalTechnicalUser

            migrationBuilder.RenameTable(
                name: "dim_company_service_accounts",
                schema: "portal",
                newName: "external_technical_users");

            migrationBuilder.RenameIndex(
                name: "pk_dim_company_service_accounts",
                table: "external_technical_users",
                schema: "portal",
                newName: "pk_external_technical_users");

            // DimUserCreationData -> ExternalTechnicalUserCreationData

            migrationBuilder.DropIndex(
                name: "ix_dim_user_creation_data_service_account_id",
                table: "dim_user_creation_data",
                schema: "portal");

            migrationBuilder.RenameTable(
                name: "dim_user_creation_data",
                schema: "portal",
                newName: "external_technical_user_creation_data");

            migrationBuilder.RenameColumn(
                name: "service_account_id",
                table: "external_technical_user_creation_data",
                schema: "portal",
                newName: "technical_user_id");

            migrationBuilder.RenameIndex(
                name: "pk_dim_user_creation_data",
                table: "external_technical_user_creation_data",
                schema: "portal",
                newName: "pk_external_technical_user_creation_data");

            migrationBuilder.RenameIndex(
                name: "ix_dim_user_creation_data_process_id",
                table: "external_technical_user_creation_data",
                schema: "portal",
                newName: "ix_external_technical_user_creation_data_process_id");

            migrationBuilder.CreateIndex(
                name: "ix_external_technical_user_creation_data_technical_user_id",
                table: "external_technical_user_creation_data",
                schema: "portal",
                column: "technical_user_id");

            // CompaniesLinkedServiceAccount -> CompaniesLinkedTechnicalUser

            migrationBuilder.Sql(@"DROP VIEW IF EXISTS portal.company_linked_service_accounts");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW portal.company_linked_technical_users AS
                SELECT
                    tu.id AS technical_user_id,
                    i.company_id AS owners,
                    CASE
                        WHEN tu.offer_subscription_id IS NOT NULL THEN o.provider_company_id
                        WHEN EXISTS (SELECT 1 FROM portal.connectors cs WHERE cs.technical_user_id = tu.id) THEN c.host_id
                        END AS provider
                FROM portal.technical_users tu
                    JOIN portal.identities i ON tu.id = i.id
                    LEFT JOIN portal.offer_subscriptions os ON tu.offer_subscription_id = os.id
                    LEFT JOIN portal.offers o ON os.offer_id = o.id
                    LEFT JOIN portal.connectors c ON tu.id = c.technical_user_id
                WHERE tu.technical_user_type_id = 1 AND i.identity_type_id = 2
                UNION
                SELECT
                    tu.id AS technical_user_id,
                    i.company_id AS owners,
                    null AS provider
                FROM
                    portal.technical_users tu
                        JOIN portal.identities i ON tu.id = i.id
                WHERE tu.technical_user_type_id = 2
                ");

            // re-add foreign keys AppInstanceAssignedTechnicalUser (AppInstanceAssignedCompanyServiceAccount)

            migrationBuilder.AddForeignKey(
                name: "fk_app_instance_assigned_technical_users_app_instances_app_ins",
                table: "app_instance_assigned_technical_users",
                column: "app_instance_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "app_instances",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_app_instance_assigned_technical_users_technical_users_techn",
                table: "app_instance_assigned_technical_users",
                column: "technical_user_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "technical_users",
                principalColumn: "id");

            // re-add foreign keys TechnicalUser (CompanyServiceAccount)

            migrationBuilder.AddForeignKey(
                name: "fk_technical_users_identities_id",
                table: "technical_users",
                column: "id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "identities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_technical_users_offer_subscriptions_offer_subscription_id",
                table: "technical_users",
                column: "offer_subscription_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "offer_subscriptions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
               name: "fk_technical_users_technical_user_kinds_technical_user_kind_id",
               table: "technical_users",
               column: "technical_user_kind_id",
               schema: "portal",
               principalSchema: "portal",
               principalTable: "technical_user_kinds",
               principalColumn: "id",
               onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_technical_users_technical_user_types_technical_user_type_id",
                table: "technical_users",
                column: "technical_user_type_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "technical_user_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // re-add foreign keys Connector

            migrationBuilder.AddForeignKey(
                name: "fk_connectors_technical_users_technical_user_id",
                schema: "portal",
                table: "connectors",
                column: "technical_user_id",
                principalSchema: "portal",
                principalTable: "technical_users",
                principalColumn: "id");

            // re-add foreign keys ExternalTechnicalUser (DimCompanyServiceAccount)

            migrationBuilder.AddForeignKey(
               name: "fk_external_technical_users_external_technical_users_id",
               table: "external_technical_users",
               column: "id",
               schema: "portal",
               principalSchema: "portal",
               principalTable: "technical_users",
               principalColumn: "id",
               onDelete: ReferentialAction.Cascade);

            // re-add foreign keys ExternalTechnicalUserCreationData (DimUserCreationData)

            migrationBuilder.AddForeignKey(
                name: "fk_external_technical_user_creation_data_processes_process_id",
                table: "external_technical_user_creation_data",
                column: "process_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "processes",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_external_technical_user_creation_data_technical_users_techn",
                table: "external_technical_user_creation_data",
                column: "technical_user_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "technical_users",
                principalColumn: "id");

            // as company_linked_technical_users is a view the autocreated constraint ain't work
            // the respective navigational property nevertheless works without.
            // the autocreated statement is left here for documentation purpose.

            // migrationBuilder.AddForeignKey(
            //     name: "fk_company_linked_technical_users_technical_users_technical_us",
            //     table: "company_linked_technical_users",
            //     column: "technical_user_id",
            //     schema: "portal",
            //     principalSchema: "portal",
            //     principalTable: "technical_users",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.Cascade);

            // AuditConnector20241008

            migrationBuilder.CreateTable(
                name: "audit_connector20241008",
                schema: "portal",
                columns: table => new
                {
                    audit_v1id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    connector_url = table.Column<string>(type: "text", nullable: true),
                    type_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    provider_id = table.Column<Guid>(type: "uuid", nullable: true),
                    host_id = table.Column<Guid>(type: "uuid", nullable: true),
                    self_description_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location_id = table.Column<string>(type: "text", nullable: true),
                    self_description_message = table.Column<string>(type: "text", nullable: true),
                    date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    technical_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sd_creation_process_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    audit_v1last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1operation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_connector20241008", x => x.audit_v1id);
                });

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 12,
                column: "label",
                value: "START_APPLICATION_ACTIVATION");

            migrationBuilder.InsertData(
                schema: "portal",
                table: "process_step_types",
                columns: new[] { "id", "label" },
                values: new object[,]
                {
                    { 33, "ASSIGN_INITIAL_ROLES" },
                    { 34, "ASSIGN_BPN_TO_USERS" },
                    { 35, "REMOVE_REGISTRATION_ROLES" },
                    { 36, "SET_THEME" },
                    { 37, "SET_MEMBERSHIP" },
                    { 38, "FINISH_APPLICATION_ACTIVATION" },
                    { 39, "RETRIGGER_ASSIGN_INITIAL_ROLES" },
                    { 40, "RETRIGGER_ASSIGN_BPN_TO_USERS" },
                    { 41, "RETRIGGER_REMOVE_REGISTRATION_ROLES" },
                    { 42, "RETRIGGER_SET_THEME" },
                    { 43, "RETRIGGER_SET_MEMBERSHIP" }
                });

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20241008\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"technical_user_id\", \"sd_creation_process_id\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"technical_user_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_CONNECTOR AFTER INSERT\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20241008\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"technical_user_id\", \"sd_creation_process_id\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"technical_user_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_CONNECTOR AFTER UPDATE\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"();");
            #endregion

            #region 20241213081306_2.4.0-alpha.1
            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_DOCUMENT\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_DOCUMENT\"() CASCADE;");

            migrationBuilder.AddColumn<long>(
                name: "document_size",
                schema: "portal",
                table: "documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "audit_document20241120",
                schema: "portal",
                columns: table => new
                {
                    audit_v1id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    document_hash = table.Column<byte[]>(type: "bytea", nullable: true),
                    document_content = table.Column<byte[]>(type: "bytea", nullable: true),
                    document_name = table.Column<string>(type: "text", nullable: true),
                    media_type_id = table.Column<int>(type: "integer", nullable: true),
                    document_type_id = table.Column<int>(type: "integer", nullable: true),
                    document_status_id = table.Column<int>(type: "integer", nullable: true),
                    document_size = table.Column<long>(type: "bigint", nullable: true),
                    company_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    audit_v1last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1operation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_document20241120", x => x.audit_v1id);
                });

            migrationBuilder.Sql("UPDATE portal.documents SET document_size = (octet_length(document_content) / 1024)");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_DOCUMENT\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_DOCUMENT$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_document20241120\" (\"id\", \"date_created\", \"document_hash\", \"document_content\", \"document_name\", \"media_type_id\", \"document_type_id\", \"document_status_id\", \"company_user_id\", \"date_last_changed\", \"last_editor_id\", \"document_size\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"date_created\", \r\n  NEW.\"document_hash\", \r\n  NEW.\"document_content\", \r\n  NEW.\"document_name\", \r\n  NEW.\"media_type_id\", \r\n  NEW.\"document_type_id\", \r\n  NEW.\"document_status_id\", \r\n  NEW.\"company_user_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  NEW.\"document_size\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_DOCUMENT$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_DOCUMENT AFTER INSERT\r\nON \"portal\".\"documents\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_DOCUMENT\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_DOCUMENT\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_DOCUMENT$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_document20241120\" (\"id\", \"date_created\", \"document_hash\", \"document_content\", \"document_name\", \"media_type_id\", \"document_type_id\", \"document_status_id\", \"company_user_id\", \"date_last_changed\", \"last_editor_id\", \"document_size\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"date_created\", \r\n  NEW.\"document_hash\", \r\n  NEW.\"document_content\", \r\n  NEW.\"document_name\", \r\n  NEW.\"media_type_id\", \r\n  NEW.\"document_type_id\", \r\n  NEW.\"document_status_id\", \r\n  NEW.\"company_user_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  NEW.\"document_size\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_DOCUMENT$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_DOCUMENT AFTER UPDATE\r\nON \"portal\".\"documents\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_DOCUMENT\"();");

            #endregion

            #region 20250107114624_2.4.0-alpha.2

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() CASCADE;");

            migrationBuilder.AddColumn<bool>(
                name: "display_technical_user",
                schema: "portal",
                table: "offers",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "audit_offer20241219",
                schema: "portal",
                columns: table => new
                {
                    audit_v1id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    date_created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    date_released = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    marketing_url = table.Column<string>(type: "text", nullable: true),
                    contact_email = table.Column<string>(type: "text", nullable: true),
                    contact_number = table.Column<string>(type: "text", nullable: true),
                    display_technical_user = table.Column<bool>(type: "boolean", nullable: true),
                    offer_type_id = table.Column<int>(type: "integer", nullable: true),
                    sales_manager_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    offer_status_id = table.Column<int>(type: "integer", nullable: true),
                    license_type_id = table.Column<int>(type: "integer", nullable: true),
                    date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1operation_id = table.Column<int>(type: "integer", nullable: false),
                    audit_v1date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_offer20241219", x => x.audit_v1id);
                });

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20241219\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"display_technical_user\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"display_technical_user\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_OFFER AFTER INSERT\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20241219\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"display_technical_user\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"display_technical_user\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_OFFER AFTER UPDATE\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"();");
            #endregion

            #region 20250207083336_2.4.0-rc1

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() CASCADE;");

            migrationBuilder.DropForeignKey(
                name: "fk_process_steps_process_step_statuses_process_step_status_id",
                schema: "portal",
                table: "process_steps");

            migrationBuilder.DropColumn(
                name: "display_technical_user",
                schema: "portal",
                table: "offers");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "sd_skipped_date",
                schema: "portal",
                table: "connectors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "agreement_descriptions",
                schema: "portal",
                columns: table => new
                {
                    agreement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_name = table.Column<string>(type: "character(2)", maxLength: 2, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agreement_descriptions", x => new { x.agreement_id, x.language_short_name });
                    table.ForeignKey(
                        name: "fk_agreement_descriptions_agreements_agreement_id",
                        column: x => x.agreement_id,
                        principalSchema: "portal",
                        principalTable: "agreements",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_agreement_descriptions_languages_language_short_name",
                        column: x => x.language_short_name,
                        principalSchema: "portal",
                        principalTable: "languages",
                        principalColumn: "short_name");
                });

            migrationBuilder.CreateTable(
                name: "audit_connector20250113",
                schema: "portal",
                columns: table => new
                {
                    audit_v1id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    connector_url = table.Column<string>(type: "text", nullable: true),
                    type_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    provider_id = table.Column<Guid>(type: "uuid", nullable: true),
                    host_id = table.Column<Guid>(type: "uuid", nullable: true),
                    self_description_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location_id = table.Column<string>(type: "text", nullable: true),
                    self_description_message = table.Column<string>(type: "text", nullable: true),
                    date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    technical_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sd_creation_process_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sd_skipped_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    audit_v1last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1operation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_connector20250113", x => x.audit_v1id);
                });

            migrationBuilder.CreateTable(
                name: "audit_offer20250121",
                schema: "portal",
                columns: table => new
                {
                    audit_v1id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    date_created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    date_released = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    marketing_url = table.Column<string>(type: "text", nullable: true),
                    contact_email = table.Column<string>(type: "text", nullable: true),
                    contact_number = table.Column<string>(type: "text", nullable: true),
                    offer_type_id = table.Column<int>(type: "integer", nullable: true),
                    sales_manager_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    offer_status_id = table.Column<int>(type: "integer", nullable: true),
                    license_type_id = table.Column<int>(type: "integer", nullable: true),
                    date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1last_editor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_v1operation_id = table.Column<int>(type: "integer", nullable: false),
                    audit_v1date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_offer20250121", x => x.audit_v1id);
                });

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "identity_type",
                keyColumn: "id",
                keyValue: 2,
                column: "label",
                value: "TECHNICAL_USER");

            migrationBuilder.InsertData(
                schema: "portal",
                table: "notification_type",
                columns: new[] { "id", "label" },
                values: new object[,]
                {
                    { 27, "APP_SUBSCRIPTION_DECLINE" },
                    { 28, "SERVICE_SUBSCRIPTION_DECLINE" }
                });

            migrationBuilder.InsertData(
                schema: "portal",
                table: "process_step_types",
                columns: new[] { "id", "label" },
                values: new object[,]
                {
                    { 44, "SET_CX_MEMBERSHIP_IN_BPDM" },
                    { 45, "RETRIGGER_SET_CX_MEMBERSHIP_IN_BPDM" },
                    { 804, "AWAIT_SELF_DESCRIPTION_CONNECTOR_RESPONSE" },
                    { 805, "AWAIT_SELF_DESCRIPTION_COMPANY_RESPONSE" },
                    { 806, "RETRIGGER_AWAIT_SELF_DESCRIPTION_CONNECTOR_RESPONSE" },
                    { 807, "RETRIGGER_AWAIT_SELF_DESCRIPTION_COMPANY_RESPONSE" }
                });

            migrationBuilder.InsertData(
                schema: "portal",
                table: "technical_user_types",
                columns: new[] { "id", "label" },
                values: new object[] { 3, "PROVIDER_OWNED" });

            migrationBuilder.CreateIndex(
                name: "ix_agreement_descriptions_language_short_name",
                schema: "portal",
                table: "agreement_descriptions",
                column: "language_short_name");

            migrationBuilder.AddForeignKey(
                name: "fk_process_steps_process_step_statuses_process_step_status_id",
                schema: "portal",
                table: "process_steps",
                column: "process_step_status_id",
                principalSchema: "portal",
                principalTable: "process_step_statuses",
                principalColumn: "id");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20250113\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"technical_user_id\", \"sd_creation_process_id\", \"sd_skipped_date\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"technical_user_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"sd_skipped_date\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_CONNECTOR AFTER INSERT\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20250113\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"technical_user_id\", \"sd_creation_process_id\", \"sd_skipped_date\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"technical_user_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"sd_skipped_date\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_CONNECTOR AFTER UPDATE\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20250121\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_OFFER AFTER INSERT\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20250121\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_OFFER AFTER UPDATE\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"();");

            // Adjust CompaniesLinkedTechnicalUser
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW portal.company_linked_technical_users AS
                 SELECT
                     tu.id AS technical_user_id,
                     i.company_id AS owners,
                     CASE
                         WHEN tu.offer_subscription_id IS NOT NULL THEN o.provider_company_id
                         WHEN EXISTS (SELECT 1 FROM portal.connectors cs WHERE cs.technical_user_id = tu.id) THEN c.host_id
                         END AS provider
                 FROM portal.technical_users tu
                     JOIN portal.identities i ON tu.id = i.id
                     LEFT JOIN portal.offer_subscriptions os ON tu.offer_subscription_id = os.id
                     LEFT JOIN portal.offers o ON os.offer_id = o.id
                     LEFT JOIN portal.connectors c ON tu.id = c.technical_user_id
                 WHERE tu.technical_user_type_id = 1 AND i.identity_type_id = 2
                 UNION
                 SELECT
                     tu.id AS technical_user_id,
                     i.company_id AS owners,
                     null AS provider
                 FROM
                     portal.technical_users tu
                         JOIN portal.identities i ON tu.id = i.id
                 WHERE tu.technical_user_type_id = 2
                 UNION
                 SELECT
                     tu.id AS technical_user_id,
                     o.provider_company_id AS owners,
                     o.provider_company_id AS provider
                 FROM
                     portal.technical_users tu
                         JOIN portal.identities i ON tu.id = i.id
                         LEFT JOIN portal.offer_subscriptions os ON tu.offer_subscription_id = os.id
                         LEFT JOIN portal.offers o ON os.offer_id = o.id
                 WHERE tu.technical_user_type_id = 3");
            #endregion

            #region 20250311081235_2.4.0-rc2

            migrationBuilder.AlterColumn<string>(
               name: "region",
               schema: "portal",
               table: "addresses",
               type: "character varying(255)",
               maxLength: 255,
               nullable: false,
               defaultValue: "",
               oldClrType: typeof(string),
               oldType: "character varying(255)",
               oldMaxLength: 255,
               oldNullable: true);

            migrationBuilder.InsertData(
                schema: "portal",
                table: "process_step_types",
                columns: new[] { "id", "label" },
                values: new object[] { 808, "RETRIGGER_CONNECTOR_SELF_DESCRIPTION_WITH_OUTDATED_LEGAL_PERSON" });

            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #region 20240919142711_2.3.0-alpha.1

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() CASCADE;");

            migrationBuilder.DropForeignKey(
                name: "fk_offers_companies_provider_company_id",
                schema: "portal",
                table: "offers");

            // migrationBuilder.DropTable(
            //     name: "audit_offer20240911",
            //     schema: "portal");

            migrationBuilder.Sql("DELETE from portal.process_steps WHERE process_step_type_id = 31");

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.Sql("DELETE from portal.process_steps WHERE process_step_type_id = 32");

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.AlterColumn<Guid>(
                name: "provider_company_id",
                schema: "portal",
                table: "offers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // migrationBuilder.AddColumn<string>(
            //     name: "provider",
            //     schema: "portal",
            //     table: "offers",
            //     type: "character varying(255)",
            //     maxLength: 255,
            //     nullable: false,
            //     defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "application_id",
                schema: "portal",
                table: "company_invitations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "organisation_name",
                schema: "portal",
                table: "company_invitations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 1,
                column: "label",
                value: "VERIFY_REGISTRATION");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 9,
                column: "label",
                value: "END_CLEARING_HOUSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 16,
                column: "label",
                value: "TRIGGER_OVERRIDE_CLEARING_HOUSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 18,
                column: "label",
                value: "FINISH_SELF_DESCRIPTION_LP");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 19,
                column: "label",
                value: "DECLINE_APPLICATION");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 21,
                column: "label",
                value: "AWAIT_DIM_RESPONSE");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 26,
                column: "label",
                value: "STORED_BPN_CREDENTIAL");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 28,
                column: "label",
                value: "STORED_MEMBERSHIP_CREDENTIAL");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 101,
                column: "label",
                value: "START_AUTOSETUP");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 111,
                column: "label",
                value: "TRIGGER_ACTIVATE_SUBSCRIPTION");

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 505,
                column: "label",
                value: "AWAIT_DELETE_DIM_TECHNICAL_USER");

            migrationBuilder.InsertData(
                schema: "portal",
                table: "process_step_types",
                columns: new[] { "id", "label" },
                values: new object[,]
                {
                    { 15, "OVERRIDE_BUSINESS_PARTNER_NUMBER" },
                    { 503, "RETRIGGER_AWAIT_CREATE_DIM_TECHNICAL_USER_RESPONSE" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_offers_companies_provider_company_id",
                schema: "portal",
                table: "offers",
                column: "provider_company_id",
                principalSchema: "portal",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20231115\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"provider\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"provider\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_OFFER AFTER INSERT\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20231115\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"provider\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"provider\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_OFFER AFTER UPDATE\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"();");

            #endregion

            #region 20241021154921_2.3.0-alpha.3

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() CASCADE;");

            migrationBuilder.Sql("DELETE from portal.process_steps WHERE process_step_type_id IN (33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43)");

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 43);

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 12,
                column: "label",
                value: "ACTIVATE_APPLICATION");

            migrationBuilder.DropTable(
                name: "audit_connector20241008",
                schema: "portal");

            // drop foreign keys AppInstanceAssignedTechnicalUser (AppInstanceAssignedCompanyServiceAccount)

            migrationBuilder.DropForeignKey(
                name: "fk_app_instance_assigned_technical_users_app_instances_app_ins",
                schema: "portal",
                table: "app_instance_assigned_technical_users");

            migrationBuilder.DropForeignKey(
                name: "fk_app_instance_assigned_technical_users_technical_users_techn",
                schema: "portal",
                table: "app_instance_assigned_technical_users");

            // drop foreign keys TechnicalUsers (CompanyServiceAccount)

            migrationBuilder.DropForeignKey(
                name: "fk_technical_users_identities_id",
                schema: "portal",
                table: "technical_users");

            migrationBuilder.DropForeignKey(
                name: "fk_technical_users_offer_subscriptions_offer_subscription_id",
                schema: "portal",
                table: "technical_users");

            migrationBuilder.DropForeignKey(
                name: "fk_technical_users_technical_user_kinds_technical_user_kind_id",
                schema: "portal",
                table: "technical_users");

            migrationBuilder.DropForeignKey(
                name: "fk_technical_users_technical_user_types_technical_user_type_id",
                schema: "portal",
                table: "technical_users");

            // drop foreign keys Connector

            migrationBuilder.DropForeignKey(
                name: "fk_connectors_technical_users_technical_user_id",
                schema: "portal",
                table: "connectors");

            // drop foreign keys ExternalTechnicalUser (DimCompanyServiceAccount)

            migrationBuilder.DropForeignKey(
                name: "fk_external_technical_users_external_technical_users_id",
                schema: "portal",
                table: "external_technical_users");

            // drop foreign keys ExternalTechnicalUserCreationData (DimUserCreationData)

            migrationBuilder.DropForeignKey(
                name: "fk_external_technical_user_creation_data_processes_process_id",
                schema: "portal",
                table: "external_technical_user_creation_data");

            migrationBuilder.DropForeignKey(
                name: "fk_external_technical_user_creation_data_technical_users_techn",
                schema: "portal",
                table: "external_technical_user_creation_data");

            // as company_linked_technical_users is a view the autocreated constraint ain't work
            // the respective navigational property nevertheless works without.
            // the autocreated statement is left here for documentation purpose.

            // migrationBuilder.DropForeignKey(
            //     name: "fk_company_linked_technical_users_technical_users_technical_us",
            //     schema: "portal",
            //     table: "company_linked_technical_users");

            // AppInstanceAssignedTechnicalUser -> AppInstanceAssignedCompanyServiceAccount

            migrationBuilder.DropPrimaryKey(
                name: "pk_app_instance_assigned_technical_users",
                table: "app_instance_assigned_technical_users",
                schema: "portal");

            migrationBuilder.DropIndex(
                name: "ix_app_instance_assigned_technical_users_technical_user_id",
                table: "app_instance_assigned_technical_users",
                schema: "portal");

            migrationBuilder.RenameTable(
                name: "app_instance_assigned_technical_users",
                schema: "portal",
                newName: "app_instance_assigned_service_accounts");

            migrationBuilder.RenameColumn(
                name: "technical_user_id",
                table: "app_instance_assigned_service_accounts",
                schema: "portal",
                newName: "company_service_account_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_app_instance_assigned_service_accounts",
                table: "app_instance_assigned_service_accounts",
                schema: "portal",
                columns: ["app_instance_id", "company_service_account_id"]);

            migrationBuilder.CreateIndex(
                name: "ix_app_instance_assigned_service_accounts_company_service_acco",
                table: "app_instance_assigned_service_accounts",
                schema: "portal",
                column: "company_service_account_id");

            // TechnicalUser -> CompanyServiceAccount

            migrationBuilder.DropIndex(
                name: "ix_technical_users_client_client_id",
                table: "technical_users",
                schema: "portal");

            migrationBuilder.DropIndex(
                name: "ix_technical_users_technical_user_kind_id",
                table: "technical_users",
                schema: "portal");

            migrationBuilder.DropIndex(
                name: "ix_technical_users_technical_user_type_id",
                table: "technical_users",
                schema: "portal");

            migrationBuilder.RenameTable(
                name: "technical_users",
                schema: "portal",
                newName: "company_service_accounts");

            migrationBuilder.RenameColumn(
                name: "technical_user_kind_id",
                table: "company_service_accounts",
                schema: "portal",
                newName: "company_service_account_kind_id");

            migrationBuilder.RenameColumn(
                name: "technical_user_type_id",
                table: "company_service_accounts",
                schema: "portal",
                newName: "company_service_account_type_id");

            migrationBuilder.RenameIndex(
                name: "pk_technical_users",
                table: "company_service_accounts",
                schema: "portal",
                newName: "pk_company_service_accounts");

            migrationBuilder.CreateIndex(
                name: "ix_company_service_accounts_client_client_id",
                table: "company_service_accounts",
                schema: "portal",
                column: "client_client_id",
                filter: "client_client_id is not null AND company_service_account_kind_id = 1");

            migrationBuilder.RenameIndex(
                name: "ix_technical_users_offer_subscription_id",
                table: "company_service_accounts",
                schema: "portal",
                newName: "ix_company_service_accounts_offer_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_company_service_accounts_company_service_account_kind_id",
                table: "company_service_accounts",
                schema: "portal",
                column: "company_service_account_kind_id");

            migrationBuilder.CreateIndex(
                name: "ix_company_service_accounts_company_service_account_type_id",
                table: "company_service_accounts",
                schema: "portal",
                column: "company_service_account_type_id");

            // TechnicalUserKind -> CompanyServiceAccountKind

            migrationBuilder.RenameTable(
                name: "technical_user_kinds",
                schema: "portal",
                newName: "company_service_account_kindes");

            migrationBuilder.RenameIndex(
                name: "pk_technical_user_kinds",
                table: "company_service_account_kindes",
                schema: "portal",
                newName: "pk_company_service_account_kindes");

            // TechnicalUserType -> CompanyServiceAccountType

            migrationBuilder.RenameTable(
                name: "technical_user_types",
                schema: "portal",
                newName: "company_service_account_types");

            migrationBuilder.RenameIndex(
                name: "pk_technical_user_types",
                table: "company_service_account_types",
                schema: "portal",
                newName: "pk_company_service_account_types");

            // Connector

            migrationBuilder.DropIndex(
                name: "ix_connectors_technical_user_id",
                table: "connectors",
                schema: "portal");

            migrationBuilder.RenameColumn(
                name: "technical_user_id",
                schema: "portal",
                table: "connectors",
                newName: "company_service_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_connectors_company_service_account_id",
                table: "connectors",
                schema: "portal",
                column: "company_service_account_id");

            // ExternalTechnicalUser -> DimCompanyServiceAccount

            migrationBuilder.RenameTable(
                name: "external_technical_users",
                schema: "portal",
                newName: "dim_company_service_accounts"
            );

            migrationBuilder.RenameIndex(
                name: "pk_external_technical_users",
                table: "dim_company_service_accounts",
                schema: "portal",
                newName: "pk_dim_company_service_accounts");

            // ExternalTechnicalUserCreationData -> DimUserCreationData

            migrationBuilder.DropIndex(
                name: "ix_external_technical_user_creation_data_technical_user_id",
                table: "external_technical_user_creation_data",
                schema: "portal");

            migrationBuilder.RenameTable(
                name: "external_technical_user_creation_data",
                schema: "portal",
                newName: "dim_user_creation_data");

            migrationBuilder.RenameColumn(
                name: "technical_user_id",
                table: "dim_user_creation_data",
                schema: "portal",
                newName: "service_account_id");

            migrationBuilder.RenameIndex(
                name: "pk_external_technical_user_creation_data",
                table: "dim_user_creation_data",
                schema: "portal",
                newName: "pk_dim_user_creation_data");

            migrationBuilder.RenameIndex(
                name: "ix_external_technical_user_creation_data_process_id",
                table: "dim_user_creation_data",
                schema: "portal",
                newName: "ix_dim_user_creation_data_process_id");

            migrationBuilder.CreateIndex(
                name: "ix_dim_user_creation_data_service_account_id",
                table: "dim_user_creation_data",
                schema: "portal",
                column: "service_account_id");

            // CompaniesLinkedTechnicalUser -> CompaniesLinkedServiceAccount

            migrationBuilder.Sql(@"DROP VIEW IF EXISTS portal.company_linked_technical_users");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW portal.company_linked_service_accounts AS
                SELECT
                    csa.id AS service_account_id,
                    i.company_id AS owners,
                    CASE
                        WHEN csa.offer_subscription_id IS NOT NULL THEN o.provider_company_id
                        WHEN EXISTS (SELECT 1 FROM portal.connectors cs WHERE cs.company_service_account_id = csa.id) THEN c.host_id
                        END AS provider
                FROM portal.company_service_accounts csa
                    JOIN portal.identities i ON csa.id = i.id
                    LEFT JOIN portal.offer_subscriptions os ON csa.offer_subscription_id = os.id
                    LEFT JOIN portal.offers o ON os.offer_id = o.id
                    LEFT JOIN portal.connectors c ON csa.id = c.company_service_account_id
                WHERE csa.company_service_account_type_id = 1 AND i.identity_type_id = 2
                UNION
                SELECT
                    csa.id AS service_account_id,
                    i.company_id AS owners,
                    null AS provider
                FROM
                    portal.company_service_accounts csa
                        JOIN portal.identities i ON csa.id = i.id
                WHERE csa.company_service_account_type_id = 2
                ");

            // re-add foreign keys AppInstanceAssignedCompanyServiceAccount (AppInstanceAssignedTechnicalUser)

            migrationBuilder.AddForeignKey(
                name: "fk_app_instance_assigned_service_accounts_app_instances_app_in",
                schema: "portal",
                table: "app_instance_assigned_service_accounts",
                column: "app_instance_id",
                principalSchema: "portal",
                principalTable: "app_instances",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_app_instance_assigned_service_accounts_company_service_acco",
                schema: "portal",
                table: "app_instance_assigned_service_accounts",
                column: "company_service_account_id",
                principalSchema: "portal",
                principalTable: "company_service_accounts",
                principalColumn: "id");

            // re-add foreign keys CompanyServiceAccount (TechnicalUser)

            migrationBuilder.AddForeignKey(
                name: "fk_company_service_accounts_identities_id",
                table: "company_service_accounts",
                column: "id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "identities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_company_service_accounts_offer_subscriptions_offer_subscrip",
                table: "company_service_accounts",
                column: "offer_subscription_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "offer_subscriptions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_company_service_accounts_company_service_account_kindes_com",
                table: "company_service_accounts",
                column: "company_service_account_kind_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "company_service_account_kindes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_company_service_accounts_company_service_account_types_comp",
                table: "company_service_accounts",
                column: "company_service_account_type_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "company_service_account_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // re-add foreign keys Connector

            migrationBuilder.AddForeignKey(
                name: "fk_connectors_company_service_accounts_company_service_account",
                table: "connectors",
                column: "company_service_account_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "company_service_accounts",
                principalColumn: "id");

            // re-add foreign keys DimCompanyServiceAccount (ExternalTechnicalUser)

            migrationBuilder.AddForeignKey(
               name: "fk_dim_company_service_accounts_company_service_accounts_id",
               table: "dim_company_service_accounts",
               column: "id",
               schema: "portal",
               principalSchema: "portal",
               principalTable: "company_service_accounts",
               principalColumn: "id",
               onDelete: ReferentialAction.Cascade);

            // re-add foreign keys DimUserCreationData (ExternalTechnicalUserCreationData)

            migrationBuilder.AddForeignKey(
                name: "fk_dim_user_creation_data_processes_process_id",
                table: "dim_user_creation_data",
                column: "process_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "processes",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_dim_user_creation_data_company_service_accounts_service_acc",
                table: "dim_user_creation_data",
                column: "service_account_id",
                schema: "portal",
                principalSchema: "portal",
                principalTable: "company_service_accounts",
                principalColumn: "id");

            // as company_linked_technical_users is a view the autocreated constraint ain't work
            // the respective navigational property nevertheless works without.
            // the autocreated statement is left here for documentation purpose.

            // migrationBuilder.AddForeignKey(
            //     name: "fk_company_linked_service_accounts_company_service_accounts_co",
            //     table: "company_linked_service_accounts",
            //     column: "company_service_account",
            //     schema: "portal",
            //     principalSchema: "portal",
            //     principalTable: "company_service_accounts",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.Cascade);

            // AuditConnector20240814

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20240814\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"company_service_account_id\", \"sd_creation_process_id\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"company_service_account_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_CONNECTOR AFTER INSERT\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20240814\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"company_service_account_id\", \"sd_creation_process_id\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"company_service_account_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_CONNECTOR AFTER UPDATE\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"();");
            #endregion

            #region 20241213081306_2.4.0-alpha.1

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_DOCUMENT\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_DOCUMENT\"() CASCADE;");

            migrationBuilder.DropTable(
                name: "audit_document20241120",
                schema: "portal");

            migrationBuilder.DropColumn(
                name: "document_size",
                schema: "portal",
                table: "documents");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_DOCUMENT\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_DOCUMENT$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_document20231115\" (\"id\", \"date_created\", \"document_hash\", \"document_content\", \"document_name\", \"media_type_id\", \"document_type_id\", \"document_status_id\", \"company_user_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"date_created\", \r\n  NEW.\"document_hash\", \r\n  NEW.\"document_content\", \r\n  NEW.\"document_name\", \r\n  NEW.\"media_type_id\", \r\n  NEW.\"document_type_id\", \r\n  NEW.\"document_status_id\", \r\n  NEW.\"company_user_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_DOCUMENT$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_DOCUMENT AFTER INSERT\r\nON \"portal\".\"documents\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_DOCUMENT\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_DOCUMENT\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_DOCUMENT$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_document20231115\" (\"id\", \"date_created\", \"document_hash\", \"document_content\", \"document_name\", \"media_type_id\", \"document_type_id\", \"document_status_id\", \"company_user_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"date_created\", \r\n  NEW.\"document_hash\", \r\n  NEW.\"document_content\", \r\n  NEW.\"document_name\", \r\n  NEW.\"media_type_id\", \r\n  NEW.\"document_type_id\", \r\n  NEW.\"document_status_id\", \r\n  NEW.\"company_user_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_DOCUMENT$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_DOCUMENT AFTER UPDATE\r\nON \"portal\".\"documents\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_DOCUMENT\"();");
            #endregion

            #region 20250107114624_2.4.0-alpha.2

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() CASCADE;");

            migrationBuilder.DropTable(
                name: "audit_offer20241219",
                schema: "portal");

            migrationBuilder.DropColumn(
                name: "display_technical_user",
                schema: "portal",
                table: "offers");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20240911\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_OFFER AFTER INSERT\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20240911\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_OFFER AFTER UPDATE\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"();");

            #endregion

            #region 20250207083336_2.4.0-rc1

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() CASCADE;");

            migrationBuilder.DropForeignKey(
                name: "fk_process_steps_process_step_statuses_process_step_status_id",
                schema: "portal",
                table: "process_steps");

            migrationBuilder.DropTable(
                name: "agreement_descriptions",
                schema: "portal");

            migrationBuilder.DropTable(
                name: "audit_connector20250113",
                schema: "portal");

            migrationBuilder.DropTable(
                name: "audit_offer20250121",
                schema: "portal");

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "notification_type",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "notification_type",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 804);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 805);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 806);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "process_step_types",
                keyColumn: "id",
                keyValue: 807);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "technical_user_types",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "sd_skipped_date",
                schema: "portal",
                table: "connectors");

            migrationBuilder.AddColumn<bool>(
                name: "display_technical_user",
                schema: "portal",
                table: "offers",
                type: "boolean",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "portal",
                table: "identity_type",
                keyColumn: "id",
                keyValue: 2,
                column: "label",
                value: "COMPANY_SERVICE_ACCOUNT");

            migrationBuilder.AddForeignKey(
                name: "fk_process_steps_process_step_statuses_process_step_status_id",
                schema: "portal",
                table: "process_steps",
                column: "process_step_status_id",
                principalSchema: "portal",
                principalTable: "process_step_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20241008\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"technical_user_id\", \"sd_creation_process_id\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"technical_user_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_CONNECTOR AFTER INSERT\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_CONNECTOR\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_CONNECTOR$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_connector20241008\" (\"id\", \"name\", \"connector_url\", \"type_id\", \"status_id\", \"provider_id\", \"host_id\", \"self_description_document_id\", \"location_id\", \"self_description_message\", \"date_last_changed\", \"technical_user_id\", \"sd_creation_process_id\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"connector_url\", \r\n  NEW.\"type_id\", \r\n  NEW.\"status_id\", \r\n  NEW.\"provider_id\", \r\n  NEW.\"host_id\", \r\n  NEW.\"self_description_document_id\", \r\n  NEW.\"location_id\", \r\n  NEW.\"self_description_message\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"technical_user_id\", \r\n  NEW.\"sd_creation_process_id\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_CONNECTOR$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_CONNECTOR AFTER UPDATE\r\nON \"portal\".\"connectors\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_CONNECTOR\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_INSERT_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20241219\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"display_technical_user\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"display_technical_user\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  1, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_INSERT_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_INSERT_OFFER AFTER INSERT\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_INSERT_OFFER\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"() RETURNS trigger as $LC_TRIGGER_AFTER_UPDATE_OFFER$\r\nBEGIN\r\n  INSERT INTO \"portal\".\"audit_offer20241219\" (\"id\", \"name\", \"date_created\", \"date_released\", \"marketing_url\", \"contact_email\", \"contact_number\", \"display_technical_user\", \"offer_type_id\", \"sales_manager_id\", \"provider_company_id\", \"offer_status_id\", \"license_type_id\", \"date_last_changed\", \"last_editor_id\", \"audit_v1id\", \"audit_v1operation_id\", \"audit_v1date_last_changed\", \"audit_v1last_editor_id\") SELECT NEW.\"id\", \r\n  NEW.\"name\", \r\n  NEW.\"date_created\", \r\n  NEW.\"date_released\", \r\n  NEW.\"marketing_url\", \r\n  NEW.\"contact_email\", \r\n  NEW.\"contact_number\", \r\n  NEW.\"display_technical_user\", \r\n  NEW.\"offer_type_id\", \r\n  NEW.\"sales_manager_id\", \r\n  NEW.\"provider_company_id\", \r\n  NEW.\"offer_status_id\", \r\n  NEW.\"license_type_id\", \r\n  NEW.\"date_last_changed\", \r\n  NEW.\"last_editor_id\", \r\n  gen_random_uuid(), \r\n  2, \r\n  CURRENT_TIMESTAMP, \r\n  NEW.\"last_editor_id\";\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_AFTER_UPDATE_OFFER$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_OFFER AFTER UPDATE\r\nON \"portal\".\"offers\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"portal\".\"LC_TRIGGER_AFTER_UPDATE_OFFER\"();");

            // Revert adjusted changes in CompaniesLinkedTechnicalUser
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW portal.company_linked_technical_users AS
                 SELECT
                     tu.id AS technical_user_id,
                     i.company_id AS owners,
                     CASE
                         WHEN tu.offer_subscription_id IS NOT NULL THEN o.provider_company_id
                         WHEN EXISTS (SELECT 1 FROM portal.connectors cs WHERE cs.technical_user_id = tu.id) THEN c.host_id
                         END AS provider
                 FROM portal.technical_users tu
                     JOIN portal.identities i ON tu.id = i.id
                     LEFT JOIN portal.offer_subscriptions os ON tu.offer_subscription_id = os.id
                     LEFT JOIN portal.offers o ON os.offer_id = o.id
                     LEFT JOIN portal.connectors c ON tu.id = c.technical_user_id
                 WHERE tu.technical_user_type_id = 1 AND i.identity_type_id = 2
                 UNION
                 SELECT
                     tu.id AS technical_user_id,
                     i.company_id AS owners,
                     null AS provider
                 FROM
                     portal.technical_users tu
                         JOIN portal.identities i ON tu.id = i.id
                 WHERE tu.technical_user_type_id = 2
              ");

            #endregion

            #region 20250311081235_2.4.0-rc2

            migrationBuilder.DeleteData(
               schema: "portal",
               table: "process_step_types",
               keyColumn: "id",
               keyValue: 808);

            migrationBuilder.AlterColumn<string>(
                name: "region",
                schema: "portal",
                table: "addresses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldDefaultValue: "");
            #endregion
        }
    }
}
