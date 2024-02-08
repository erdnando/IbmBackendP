using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Domain.Entities.AssignmentReport;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            //obtiene ref al reporte (horus report) que se va a afectar
            var currentAssignment = _dataBaseService.assignmentReports.
            Where(x => x.HorusReportEntityId == modelAprobador.HorusReportEntityId && x.IdAssignmentReport==modelAprobador.idAssignmentReport)
            .FirstOrDefault();

            if (modelAprobador.State == 0) { //APROBADO

                if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    //Se marca como aprobada N1 para el aprobador N1
                    currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1;
                    currentAssignment.TipoAssignment = "Employee";
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();


                    //Crea su history N1 al empleado
                    var assignmentN1 = new CreateAssignmentReportModel();
                    assignmentN1.IdAssignmentReport = Guid.NewGuid();
                    assignmentN1.UserEntityId = Guid.Parse(modelAprobador.EmpleadoUserEntityId.ToString());
                    assignmentN1.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1;
                    assignmentN1.Employee = currentAssignment.Employee;
                    assignmentN1.TipoAssignment = "Employee";
                    assignmentN1.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentN1.Description = modelAprobador.Description;
                    assignmentN1.DateApprovalCancellation = DateTime.Now;

                    var entityN1 = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentN1);
                    await _dataBaseService.assignmentReports.AddAsync(entityN1);
                    await _dataBaseService.SaveAsync();



                    //Crea asignacion para el N2
                    var assignmentN2 = new CreateAssignmentReportModel();
                    assignmentN2.IdAssignmentReport = Guid.NewGuid();
                    assignmentN2.UserEntityId = Guid.Parse(modelAprobador.Aprobador2UserEntityId.ToString());
                    assignmentN2.State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
                    assignmentN2.Employee = currentAssignment.Employee;
                    assignmentN2.TipoAssignment = "Approver";
                    assignmentN2.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentN2.Description = modelAprobador.Description;
                    assignmentN2.DateApprovalCancellation = DateTime.Now;

                    var entityN2 = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentN2);
                    await _dataBaseService.assignmentReports.AddAsync(entityN2);
                    await _dataBaseService.SaveAsync();


                   


                }else if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    //Se actualiza el estado a la asignacion del N2 
                    currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    currentAssignment.TipoAssignment = "Approver";
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();


                    //Crea su registro N2
                    var assignmentN2 = new CreateAssignmentReportModel();
                    assignmentN2.IdAssignmentReport = Guid.NewGuid();
                    assignmentN2.UserEntityId = Guid.Parse(modelAprobador.EmpleadoUserEntityId.ToString());
                    assignmentN2.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    assignmentN2.Employee = currentAssignment.Employee;
                    assignmentN2.TipoAssignment = "Employee";
                    assignmentN2.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentN2.Description = modelAprobador.Description;
                    assignmentN2.DateApprovalCancellation = DateTime.Now;

                    var entityN2 = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentN2);
                    await _dataBaseService.assignmentReports.AddAsync(entityN2);
                    await _dataBaseService.SaveAsync();


                    
                }

            }
            else if (modelAprobador.State == 1) //RECHAZADO
            {
               

                if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    //Se actualiza el estado a la asignacion del N1
                    currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentAssignment.TipoAssignment = "Approver";
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();



                    //se le crea al usuario su evidencia
                    var assignmentN1 = new CreateAssignmentReportModel();
                    assignmentN1.UserEntityId = Guid.Parse(modelAprobador.EmpleadoUserEntityId.ToString());
                    assignmentN1.Employee = currentAssignment.Employee;
                    assignmentN1.TipoAssignment = "Employee";
                    assignmentN1.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentN1.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado; 
                    assignmentN1.Description = modelAprobador.Description;
                    assignmentN1.DateApprovalCancellation = DateTime.Now;
                    
                    assignmentN1.IdAssignmentReport = Guid.NewGuid();
                    var entityN1 = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentN1);
                    _dataBaseService.assignmentReports.Add(entityN1);
                    await _dataBaseService.SaveAsync();



                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    //Se actualiza el estado a la asignacion del N2
                    currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentAssignment.TipoAssignment = "Approver";
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();


                    //se le crea al usuario su evidencia
                    var assignmentReport = new CreateAssignmentReportModel();
                    assignmentReport.UserEntityId = Guid.Parse(modelAprobador.EmpleadoUserEntityId.ToString());
                    assignmentReport.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    assignmentReport.Employee = currentAssignment.Employee;
                    assignmentReport.TipoAssignment = "Employee";
                    assignmentReport.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    
                    assignmentReport.Description = modelAprobador.Description;
                    assignmentReport.DateApprovalCancellation = DateTime.Now;
                    assignmentReport.IdAssignmentReport = Guid.NewGuid();

                    var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                    _dataBaseService.assignmentReports.Add(entity);
                    await _dataBaseService.SaveAsync();
                }
            }




                try
                {
                if (modelAprobador.State == 0)
                {
                    //aprobado
                    if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                    {
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador1UserEntityId).Result).Email, Plantilla = "23" });
                    }
                    if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                    {
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador1UserEntityId).Result).Email, Plantilla = "25" });
                    }



                    //Email para el empleado
                    if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                    {
                        //TODO validar correo
                        /*try
                        {
                            _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador2UserEntityId).Result).Email, Plantilla = "5" });
                        }
                        catch(Exception ex)
                        {

                        }*/
                        
                    }

                    try
                    {
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.EmpleadoUserEntityId).Result).Email, Plantilla = "5" });
                    }
                    catch (Exception exx)
                    {

                     
                    }
                    
                }
                else
                {
                    try
                    {
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.EmpleadoUserEntityId).Result).Email, Plantilla = "3" });
                    }
                    catch (Exception exc)
                    {

                        
                    }
                    
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
