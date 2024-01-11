using Algar.Hours.Domain.Entities;
using Algar.Hours.Domain.Entities.AprobadorUsuario;
using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.HorusReportManagerEntity;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.Load.Philadedata;
using Algar.Hours.Domain.Entities.Menu;
using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Algar.Hours.Domain.Entities.Rol;
using Algar.Hours.Domain.Entities.RolMenu;
using Algar.Hours.Domain.Entities.User;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Microsoft.EntityFrameworkCore;


namespace Algar.Hours.Application.DataBase
{
    public interface IDataBaseService
    {
        DbSet<Domain.Entities.AssignmentReport.AssignmentReport> assignmentReports { get; set; }
        DbSet<ClientEntity> ClientEntity { get; set; }
        DbSet<CountryEntity> CountryEntity { get; set; }
        DbSet<HorusReportEntity> HorusReportEntity { get; set; }
        DbSet<HorusReportManagerEntity> HorusReportManagerEntity { get; set; }
        DbSet<MenuEntity> MenuEntity { get; set; }
        DbSet<ParametersEntity> ParametersEntity { get; set; }
        DbSet<RoleEntity> RoleEntity { get; set; }
        DbSet<RoleMenuEntity> RoleMenuEntity { get; set; }
        DbSet<UserEntity> UserEntity { get; set; }
        DbSet<UsersExceptions> UsersExceptions { get; set; }
        DbSet<Domain.Entities.Aprobador.Aprobador> Aprobador { get; set; }
        DbSet<AprobadorUsuario> AprobadorUsuario { get; set; }
        DbSet<ARPLoadDetailEntity> ARPLoadDetailEntity { get; set; }
        DbSet<ARPLoadEntity> ARPLoadEntity { get; set; }
        DbSet<FestivosEntity> FestivosEntity { get; set; }
        DbSet<STELoadEntity> STELoadEntity { get; set; }
        DbSet<TSELoadEntity> TSELoadEntity { get; set; }
        DbSet<workinghoursEntity> workinghoursEntity { get; set; }
        DbSet<ParametersArpInitialEntity> ParametersArpInitialEntity { get; set; }
        DbSet<QueuesAcceptanceEntity> QueuesAcceptanceEntity { get; set; }
        DbSet<QueuesAcceptanceEntityTSE> QueuesAcceptanceEntityTSE { get; set; }
        DbSet<QueuesAcceptanceEntitySTE> QueuesAcceptanceEntitySTE { get; set; }
        DbSet<ParametersTseInitialEntity> ParametersTseInitialEntity { get; set; }
        DbSet<ParametersSteInitialEntity> ParametersSteInitialEntity { get; set; }
        DbSet<UserZonaHoraria> UserZonaHoraria { get; set; }
        DbSet<Philadedata> Philadedata { get; set; }
        Task<bool> SaveAsync();
    }
}
