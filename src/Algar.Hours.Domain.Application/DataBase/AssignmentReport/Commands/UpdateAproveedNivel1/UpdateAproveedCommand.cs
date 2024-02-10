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
            //======================================================================================================================================
            //obtiene ref a la assignacion seleccionada (assignment) 
            var currentAssignment = _dataBaseService.assignmentReports.
            Where(x => x.HorusReportEntityId == modelAprobador.HorusReportEntityId && x.IdAssignmentReport==modelAprobador.idAssignmentReport)
            .FirstOrDefault();

            //obtiene ref al reporte (horus report) basado en el assignment seleccionado
            var currentHReport = _dataBaseService.HorusReportEntity
                   .Include(a => a.UserEntity)
                   .Include(a => a.ClientEntity)
                   .Where(x => x.IdHorusReport == currentAssignment.HorusReportEntityId).FirstOrDefault();
            //========================================================================================================================================



            if (modelAprobador.State == 0) { //APROBADO

                if (modelAprobador.roleAprobador == "Usuario estandar")
                {

                    //Se maerca reporte como pendiente
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    _dataBaseService.HorusReportEntity.Update(currentHReport);
                    await _dataBaseService.SaveAsync();

                    //Se marca como atendida por el empleado
                    currentAssignment.State = 1;
                    currentAssignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN0;
                    currentAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    currentAssignment.Nivel = 0;
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();

                    //Crea asignacion para el N1
                    var assignmentN1 = new CreateAssignmentReportModel();
                    assignmentN1.IdAssignmentReport = Guid.NewGuid();
                    //Solo en caso de standar, como el no es aprobador, se toma al N2 como N1
                    assignmentN1.UserEntityId = Guid.Parse(modelAprobador.Aprobador2UserEntityId.ToString());
                    assignmentN1.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentN1.State =0;
                    assignmentN1.Description = modelAprobador.Description;
                    assignmentN1.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    var entityN1 = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentN1);
                    await _dataBaseService.assignmentReports.AddAsync(entityN1);
                    await _dataBaseService.SaveAsync();



                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {

                    //Se maerca reporte como pendiente
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    _dataBaseService.HorusReportEntity.Update(currentHReport);
                    await _dataBaseService.SaveAsync();


                    //Se marca como atendida por el empleado
                    currentAssignment.State = 1;
                    currentAssignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1;
                    currentAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    currentAssignment.Nivel = 1;
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();

                    //Crea asignacion para el N2
                    var assignmentN1 = new CreateAssignmentReportModel();
                    assignmentN1.IdAssignmentReport = Guid.NewGuid();
                    assignmentN1.UserEntityId = Guid.Parse(modelAprobador.Aprobador2UserEntityId.ToString());
                    assignmentN1.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentN1.State = 0;
                    assignmentN1.Description = modelAprobador.Description;
                    assignmentN1.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    var entityN1 = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentN1);
                    await _dataBaseService.assignmentReports.AddAsync(entityN1);
                    await _dataBaseService.SaveAsync();


                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    //Se maerca reporte como aprobado N2
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    _dataBaseService.HorusReportEntity.Update(currentHReport);
                     await _dataBaseService.SaveAsync();

                    //Se marca como atendida por el empleado
                    currentAssignment.State = 1;
                    currentAssignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    currentAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.Nivel = 2;
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();

                    

                }

            }
            else if (modelAprobador.State == 1) //RECHAZADO
            {

                if (modelAprobador.roleAprobador == "Usuario estandar")
                {

                    //Se rechaza el reporte de hrs 
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    
                    _dataBaseService.HorusReportEntity.Update(currentHReport);
                    await _dataBaseService.SaveAsync();

                    //Se marca como atendida por el empleado
                    currentAssignment.State = 1;
                    currentAssignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.Nivel = 0;
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();

                   




                } else if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    //Se rechaza el reporte de hrs 
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    
                    _dataBaseService.HorusReportEntity.Update(currentHReport);
                    await _dataBaseService.SaveAsync();

                    //Se marca como atendida por el empleado
                    currentAssignment.State = 1;
                    currentAssignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.Nivel = 1;
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();


                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    //Se rechaza el reporte de hrs 
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    
                    _dataBaseService.HorusReportEntity.Update(currentHReport);
                    await _dataBaseService.SaveAsync();

                    //Se marca como atendida por el empleado
                    currentAssignment.State = 1;
                    currentAssignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.Nivel = 2;
                    _dataBaseService.assignmentReports.Update(currentAssignment);
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
