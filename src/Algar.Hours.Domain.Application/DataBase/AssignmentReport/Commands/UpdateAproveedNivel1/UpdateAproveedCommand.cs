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
        public  ModelAproveed Execute(ModelAproveed modelAprobador)
        {

            var ReportUpdate = _dataBaseService.assignmentReports.
                        Where(x => x.HorusReportEntityId == modelAprobador.HorusReportEntityId)
                        .FirstOrDefault();


            if (modelAprobador.State == 1)
            {
                ReportUpdate.State = (byte)Enums.Enums.Aprobacion.Aprobado;
                ReportUpdate.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                ReportUpdate.Description = modelAprobador.Description;
                ReportUpdate.DateApprovalCancellation = DateTime.Now;
                var entityreport = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(ReportUpdate);

                _dataBaseService.assignmentReports.Update(ReportUpdate);
                _dataBaseService.SaveAsync();


                CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();

                assignmentReport.UserEntityId = modelAprobador.UserId;
                assignmentReport.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                assignmentReport.State = (byte)Enums.Enums.Aprobacion.Pendiente;
                assignmentReport.Description = "";
                assignmentReport.DateApprovalCancellation = DateTime.Now;

                var response = CrearNivel2(assignmentReport);

            }
            else
            {
                if (modelAprobador.State == 2)
                {
                    ReportUpdate.State = (byte)Enums.Enums.Aprobacion.Rechazado;
                    ReportUpdate.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    ReportUpdate.Description = modelAprobador.Description;
                    ReportUpdate.DateApprovalCancellation = DateTime.Now;

                    _dataBaseService.assignmentReports.Update(ReportUpdate);
                    _dataBaseService.SaveAsync();
                }
            }

            try
            {
                if (modelAprobador.State == 1)
                {
                    //aprovado
                    _emailCommand.SendEmail(new EmailModel { To = ( _usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador1UserEntityId).Result).Email, Plantilla = "5" });
                    _emailCommand.SendEmail(new EmailModel { To = ( _usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador2UserEntityId).Result).Email, Plantilla = "5" });
                    _emailCommand.SendEmail(new EmailModel { To = ( _usuarioCommand.GetByUsuarioId(modelAprobador.EmpleadoUserEntityId).Result).Email, Plantilla = "5" });
                }
                else
                {
                    //rechazado
                   // _emailCommand.SendEmail(new EmailModel { To = ( _usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador1UserEntityId).Result).Email, Plantilla = "4" });
                   // _emailCommand.SendEmail(new EmailModel { To = ( _usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador2UserEntityId).Result).Email, Plantilla = "4" });
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

            _dataBaseService.assignmentReports.AddAsync(entity);
            _dataBaseService.SaveAsync();





            return model;
        }
    }
}
