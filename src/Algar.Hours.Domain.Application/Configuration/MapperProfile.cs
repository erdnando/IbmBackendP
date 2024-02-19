using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.UpdateAproveedNivel1;
using Algar.Hours.Application.DataBase.Client.Commands;
using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Dashboard.Commands.Consult;
using Algar.Hours.Application.DataBase.Festivos.Consult;
using Algar.Hours.Application.DataBase.Festivos.Create;
using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Consult;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using Algar.Hours.Application.DataBase.LoadData.LoadData;
using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using Algar.Hours.Application.DataBase.Parameters.Commands.ConsulParameters;
using Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters;
using Algar.Hours.Application.DataBase.Parameters.Commands.UpdateParameters;
using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Application.DataBase.RolMenu.Commands.Consult;
using Algar.Hours.Application.DataBase.RolMenu.Commands.CreateRolMenu;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.ListHoursUser;
using Algar.Hours.Application.DataBase.UserException.Commands.Consult;
using Algar.Hours.Application.DataBase.UserException.Commands.Create;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using Algar.Hours.Domain.Entities.Aprobador;
using Algar.Hours.Domain.Entities.AprobadorUsuario;
using Algar.Hours.Domain.Entities.AssignmentReport;
using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.HorusReportManagerEntity;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.Menu;
using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.Rol;
using Algar.Hours.Domain.Entities.RolMenu;
using Algar.Hours.Domain.Entities.User;
using Algar.Hours.Domain.Entities.UsersExceptions;
using AutoMapper;
using System.Text.Json.Nodes;
using Algar.Hours.Domain.Entities.PortalDB;
using Algar.Hours.Application.DataBase.PortalDB.Commands.Create;
using Algar.Hours.Application.DataBase.PortalDB.Commands;
using Algar.Hours.Application.DataBase.PortalDBAproveHistory.Commands.Create;
using Algar.Hours.Application.DataBase.ReportException.Commands.Create;
using Algar.Hours.Domain.Entities.ReportException;
using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;

namespace Algar.Hours.Application.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserEntity, CreateUserModel>().ReverseMap();
            CreateMap<RoleEntity, CreateRolModel>().ReverseMap();
            CreateMap<Domain.Entities.AssignmentReport.AssignmentReport, CreateAssignmentReportModel>().ReverseMap();
            CreateMap<ClientEntity, ClientModel>().ReverseMap();
            CreateMap<CountryEntity, CountryModel>().ReverseMap();
            CreateMap<HorusReportEntity, HorusReportModel>().ReverseMap();
            CreateMap<HorusReportEntity, HorusReportModel>().ReverseMap();
            CreateMap<MenuEntity, MenuModel>().ReverseMap();
            CreateMap<ParametersEntity, CreateParametersModel>().ReverseMap();
            CreateMap<RoleEntity, CreateRolModel>().ReverseMap();
            CreateMap<RoleEntity, RolModel>().ReverseMap();
            CreateMap<RoleMenuEntity, CreateRolMenuModel>().ReverseMap();
            CreateMap<UserEntity, CreateUserModel>().ReverseMap();
            CreateMap<MenuModel,MenuEntity>().ReverseMap();
            CreateMap<AprobadorUsuarioModel, AprobadorUsuario>().ReverseMap();
            CreateMap<AprobadorUsuario,AprobadorUsuarioModel>().ReverseMap();
            CreateMap<Aprobador,AprobadorModel>().ReverseMap();
            CreateMap<Aprobador,AprobadorModel>().ReverseMap();
            CreateMap<CreateUserModelc,UserEntity>().ReverseMap();
            CreateMap<ListHorursUserModel, HorusReportEntity>().ReverseMap();
            CreateMap<ConsultParametersModel, ParametersEntity>().ReverseMap();
            CreateMap<UpdateParametersModel, ParametersEntity>().ReverseMap();
            CreateMap<JsonArray,ARPLoadDetailEntity>().ReverseMap();
            CreateMap<JsonObject, ARPLoadDetailEntity>().ReverseMap();    
            CreateMap<FestivosEntity,FestivosModel>().ReverseMap();
            CreateMap<FestivosEntity,CreateFestivoModel>().ReverseMap();
            CreateMap<MenuEntity,MenuModelc>().ReverseMap();
            CreateMap<RoleMenuEntity,RolMenuModel>().ReverseMap();
            CreateMap<ConsultMoldeHosrusReportModel, HorusReportEntity>().ReverseMap();
            CreateMap<CreateWorkingHoursModel, workinghoursEntity>().ReverseMap();
            CreateMap<ListAproveedModel,AssignmentReport>().ReverseMap();
            CreateMap<AssignmentReport, ModelAproveed>().ReverseMap();
            CreateMap<ARPLoadEntity, ARPLoadEntityc>().ReverseMap();
            CreateMap<UsersExceptions, UserExceptionModel>().ReverseMap();
            CreateMap<ReportExceptionEntity, ReportExceptionModel>().ReverseMap();
            CreateMap<ReportExceptionModel, ReportExceptionModelC>().ReverseMap();
            CreateMap<ReportExceptionEntity, ReportExceptionModelC>().ReverseMap();
            CreateMap<ParametersTseInitialEntity, ParametersTseInitialEntityC>().ReverseMap();
            CreateMap<HorusReportManagerEntity, CreateHorusReportManagerModel>().ReverseMap();
            CreateMap<PortalDBEntity, CreatePortalDBModel>().ReverseMap();
            CreateMap<PortalDBModel, CreatePortalDBModel>().ReverseMap();
            CreateMap<PortalDBEntity, PortalDBModel>().ReverseMap();
            CreateMap<PortalDBAproveHistoryEntity, CreatePortalDBAproveHistoryModel>().ReverseMap();

            CreateMap<CreateHorusReportModel,HorusReportModel>()
                .ForMember(x=> x.StrStartDate , 
                opt => opt.MapFrom(src =>  DateTime.Parse(src.StartTime)));

        }

    }
}
