using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aprobador",
                columns: table => new
                {
                    IdAprobador = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Nivel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aprobador", x => x.IdAprobador);
                });

            migrationBuilder.CreateTable(
                name: "ClientEntity",
                columns: table => new
                {
                    IdClient = table.Column<Guid>(type: "uuid", nullable: false),
                    NameClient = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEntity", x => x.IdClient);
                });

            migrationBuilder.CreateTable(
                name: "CountryEntity",
                columns: table => new
                {
                    IdCounty = table.Column<Guid>(type: "uuid", nullable: false),
                    NameCountry = table.Column<string>(type: "text", nullable: false),
                    CodigoPais = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryEntity", x => x.IdCounty);
                });

            migrationBuilder.CreateTable(
                name: "MenuEntity",
                columns: table => new
                {
                    IdMenu = table.Column<Guid>(type: "uuid", nullable: false),
                    NameMenu = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuEntity", x => x.IdMenu);
                });

            migrationBuilder.CreateTable(
                name: "Philadedata",
                columns: table => new
                {
                    IdPhiladedata = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraInicio = table.Column<string>(type: "text", nullable: false),
                    HoraFin = table.Column<string>(type: "text", nullable: false),
                    TotalHoras = table.Column<double>(type: "double precision", nullable: false),
                    Comentarios = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Philadedata", x => x.IdPhiladedata);
                });

            migrationBuilder.CreateTable(
                name: "RoleEntity",
                columns: table => new
                {
                    IdRole = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRole = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleEntity", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "STELoadEntity",
                columns: table => new
                {
                    IdSTELoad = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionTimeId = table.Column<string>(type: "text", nullable: false),
                    SessionTimeAgentCountry = table.Column<string>(type: "text", nullable: false),
                    NumeroCaso = table.Column<string>(type: "text", nullable: false),
                    SessionEmployeeSerialNumber = table.Column<string>(type: "text", nullable: false),
                    AccountCMRNumber = table.Column<string>(type: "text", nullable: false),
                    NombreCuenta = table.Column<string>(type: "text", nullable: false),
                    StartDateTime = table.Column<string>(type: "text", nullable: false),
                    EndDateTime = table.Column<string>(type: "text", nullable: false),
                    EndHours = table.Column<string>(type: "text", nullable: false),
                    StartHours = table.Column<string>(type: "text", nullable: false),
                    TotalDuration = table.Column<string>(type: "text", nullable: false),
                    CaseSubject = table.Column<string>(type: "text", nullable: false),
                    FechaRegistro = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STELoadEntity", x => x.IdSTELoad);
                });

            migrationBuilder.CreateTable(
                name: "TSELoadEntity",
                columns: table => new
                {
                    IdTSELoad = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioISO2 = table.Column<string>(type: "text", nullable: false),
                    WorkOrder = table.Column<string>(type: "text", nullable: false),
                    NumeroEmpleado = table.Column<string>(type: "text", nullable: false),
                    ZonaHoraria = table.Column<string>(type: "text", nullable: false),
                    AccountCMRNumber = table.Column<string>(type: "text", nullable: false),
                    AccountNameText = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<string>(type: "text", nullable: false),
                    EndTime = table.Column<string>(type: "text", nullable: false),
                    StartHours = table.Column<string>(type: "text", nullable: false),
                    EndHours = table.Column<string>(type: "text", nullable: false),
                    HoraInicio = table.Column<string>(type: "text", nullable: false),
                    HoraFin = table.Column<string>(type: "text", nullable: false),
                    DurationInHours = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    FechaRegistro = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TSELoadEntity", x => x.IdTSELoad);
                });

            migrationBuilder.CreateTable(
                name: "UserZonaHoraria",
                columns: table => new
                {
                    IdUserZone = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    ZonaHorariaU = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserZonaHoraria", x => x.IdUserZone);
                });

            migrationBuilder.CreateTable(
                name: "FestivosEntity",
                columns: table => new
                {
                    IdFestivo = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    ano = table.Column<string>(type: "text", nullable: false),
                    DiaFestivo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FestivosEntity", x => x.IdFestivo);
                    table.ForeignKey(
                        name: "FK_FestivosEntity_CountryEntity_CountryId",
                        column: x => x.CountryId,
                        principalTable: "CountryEntity",
                        principalColumn: "IdCounty",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParametersEntity",
                columns: table => new
                {
                    IdParametersEntity = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetTimeDay = table.Column<double>(type: "double precision", nullable: false),
                    TargetHourWeek = table.Column<double>(type: "double precision", nullable: false),
                    TargetHourMonth = table.Column<double>(type: "double precision", nullable: false),
                    TargetHourYear = table.Column<double>(type: "double precision", nullable: false),
                    TypeLimits = table.Column<int>(type: "integer", nullable: false),
                    TypeHours = table.Column<int>(type: "integer", nullable: false),
                    CountryEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametersEntity", x => x.IdParametersEntity);
                    table.ForeignKey(
                        name: "FK_ParametersEntity_CountryEntity_CountryEntityId",
                        column: x => x.CountryEntityId,
                        principalTable: "CountryEntity",
                        principalColumn: "IdCounty",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenuEntity",
                columns: table => new
                {
                    IdRoleMenu = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenuEntity", x => x.IdRoleMenu);
                    table.ForeignKey(
                        name: "FK_RoleMenuEntity_MenuEntity_MenuEntityId",
                        column: x => x.MenuEntityId,
                        principalTable: "MenuEntity",
                        principalColumn: "IdMenu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenuEntity_RoleEntity_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleEntity",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    IdUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameUser = table.Column<string>(type: "text", nullable: false),
                    surnameUser = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    RoleEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_UserEntity_CountryEntity_CountryEntityId",
                        column: x => x.CountryEntityId,
                        principalTable: "CountryEntity",
                        principalColumn: "IdCounty",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEntity_RoleEntity_RoleEntityId",
                        column: x => x.RoleEntityId,
                        principalTable: "RoleEntity",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParametersSteInitialEntity",
                columns: table => new
                {
                    IdParamSTEInitialId = table.Column<Guid>(type: "uuid", nullable: false),
                    HoraInicio = table.Column<string>(type: "text", nullable: false),
                    HoraFin = table.Column<string>(type: "text", nullable: false),
                    OutIme = table.Column<string>(type: "text", nullable: false),
                    OverTime = table.Column<string>(type: "text", nullable: false),
                    Semana = table.Column<int>(type: "integer", nullable: false),
                    Festivo = table.Column<string>(type: "text", nullable: false),
                    HorasInicio = table.Column<int>(type: "integer", nullable: false),
                    HorasFin = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    STELoadEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametersSteInitialEntity", x => x.IdParamSTEInitialId);
                    table.ForeignKey(
                        name: "FK_ParametersSteInitialEntity_STELoadEntity_STELoadEntityId",
                        column: x => x.STELoadEntityId,
                        principalTable: "STELoadEntity",
                        principalColumn: "IdSTELoad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueuesAcceptanceEntitySTE",
                columns: table => new
                {
                    IdQueuesAcceptanceEntitySTE = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_empleado = table.Column<string>(type: "text", nullable: false),
                    Hora_Inicio = table.Column<string>(type: "text", nullable: false),
                    Hora_Fin = table.Column<string>(type: "text", nullable: false),
                    Horas_Total = table.Column<double>(type: "double precision", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: false),
                    AprobadoSistema = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    STELoadEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaRe = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuesAcceptanceEntitySTE", x => x.IdQueuesAcceptanceEntitySTE);
                    table.ForeignKey(
                        name: "FK_QueuesAcceptanceEntitySTE_STELoadEntity_STELoadEntityId",
                        column: x => x.STELoadEntityId,
                        principalTable: "STELoadEntity",
                        principalColumn: "IdSTELoad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParametersTseInitialEntity",
                columns: table => new
                {
                    IdParamTSEInitialId = table.Column<Guid>(type: "uuid", nullable: false),
                    HoraInicio = table.Column<string>(type: "text", nullable: false),
                    HoraFin = table.Column<string>(type: "text", nullable: false),
                    OutIme = table.Column<string>(type: "text", nullable: false),
                    OverTime = table.Column<string>(type: "text", nullable: false),
                    Semana = table.Column<int>(type: "integer", nullable: false),
                    Festivo = table.Column<string>(type: "text", nullable: false),
                    HorasInicio = table.Column<int>(type: "integer", nullable: false),
                    HorasFin = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    TSELoadEntityIdTSELoad = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametersTseInitialEntity", x => x.IdParamTSEInitialId);
                    table.ForeignKey(
                        name: "FK_ParametersTseInitialEntity_TSELoadEntity_TSELoadEntityIdTSE~",
                        column: x => x.TSELoadEntityIdTSELoad,
                        principalTable: "TSELoadEntity",
                        principalColumn: "IdTSELoad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueuesAcceptanceEntityTSE",
                columns: table => new
                {
                    IdQueuesAcceptanceEntityTSE = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_empleado = table.Column<string>(type: "text", nullable: false),
                    Hora_Inicio = table.Column<string>(type: "text", nullable: false),
                    Hora_Fin = table.Column<string>(type: "text", nullable: false),
                    Horas_Total = table.Column<double>(type: "double precision", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: false),
                    AprobadoSistema = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    TSELoadEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaRe = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuesAcceptanceEntityTSE", x => x.IdQueuesAcceptanceEntityTSE);
                    table.ForeignKey(
                        name: "FK_QueuesAcceptanceEntityTSE_TSELoadEntity_TSELoadEntityId",
                        column: x => x.TSELoadEntityId,
                        principalTable: "TSELoadEntity",
                        principalColumn: "IdTSELoad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AprobadorUsuario",
                columns: table => new
                {
                    IdAprobadorUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AprobadorUsuario", x => x.IdAprobadorUsuario);
                    table.ForeignKey(
                        name: "FK_AprobadorUsuario_Aprobador_AprobadorId",
                        column: x => x.AprobadorId,
                        principalTable: "Aprobador",
                        principalColumn: "IdAprobador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AprobadorUsuario_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ARPLoadEntity",
                columns: table => new
                {
                    IdArpLoad = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    userEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARPLoadEntity", x => x.IdArpLoad);
                    table.ForeignKey(
                        name: "FK_ARPLoadEntity_UserEntity_userEntityId",
                        column: x => x.userEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HorusReportEntity",
                columns: table => new
                {
                    IdHorusReport = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<string>(type: "text", nullable: false),
                    EndTime = table.Column<string>(type: "text", nullable: false),
                    ClientEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoReporte = table.Column<int>(type: "integer", nullable: false),
                    DateApprovalSystem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Acitivity = table.Column<int>(type: "integer", nullable: false),
                    CountHours = table.Column<string>(type: "text", nullable: false),
                    NumberReport = table.Column<int>(type: "integer", nullable: false),
                    ApproverId = table.Column<string>(type: "text", nullable: false),
                    ARPLoadingId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorusReportEntity", x => x.IdHorusReport);
                    table.ForeignKey(
                        name: "FK_HorusReportEntity_ClientEntity_ClientEntityId",
                        column: x => x.ClientEntityId,
                        principalTable: "ClientEntity",
                        principalColumn: "IdClient");
                    table.ForeignKey(
                        name: "FK_HorusReportEntity_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersExceptions",
                columns: table => new
                {
                    IdUsersExceptions = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    horas = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersExceptions", x => x.IdUsersExceptions);
                    table.ForeignKey(
                        name: "FK_UsersExceptions_UserEntity_UserId",
                        column: x => x.UserId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workinghoursEntity",
                columns: table => new
                {
                    IdworkinghoursEntity = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    week = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    HoraInicio = table.Column<string>(type: "text", nullable: false),
                    HoraFin = table.Column<string>(type: "text", nullable: false),
                    Day = table.Column<string>(type: "text", nullable: false),
                    Ano = table.Column<string>(type: "text", nullable: false),
                    FechaWorking = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workinghoursEntity", x => x.IdworkinghoursEntity);
                    table.ForeignKey(
                        name: "FK_workinghoursEntity_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ARPLoadDetailEntity",
                columns: table => new
                {
                    IdDetail = table.Column<Guid>(type: "uuid", nullable: false),
                    DOC_NUM = table.Column<string>(type: "text", nullable: false),
                    TOOL = table.Column<string>(type: "text", nullable: false),
                    PAIS = table.Column<string>(type: "text", nullable: false),
                    ID_EMPLEADO = table.Column<string>(type: "text", nullable: false),
                    NUMERO_CLIENTE = table.Column<string>(type: "text", nullable: false),
                    NOMBRE_CLIENTE = table.Column<string>(type: "text", nullable: false),
                    ESTADO = table.Column<string>(type: "text", nullable: false),
                    FECHA_REP = table.Column<string>(type: "text", nullable: false),
                    HORA_INICIO = table.Column<string>(type: "text", nullable: false),
                    HORA_FIN = table.Column<string>(type: "text", nullable: false),
                    TOTAL_MINUTOS = table.Column<string>(type: "text", nullable: false),
                    CATEGORIA = table.Column<string>(type: "text", nullable: false),
                    ACTIVIDAD = table.Column<string>(type: "text", nullable: false),
                    COMENTARIO = table.Column<string>(type: "text", nullable: false),
                    FECHA_EXTRATED = table.Column<string>(type: "text", nullable: false),
                    ARPLoadEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARPLoadDetailEntity", x => x.IdDetail);
                    table.ForeignKey(
                        name: "FK_ARPLoadDetailEntity_ARPLoadEntity_ARPLoadEntityId",
                        column: x => x.ARPLoadEntityId,
                        principalTable: "ARPLoadEntity",
                        principalColumn: "IdArpLoad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assignmentReports",
                columns: table => new
                {
                    IdAssignmentReport = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    HorusReportEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DateApprovalCancellation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignmentReports", x => x.IdAssignmentReport);
                    table.ForeignKey(
                        name: "FK_assignmentReports_HorusReportEntity_HorusReportEntityId",
                        column: x => x.HorusReportEntityId,
                        principalTable: "HorusReportEntity",
                        principalColumn: "IdHorusReport",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_assignmentReports_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParametersArpInitialEntity",
                columns: table => new
                {
                    IdParametersInitialEntity = table.Column<Guid>(type: "uuid", nullable: false),
                    HoraInicio = table.Column<string>(type: "text", nullable: false),
                    HoraFin = table.Column<string>(type: "text", nullable: false),
                    OutIme = table.Column<string>(type: "text", nullable: false),
                    OverTime = table.Column<string>(type: "text", nullable: false),
                    Semana = table.Column<int>(type: "integer", nullable: false),
                    Festivo = table.Column<string>(type: "text", nullable: false),
                    HorasInicio = table.Column<int>(type: "integer", nullable: false),
                    HorasFin = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    ARPLoadDetailEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametersArpInitialEntity", x => x.IdParametersInitialEntity);
                    table.ForeignKey(
                        name: "FK_ParametersArpInitialEntity_ARPLoadDetailEntity_ARPLoadDetai~",
                        column: x => x.ARPLoadDetailEntityId,
                        principalTable: "ARPLoadDetailEntity",
                        principalColumn: "IdDetail",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueuesAcceptanceEntity",
                columns: table => new
                {
                    IdQueuesAcceptanceEntity = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_empleado = table.Column<string>(type: "text", nullable: false),
                    Hora_Inicio = table.Column<string>(type: "text", nullable: false),
                    Hora_Fin = table.Column<string>(type: "text", nullable: false),
                    Horas_Total = table.Column<double>(type: "double precision", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: false),
                    AprobadoSistema = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    ARPLoadDetailEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaRe = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuesAcceptanceEntity", x => x.IdQueuesAcceptanceEntity);
                    table.ForeignKey(
                        name: "FK_QueuesAcceptanceEntity_ARPLoadDetailEntity_ARPLoadDetailEnt~",
                        column: x => x.ARPLoadDetailEntityId,
                        principalTable: "ARPLoadDetailEntity",
                        principalColumn: "IdDetail",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AprobadorUsuario_AprobadorId",
                table: "AprobadorUsuario",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_AprobadorUsuario_UserEntityId",
                table: "AprobadorUsuario",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ARPLoadDetailEntity_ARPLoadEntityId",
                table: "ARPLoadDetailEntity",
                column: "ARPLoadEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ARPLoadEntity_userEntityId",
                table: "ARPLoadEntity",
                column: "userEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_assignmentReports_HorusReportEntityId",
                table: "assignmentReports",
                column: "HorusReportEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_assignmentReports_UserEntityId",
                table: "assignmentReports",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_FestivosEntity_CountryId",
                table: "FestivosEntity",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_HorusReportEntity_ClientEntityId",
                table: "HorusReportEntity",
                column: "ClientEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_HorusReportEntity_UserEntityId",
                table: "HorusReportEntity",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersArpInitialEntity_ARPLoadDetailEntityId",
                table: "ParametersArpInitialEntity",
                column: "ARPLoadDetailEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersEntity_CountryEntityId",
                table: "ParametersEntity",
                column: "CountryEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersSteInitialEntity_STELoadEntityId",
                table: "ParametersSteInitialEntity",
                column: "STELoadEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersTseInitialEntity_TSELoadEntityIdTSELoad",
                table: "ParametersTseInitialEntity",
                column: "TSELoadEntityIdTSELoad");

            migrationBuilder.CreateIndex(
                name: "IX_QueuesAcceptanceEntity_ARPLoadDetailEntityId",
                table: "QueuesAcceptanceEntity",
                column: "ARPLoadDetailEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuesAcceptanceEntitySTE_STELoadEntityId",
                table: "QueuesAcceptanceEntitySTE",
                column: "STELoadEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuesAcceptanceEntityTSE_TSELoadEntityId",
                table: "QueuesAcceptanceEntityTSE",
                column: "TSELoadEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuEntity_MenuEntityId",
                table: "RoleMenuEntity",
                column: "MenuEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuEntity_RoleId",
                table: "RoleMenuEntity",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntity_CountryEntityId",
                table: "UserEntity",
                column: "CountryEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntity_RoleEntityId",
                table: "UserEntity",
                column: "RoleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersExceptions_UserId",
                table: "UsersExceptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_workinghoursEntity_UserEntityId",
                table: "workinghoursEntity",
                column: "UserEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AprobadorUsuario");

            migrationBuilder.DropTable(
                name: "assignmentReports");

            migrationBuilder.DropTable(
                name: "FestivosEntity");

            migrationBuilder.DropTable(
                name: "ParametersArpInitialEntity");

            migrationBuilder.DropTable(
                name: "ParametersEntity");

            migrationBuilder.DropTable(
                name: "ParametersSteInitialEntity");

            migrationBuilder.DropTable(
                name: "ParametersTseInitialEntity");

            migrationBuilder.DropTable(
                name: "Philadedata");

            migrationBuilder.DropTable(
                name: "QueuesAcceptanceEntity");

            migrationBuilder.DropTable(
                name: "QueuesAcceptanceEntitySTE");

            migrationBuilder.DropTable(
                name: "QueuesAcceptanceEntityTSE");

            migrationBuilder.DropTable(
                name: "RoleMenuEntity");

            migrationBuilder.DropTable(
                name: "UsersExceptions");

            migrationBuilder.DropTable(
                name: "UserZonaHoraria");

            migrationBuilder.DropTable(
                name: "workinghoursEntity");

            migrationBuilder.DropTable(
                name: "Aprobador");

            migrationBuilder.DropTable(
                name: "HorusReportEntity");

            migrationBuilder.DropTable(
                name: "ARPLoadDetailEntity");

            migrationBuilder.DropTable(
                name: "STELoadEntity");

            migrationBuilder.DropTable(
                name: "TSELoadEntity");

            migrationBuilder.DropTable(
                name: "MenuEntity");

            migrationBuilder.DropTable(
                name: "ClientEntity");

            migrationBuilder.DropTable(
                name: "ARPLoadEntity");

            migrationBuilder.DropTable(
                name: "UserEntity");

            migrationBuilder.DropTable(
                name: "CountryEntity");

            migrationBuilder.DropTable(
                name: "RoleEntity");
        }
    }
}
