using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class alterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"ARPLoadDetailEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"ARPLoadEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"Aprobador\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"AprobadorUsuario\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"ClientEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"CountryEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"FestivosEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"HorusReportEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"MenuEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"ParametersArpInitialEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"ParametersEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"ParametersSteInitialEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"ParametersTseInitialEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"Philadedata\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"QueuesAcceptanceEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"QueuesAcceptanceEntitySTE\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"QueuesAcceptanceEntityTSE\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"RoleEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"RoleMenuEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"STELoadEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"TSELoadEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"UserEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"UserZonaHoraria\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"UsersExceptions\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"assignmentReports\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"workinghoursEntity\"  OWNER to \"ibm-cloud-base-user\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
