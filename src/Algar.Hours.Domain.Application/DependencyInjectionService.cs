using Algar.Hours.Application.Configuration;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Update;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.Create;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.UpdateAproveedNivel1;
using Algar.Hours.Application.DataBase.Client.Commands.Consult;
using Algar.Hours.Application.DataBase.Client.Commands.Create;
using Algar.Hours.Application.DataBase.Client.Commands.Update;
using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.Country.Commands.Create;
using Algar.Hours.Application.DataBase.Country.Commands.Update;
using Algar.Hours.Application.DataBase.Dashboard.Commands.Consult;
using Algar.Hours.Application.DataBase.Festivos.Consult;
using Algar.Hours.Application.DataBase.Festivos.Create;
using Algar.Hours.Application.DataBase.Festivos.Update;
using Algar.Hours.Application.DataBase.Festivos.Delete;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Consult;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using Algar.Hours.Application.DataBase.HorusReport.Commands.DetailAssigment;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Update;
using Algar.Hours.Application.DataBase.HoursReport.Commands.Consult;
using Algar.Hours.Application.DataBase.LoadData.LoadData;
using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using Algar.Hours.Application.DataBase.Menu.Commands.GetList;
using Algar.Hours.Application.DataBase.Menu.Commands.Update;
using Algar.Hours.Application.DataBase.Parameters.Commands.ConsulParameters;
using Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters;
using Algar.Hours.Application.DataBase.Parameters.Commands.UpdateParameters;
using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Application.DataBase.Rol.Commands.Consult;
using Algar.Hours.Application.DataBase.Rol.Commands.CreateRol;
using Algar.Hours.Application.DataBase.Rol.Commands.Update;
using Algar.Hours.Application.DataBase.RolMenu.Commands.Consult;
using Algar.Hours.Application.DataBase.RolMenu.Commands.CreateRolMenu;
using Algar.Hours.Application.DataBase.RolMenu.Commands.Delete;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.ListHoursUser;
using Algar.Hours.Application.DataBase.User.Commands.Login;
using Algar.Hours.Application.DataBase.User.Commands.Update;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.UserException.Commands.Consult;
using Algar.Hours.Application.DataBase.UserException.Commands.Create;
using Algar.Hours.Application.DataBase.UserException.Commands.Update;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Consult;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create;
using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Application.DataBase.ReportException.Commands.Update;
using Algar.Hours.Application.DataBase.ReportException.Commands.Create;
using Algar.Hours.Application.DataBase.ReportException.Commands.Delete;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateUserSession;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Consult;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Update;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Create;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Delete;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Activate;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Deactivate;
using Algar.Hours.Application.DataBase.Template.Commands.Create;
using Algar.Hours.Application.DataBase.Template.Commands.Consult;

