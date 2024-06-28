using Algar.Hours.Domain.Entities.PortalDB;
using Algar.Hours.Domain.Entities.ReportException;
using Algar.Hours.Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class permisosTLSDEV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ARPLoadDetailEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ARPLoadEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"Aprobador\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"AprobadorUsuario\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ClientEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"CountryEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"FestivosEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"HorusReportEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"MenuEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ParametersArpInitialEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ParametersEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ParametersSteInitialEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ParametersTseInitialEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"Philadedata\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"QueuesAcceptanceEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"QueuesAcceptanceEntitySTE\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"QueuesAcceptanceEntityTSE\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"RoleEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"RoleMenuEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"STELoadEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"TSELoadEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"UserEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"UserZonaHoraria\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"UsersExceptions\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"assignmentReports\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"workinghoursEntity\"  OWNER to \"ibm-cloud-base-user\";");

            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"HorusReportManagerEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"PaisRelacionGMTEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"PortalDBAproveHistoryEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"PortalDBEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"ReportExceptionEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"UserManagerEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"UserSessionEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls_dev.\"WorkdayExceptionEntity\"  OWNER to \"ibm-cloud-base-user\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
