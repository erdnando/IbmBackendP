using Algar.Hours.Application.DataBase;
using Algar.Hours.Domain.Entities;
using Algar.Hours.Domain.Entities.Aprobador;
using Algar.Hours.Domain.Entities.AprobadorUsuario;
using Algar.Hours.Domain.Entities.AssignmentReport;
using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.Gerentes;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.HorusReportManagerEntity;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.Load.Philadedata;
using Algar.Hours.Domain.Entities.Menu;
using Algar.Hours.Domain.Entities.PaisRelacionGMT;
using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.PortalDB;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Algar.Hours.Domain.Entities.ReportException;
using Algar.Hours.Domain.Entities.Rol;
using Algar.Hours.Domain.Entities.RolMenu;
using Algar.Hours.Domain.Entities.User;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Algar.Hours.Domain.Entities.WorkdayException;
using Algar.Hours.Persistence.Configuration;
using EFCore.BulkExtensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Persistence.DataBase
{
    public class DatabaseService : DbContext, IDataBaseService
    {
        //DbContext _dbContext;
        
        public DatabaseService(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AssignmentReport> assignmentReports { get; set; }
        public DbSet<ClientEntity> ClientEntity { get; set; }
        public DbSet<CountryEntity> CountryEntity { get; set; }
        public DbSet<HorusReportEntity> HorusReportEntity { get; set; }
        public DbSet<MenuEntity> MenuEntity { get; set; }
        public DbSet<ParametersEntity> ParametersEntity { get; set; }
        public DbSet<RoleEntity> RoleEntity { get; set; }
        public DbSet<RoleMenuEntity> RoleMenuEntity { get; set; }
        public DbSet<UserEntity> UserEntity { get; set; }
        public DbSet<UsersExceptions> UsersExceptions { get; set; }
        public DbSet<ReportExceptionEntity> ReportExceptionEntity { get; set; }
        public DbSet<WorkdayExceptionEntity> WorkdayExceptionEntity { get; set; }
        public DbSet<Aprobador> Aprobador { get; set; }
        public DbSet<AprobadorUsuario> AprobadorUsuario { get; set; }
        public DbSet<ARPLoadDetailEntity> ARPLoadDetailEntity { get; set; }
        public DbSet<ARPLoadEntity> ARPLoadEntity { get; set; }
        public DbSet<FestivosEntity> FestivosEntity { get; set; }
        public DbSet<workinghoursEntity> workinghoursEntity { get; set; }
        public DbSet<STELoadEntity> STELoadEntity { get; set; }
        public DbSet<TSELoadEntity> TSELoadEntity { get; set; }
        public DbSet<ParametersArpInitialEntity> ParametersArpInitialEntity { get; set; }
        public DbSet<QueuesAcceptanceEntity> QueuesAcceptanceEntity { get; set; }
        public DbSet<QueuesAcceptanceEntityTSE> QueuesAcceptanceEntityTSE { get; set; }
        public DbSet<QueuesAcceptanceEntitySTE> QueuesAcceptanceEntitySTE { get; set; }
        public DbSet<ParametersTseInitialEntity> ParametersTseInitialEntity { get; set; }
        public DbSet<ParametersSteInitialEntity> ParametersSteInitialEntity { get; set; }
        public DbSet<UserZonaHoraria> UserZonaHoraria { get; set; }
        public DbSet<Philadedata> Philadedata { get; set; }
        public DbSet<HorusReportManagerEntity> HorusReportManagerEntity { get; set; }
        public DbSet<UserManagerEntity> UserManagerEntity { get; set; }
        public DbSet<PaisRelacionGMTEntity> PaisRelacionGMTEntity { get; set; }
        public DbSet<PortalDBEntity> PortalDBEntity { get; set; }
        public DbSet<PortalDBAproveHistoryEntity> PortalDBAproveHistoryEntity { get; set; }
        public DbSet<UserSessionEntity> UserSessionEntity { get; set; }

        public void BulkInsertParametersArpInitialEntity(List<ParametersArpInitialEntity> records)
        {
            this.BulkInsert(records);
        }
        public void BulkInsertARPLoadDetailEntity(List<ARPLoadDetailEntity> records)
        {
            this.BulkInsert(records);
        }


        public async Task<bool> SaveAsync()
        {
            return await SaveChangesAsync() > 0;
        }
      
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityConfuguration(modelBuilder);
        }
        private void EntityConfuguration(ModelBuilder modelBuilder)
        {
            new AssignmentReportConfiguration(modelBuilder.Entity<AssignmentReport>());
            new ClientConfiguration(modelBuilder.Entity<ClientEntity>());
            new CountryConfiguration(modelBuilder.Entity<CountryEntity>());
            new HorusReportEntityConfiguration(modelBuilder.Entity<HorusReportEntity>());
            new MenuEntiryConfiguration(modelBuilder.Entity<MenuEntity>());
            new ParametersEntityConfiguration(modelBuilder.Entity<ParametersEntity>());
            new RolEntityConfiguration(modelBuilder.Entity<RoleEntity>());
            new RolMenuEntityConfiguration(modelBuilder.Entity<RoleMenuEntity>());
            new UserConfiguration(modelBuilder.Entity<UserEntity>());
            new UserExceptionsConfiguration(modelBuilder.Entity<UsersExceptions>());
            new ReportExceptionEntityConfiguration(modelBuilder.Entity<ReportExceptionEntity>());
            new AprobadorConfiguration(modelBuilder.Entity<Aprobador>());
            new AprobadorUsuarioConfiguration(modelBuilder.Entity<AprobadorUsuario>());
            new ARPLoadDetailConfiguration(modelBuilder.Entity<ARPLoadDetailEntity>());
            new ARPLoadConfiguration(modelBuilder.Entity<ARPLoadEntity>());
            new FestivosEntityConfiguration(modelBuilder.Entity<FestivosEntity>());
            new workinghoursEntityConfiguration(modelBuilder.Entity<workinghoursEntity>());
            new TSELoadConfiguration(modelBuilder.Entity<TSELoadEntity>());
            new STELoadConfiguration(modelBuilder.Entity<STELoadEntity>());
            new ParametersInitialEnityConfiguration(modelBuilder.Entity<ParametersArpInitialEntity>());
            new QueuesAcceptanceEntityConfiguration(modelBuilder.Entity<QueuesAcceptanceEntity>());
            new ParametersInitialEntityTSEConfiguration(modelBuilder.Entity<ParametersTseInitialEntity>());
            new ParametersInitialEntitySTEConfiguration(modelBuilder.Entity<ParametersSteInitialEntity>());
            new QueuesAcceptanceEntityTSEConfiguration(modelBuilder.Entity<QueuesAcceptanceEntityTSE>());
            new QueuesAcceptanceEntitySTEConfiguration(modelBuilder.Entity<QueuesAcceptanceEntitySTE>());
            new UserZonaHorariaConfiguration(modelBuilder.Entity<UserZonaHoraria>());
            new HorusReportManagerEntityConfiguration(modelBuilder.Entity<HorusReportManagerEntity>());
            new UserManagerEntityConfiguration(modelBuilder.Entity<UserManagerEntity>());
            new PaisRelacionGMTEntityConfiguration(modelBuilder.Entity<PaisRelacionGMTEntity>());
            new PortalDBEntityConfiguration(modelBuilder.Entity<PortalDBEntity>());
            new PortalDBAproveHistoryEntityConfiguration(modelBuilder.Entity<PortalDBAproveHistoryEntity>());
            new UserSessionConfiguration(modelBuilder.Entity<UserSessionEntity>());

        }

     
    }
}