namespace Algar.Hours.Application
{
    public static class DependencyInjectionService
    {

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var mapper = new MapperConfiguration(config =>
            {
                config.AddProfile(new MapperProfile());

            });
            services.AddSingleton(mapper.CreateMapper());
            services.AddTransient<ICreateUserCommand, CreateUserCommand>();
            services.AddTransient<ICreateRolCommand, CreateRolCommand>();
            services.AddTransient<ICreateMenuCommand, CreateMenuCommand>();
            services.AddTransient<ICreateParametersCommand, CreateParametersCommand>();
            services.AddTransient<ICreateRolMenuCommand, CreateRolMenuCommand>();
            services.AddTransient<ILoginUserCommand, LoginUserCommand>();
            services.AddTransient<IConsultMenuCommand, ConsultMenuCommand>();
            services.AddTransient<IGetListMenuCommand, GetListMenuCommand>();
            services.AddTransient<IUpdateMenuCommand, UpdateMenuCommand>();
            services.AddTransient<IConsultAprobadorCommand, ConsultAprobadorCommand>();
            services.AddTransient<IGetListRolCommand, GetListRolCommand>();
            services.AddTransient<IGetListUsuarioCommand, GetListUsuarioCommand>();
            services.AddTransient<IUpdateUsuarioCommand, UpdateUsuarioCommand>();
            services.AddTransient<IUpdateRolCommand, UpdateRolCommand>();
            services.AddTransient<ICreateAprobadorCommand, CreateAprobadorCommand>();
            services.AddTransient<IUpdateAprobadorCommand, UpdateAprobadorCommand>();
            services.AddTransient<IListHoursUserCommand, ListHoursUserCommand>();
            services.AddTransient<IConsultParametersCommand, ConsultParametersCommand>();
            services.AddTransient<IUpdtaeParametersCommand, UpdtaeParametersCommand>();
            services.AddTransient<ILoginUserCommand, LoginUserCommand>();
            services.AddTransient<IConsultMenuCommand, ConsultMenuCommand>();
            services.AddTransient<IGetListMenuCommand, GetListMenuCommand>();
            services.AddTransient<IUpdateMenuCommand, UpdateMenuCommand>();
            services.AddTransient<IConsultAprobadorCommand, ConsultAprobadorCommand>();
            services.AddTransient<IGetListRolCommand, GetListRolCommand>();
            services.AddTransient<IGetListUsuarioCommand, GetListUsuarioCommand>();
            services.AddTransient<IUpdateUsuarioCommand, UpdateUsuarioCommand>();
            services.AddTransient<IUpdateRolCommand, UpdateRolCommand>();
            services.AddTransient<ICreateAprobadorCommand, CreateAprobadorCommand>();
            services.AddTransient<IUpdateAprobadorCommand, UpdateAprobadorCommand>();
            services.AddTransient<IUpdateFestivoCommand, UpdateFestivoCommand>();
            services.AddTransient<IDeleteFestivoCommand, DeleteFestivoCommand>();
            services.AddTransient<ICreateFestivoCommand, CreateFestivoCommand>();
            services.AddTransient<IConsultFestivosCommand, ConsultFestivosCommand>();
            services.AddTransient<IConsultRolMenuCommand, ConsultRolMenuCommand>();
            services.AddTransient<IDeleteRolMenuCommand, DeleteRolMenuCommand>();
            services.AddTransient<ICreateWorkingHoursCommand, CreateWorkingHoursCommand>();
            services.AddTransient<IListUserAproveedCommand, ListUserAproveedCommand>();
            services.AddTransient<IConsultWorkingHoursCommand, ConsultWorkingHoursCommand>();
            services.AddTransient<IConsultUserExceptionCommand, ConsultUserExceptionCommand>();
            services.AddTransient<IUpdateUsersExceptionCommand, UpdateUsersExceptionCommand>();
            services.AddTransient<ICreateUsersExceptionCommand, CreateUsersExceptionCommand>();
            services.AddTransient<IConsultReportExceptionCommand, ConsultReportExceptionCommand>();
            services.AddTransient<IUpdateReportExceptionCommand, UpdateReportExceptionCommand>();
            services.AddTransient<ICreateReportExceptionCommand, CreateReportExceptionCommand>();
            services.AddTransient<IDeleteReportExceptionCommand, DeleteReportExceptionCommand>();
            services.AddTransient<IConsultWorkdayExceptionCommand, ConsultWorkdayExceptionCommand>();
            services.AddTransient<IUpdateWorkdayExceptionCommand, UpdateWorkdayExceptionCommand>();
            services.AddTransient<ICreateWorkdayExceptionCommand, CreateWorkdayExceptionCommand>();
            services.AddTransient<IDeleteWorkdayExceptionCommand, DeleteWorkdayExceptionCommand>();
            services.AddTransient<IActivateWorkdayExceptionCommand, ActivateWorkdayExceptionCommand>();
            services.AddTransient<IDeactivateWorkdayExceptionCommand, DeactivateWorkdayExceptionCommand>();
            services.AddTransient<IConsultTemplateCommand, ConsultTemplateCommand>();
            services.AddTransient<ICreateTemplateCommand, CreateTemplateCommand>();
            services.AddTransient<ILoadWorkingHoursCommand, LoadWorkingHoursCommand>();
            services.AddTransient<ILoadHorusReportManagerCommand, LoadHoursReportManagerCommand>();
            services.AddTransient<ICreateHorusReportManagerCommand, CreateHorusReportManagerCommand>();
            services.AddTransient<IReporte1Command, Reporte1Command>();
            services.AddTransient<IEmailCommand, EmailCommand>();
            services.AddTransient<ILoadGeneric, LoadGeneric>();
            services.AddTransient<ICreateLogCommand, CreateLogCommand>();
            

            #region AssigmentReportCommand
            services.AddTransient<ICreateAssignmentReportCommand, CreateAssignmentReportCommand>();
            services.AddTransient<IConsultDetailAssigmentCommand, ConsultDetailAssigmentCommand>();
            services.AddTransient<IUpdateAproveedCommand, UpdateAproveedCommand>();
            #endregion

            #region Client
            services.AddTransient<ICreateClientCommand, CreateClientCommand>();
            services.AddTransient<IConsultClientCommand, ConsultClientCommand>();
            services.AddTransient<IUpdateClientCommand, UpdateClientCommand>();
            #endregion

            #region Country
            services.AddTransient<ICreateCountryCommand, CreateCountryCommand>();
            services.AddTransient<IConsultCountryCommand, ConsultCountryCommand>();
            services.AddTransient<IUpdateCountryCommand, UpdateCountryCommand>();
            #endregion

            #region Country
            services.AddTransient<ICreateHorusReportCommand, CreateHorusReportCommand>();
            services.AddTransient<IConsultHorusReportCommand, ConsultHorusReportCommand>();
            services.AddTransient<IUpdateHorusReportCommand, UpdateHorusReportCommand>();
            #endregion

            #region ArchivosLoad
            services.AddTransient<ILoadHoursReport, LoadHoursReport>();

            #endregion

            #region UserExepction
            services.AddTransient<IConsultUserExceptionCommand, ConsultUserExceptionCommand>();
            services.AddTransient<ICreateUsersExceptionCommand, CreateUsersExceptionCommand>();
            services.AddTransient<IUpdateUsersExceptionCommand, UpdateUsersExceptionCommand>();
            #endregion

            return services;
        }

    }
}
