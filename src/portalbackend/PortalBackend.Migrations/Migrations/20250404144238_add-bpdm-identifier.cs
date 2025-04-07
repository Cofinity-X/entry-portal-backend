using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Org.Eclipse.TractusX.Portal.Backend.PortalBackend.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class addbpdmidentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                                    DO $$
                                    BEGIN
                                        IF NOT EXISTS (SELECT 1 FROM portal.bpdm_identifiers WHERE id = 101) THEN
                                            INSERT INTO portal.bpdm_identifiers (id, label) VALUES (101, 'US_EIN_CD');
                                        END IF;

                                        IF NOT EXISTS (SELECT 1 FROM portal.bpdm_identifiers WHERE id = 102) THEN
                                            INSERT INTO portal.bpdm_identifiers (id, label) VALUES (102, 'KR_TIN_CD');
                                        END IF;

                                        IF NOT EXISTS (SELECT 1 FROM portal.bpdm_identifiers WHERE id = 103) THEN
                                            INSERT INTO portal.bpdm_identifiers (id, label) VALUES (103, 'JP_CN_CD');
                                        END IF;

                                        IF NOT EXISTS (SELECT 1 FROM portal.bpdm_identifiers WHERE id = 104) THEN
                                            INSERT INTO portal.bpdm_identifiers (id, label) VALUES (104, 'CN_CC_CD');
                                        END IF;
                                    END
                                    $$;
                                 ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "portal",
                table: "bpdm_identifiers",
                keyColumn: "id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "bpdm_identifiers",
                keyColumn: "id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "bpdm_identifiers",
                keyColumn: "id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                schema: "portal",
                table: "bpdm_identifiers",
                keyColumn: "id",
                keyValue: 104);
        }
    }
}
