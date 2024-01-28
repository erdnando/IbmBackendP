using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Domain.Entities.AssignmentReport;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands.UpdateAproveedNivel1
{
    public class UpdateAproveedCommand : IUpdateAproveedCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private IEmailCommand _emailCommand;
        private IGetListUsuarioCommand _usuarioCommand;

        public UpdateAproveedCommand(IDataBaseService dataBaseService, IMapper mapper, IEmailCommand emailCommand, IGetListUsuarioCommand usuarioCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

            _emailCommand = emailCommand;
            _usuarioCommand = usuarioCommand;
        }
        public async Task<ModelAproveed> Execute(ModelAproveed modelAprobador)
        {

            var ReportUpdate = _dataBaseService.assignmentReports.
                        Where(x => x.HorusReportEntityId == modelAprobador.HorusReportEntityId)
                        .FirstOrDefault();

            //0 aprobado 1 rechazado
            if (modelAprobador.State == 0)
            {
               

                if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    ReportUpdate.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1; //<----APROBADO
                    ReportUpdate.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    ReportUpdate.Description = modelAprobador.Description;
                    ReportUpdate.DateApprovalCancellation = DateTime.Now;
                    var entityreport = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(ReportUpdate);

                    _dataBaseService.assignmentReports.Update(ReportUpdate);
                    await _dataBaseService.SaveAsync();

                    //nivel 1
                    CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
                    assignmentReport.UserEntityId = modelAprobador.UserId;
                    assignmentReport.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentReport.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1;
                    assignmentReport.Description = "";
                    assignmentReport.DateApprovalCancellation = DateTime.Now;
                                        
                    assignmentReport.IdAssignmentReport = Guid.NewGuid();

                    var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                    await _dataBaseService.assignmentReports.AddAsync(entity);
                    await _dataBaseService.SaveAsync();
                }

                if (modelAprobador.roleAprobador != "Usuario Aprobador N2")
                {
                    ReportUpdate.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2; //<----APROBADO
                    ReportUpdate.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    ReportUpdate.Description = modelAprobador.Description;
                    ReportUpdate.DateApprovalCancellation = DateTime.Now;
                    var entityreport = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(ReportUpdate);

                    _dataBaseService.assignmentReports.Update(ReportUpdate);
                    await _dataBaseService.SaveAsync();

                    //nivel 2
                    CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
                    assignmentReport.UserEntityId = modelAprobador.UserId;
                    assignmentReport.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentReport.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    assignmentReport.Description = "";
                    assignmentReport.DateApprovalCancellation = DateTime.Now;

                    //var response = CrearNivel2(assignmentReport);
                    assignmentReport.IdAssignmentReport = Guid.NewGuid();
                    var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                    await _dataBaseService.assignmentReports.AddAsync(entity);
                    await _dataBaseService.SaveAsync();
                }
                

            }
            else
            {
                if (modelAprobador.State == 2)
                {
                    ReportUpdate.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    ReportUpdate.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    ReportUpdate.Description = modelAprobador.Description;
                    ReportUpdate.DateApprovalCancellation = DateTime.Now;

                    _dataBaseService.assignmentReports.Update(ReportUpdate);
                    _dataBaseService.SaveAsync();
                }
            }

            try
            {
                if (modelAprobador.State == 2)
                {
                    //aprovado
                    if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                    {
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador1UserEntityId).Result).Email, Plantilla = "5" });
                    }
                    if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                    {
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador1UserEntityId).Result).Email, Plantilla = "5" });
                    }



                    //Email para el empleado
                    if (modelAprobador.roleAprobador != "Usuario Aprobador N2")
                    {
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador2UserEntityId).Result).Email, Plantilla = "5" });
                    }
                        
                    _emailCommand.SendEmail(new EmailModel { To = ( _usuarioCommand.GetByUsuarioId(modelAprobador.EmpleadoUserEntityId).Result).Email, Plantilla = "5" });
                }
                else
                {
                   
                    _emailCommand.SendEmail(new EmailModel { To = ( _usuarioCommand.GetByUsuarioId(modelAprobador.EmpleadoUserEntityId).Result).Email, Plantilla = "3" });
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }



            return modelAprobador;

        }

        public CreateAssignmentReportModel CrearNivel2(CreateAssignmentReportModel model)
        {
            model.IdAssignmentReport = Guid.NewGuid();
            var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(model);

            _dataBaseService.assignmentReports.Add(entity);
            _dataBaseService.SaveAsync();





            return model;
        }
    }
}
