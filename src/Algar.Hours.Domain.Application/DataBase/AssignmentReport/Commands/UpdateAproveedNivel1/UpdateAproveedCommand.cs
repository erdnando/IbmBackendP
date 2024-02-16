﻿using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
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
        private ICreateLogCommand _logCommand;

        public UpdateAproveedCommand(IDataBaseService dataBaseService, IMapper mapper, IEmailCommand emailCommand, IGetListUsuarioCommand usuarioCommand, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;
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

                    //Se maerca reporte como pendiente y en progreso en el flujo de aprobacion
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    currentHReport.EstatusFinal = "ENPROGRESO";
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
                     _dataBaseService.assignmentReports.AddAsync(entityN1);
                    await _dataBaseService.SaveAsync();

                    await _logCommand.Log(modelAprobador.UserId.ToString(), "ESTANDAR Aprueba reporte", modelAprobador);

                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {

                    //Se maerca reporte como pendiente
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    currentHReport.EstatusFinal = "ENPROGRESO";
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

                    await _logCommand.Log(modelAprobador.UserId.ToString(), "N1 Aprueba reporte", modelAprobador);


                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    //Se maerca reporte como aprobado N2
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    currentHReport.DateApprovalSystem = DateTime.Now;

                    switch (currentHReport.EstatusOrigen)
                    {
                        case "FINAL": currentHReport.EstatusFinal = "PREAPROBADO"; break;
                        case "SUBMITTED": currentHReport.EstatusFinal = "PREAPROBADO"; break;
                        case "EXTRACTED": currentHReport.EstatusFinal = "APROBADO"; break;
                        case "STANDBY": currentHReport.EstatusFinal = "APROBADO"; break;
                        case "STE": currentHReport.EstatusFinal = "APROBADO"; break;
                        default: currentHReport.EstatusFinal = "ENPROGRESO"; break;
                    }

                    
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


                    await _logCommand.Log(modelAprobador.UserId.ToString(), "N2 Aprueba reporte", modelAprobador);
                }

            }
            else if (modelAprobador.State == 1) //RECHAZADO
            {

                if (modelAprobador.roleAprobador == "Usuario estandar")
                {

                    //Se rechaza el reporte de hrs 
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    currentHReport.EstatusFinal = "RECHAZADO";
                    _dataBaseService.HorusReportEntity.Update(currentHReport);
                    await _dataBaseService.SaveAsync();

                    //TODO
                    //Evaluar si esta en excepcion de reportes extracted
                    // if it is EXTRACTED, please move it to APPROVED!!!!
                    //Asegurarse q en el historial (currentAssignment) aparezca q fue rechazado y despues agregar q fue promovido a APROBADO

                    //Se marca como atendida por el empleado
                    currentAssignment.State = 1;
                    currentAssignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.Nivel = 0;
                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();

                    await _logCommand.Log(modelAprobador.UserId.ToString(), "ESTANDAR Rechaza reporte", modelAprobador);




                } else if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    //Se rechaza el reporte de hrs 
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    currentHReport.EstatusFinal = "RECHAZADO";
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

                    await _logCommand.Log(modelAprobador.UserId.ToString(), "N1 Rechaza reporte", modelAprobador);

                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    //Se rechaza el reporte de hrs 
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    currentHReport.EstatusFinal = "RECHAZADO";
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

                    await _logCommand.Log(modelAprobador.UserId.ToString(), "N2 Rechaza reporte", modelAprobador);
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
                        /*
                         * _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.Aprobador2UserEntityId).Result).Email, Plantilla = "5" });
                       */
                        
                    }

                   
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.EmpleadoUserEntityId).Result).Email, Plantilla = "5" });
                    
                    
                }
                else
                {
                    
                        _emailCommand.SendEmail(new EmailModel { To = (_usuarioCommand.GetByUsuarioId(modelAprobador.EmpleadoUserEntityId).Result).Email, Plantilla = "3" }); 
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
