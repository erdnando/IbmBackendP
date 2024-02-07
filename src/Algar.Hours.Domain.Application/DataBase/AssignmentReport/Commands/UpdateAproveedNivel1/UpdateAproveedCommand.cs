﻿using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
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

            var currentAssignment = _dataBaseService.assignmentReports.
                        Where(x => x.HorusReportEntityId == modelAprobador.HorusReportEntityId)
                        .FirstOrDefault();

            //0 APROBADO 
            if (modelAprobador.State == 0)
            {
               

                if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2; //<----APROBADO
                    currentAssignment.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.DateApprovalCancellation = DateTime.Now;
                    var entityreport = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(currentAssignment);

                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();

                    //nivel 1
                    CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
                    assignmentReport.UserEntityId = Guid.Parse(modelAprobador.Aprobador2UserEntityId.ToString());
                    assignmentReport.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentReport.State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
                    assignmentReport.Description = modelAprobador.Description;
                    assignmentReport.DateApprovalCancellation = DateTime.Now;
                                        
                    assignmentReport.IdAssignmentReport = Guid.NewGuid();

                    var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                    await _dataBaseService.assignmentReports.AddAsync(entity);
                    await _dataBaseService.SaveAsync();

                    //actualiza horus con el aprobador q aprobo
                    _dataBaseService.HorusReportEntity.Where(y => y.IdHorusReport == modelAprobador.HorusReportEntityId).ToList().ForEach(x => x.ApproverId = modelAprobador.Aprobador1UserEntityId.ToString());
                    await _dataBaseService.SaveAsync();
                }

                if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2; //<----APROBADO
                    currentAssignment.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.DateApprovalCancellation = DateTime.Now;
                    var entityreport = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(currentAssignment);

                    _dataBaseService.assignmentReports.Update(currentAssignment);
                    await _dataBaseService.SaveAsync();



                   
                    //se le crea al usuario su evidencia
                    CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
                    assignmentReport.UserEntityId = Guid.Parse(modelAprobador.EmpleadoUserEntityId.ToString());
                    assignmentReport.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentReport.State = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    assignmentReport.Description = modelAprobador.Description;
                    assignmentReport.DateApprovalCancellation = DateTime.Now;

                    //var response = CrearNivel2(assignmentReport);
                    assignmentReport.IdAssignmentReport = Guid.NewGuid();
                    var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                     _dataBaseService.assignmentReports.Add(entity);
                    await _dataBaseService.SaveAsync();

                    _dataBaseService.assignmentReports.Where(y => y.HorusReportEntityId == modelAprobador.HorusReportEntityId).ToList().ForEach(x => x.State = 2);
                    await _dataBaseService.SaveAsync();

                    //actualiza horus con el aprobador q aprobo
                    _dataBaseService.HorusReportEntity.Where(y => y.IdHorusReport == modelAprobador.HorusReportEntityId).ToList().ForEach(x => x.ApproverId = modelAprobador.Aprobador2UserEntityId.ToString());
                    await _dataBaseService.SaveAsync();
                }
                

            }
            else if (modelAprobador.State == 1)
            {
                //1 RECHAZADO
               
                    currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentAssignment.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    currentAssignment.Description = modelAprobador.Description;
                    currentAssignment.DateApprovalCancellation = DateTime.Now;

                    if(modelAprobador.roleAprobador == "Usuario Aprobador N1")
                    {
                        
                        _dataBaseService.assignmentReports.Where(y => y.HorusReportEntityId == modelAprobador.HorusReportEntityId).ToList().ForEach(x => x.State = 4);
                        await _dataBaseService.SaveAsync();

                        currentAssignment.UserEntityId = Guid.Parse(modelAprobador.Aprobador1UserEntityId.ToString());
                    }
                     
                    else if(modelAprobador.roleAprobador == "Usuario Aprobador N2")
                    {


                        _dataBaseService.assignmentReports.Where(y => y.HorusReportEntityId == modelAprobador.HorusReportEntityId).ToList().ForEach(x => x.State = 4);
                        await _dataBaseService.SaveAsync();

                        currentAssignment.UserEntityId = Guid.Parse(modelAprobador.Aprobador2UserEntityId.ToString());
                        currentAssignment.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                }

                _dataBaseService.assignmentReports.Update(currentAssignment);
                   await _dataBaseService.SaveAsync();



                    //se le crea al usuario su evidencia
                    CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();

                    assignmentReport.UserEntityId = Guid.Parse(modelAprobador.EmpleadoUserEntityId.ToString());
                    assignmentReport.HorusReportEntityId = modelAprobador.HorusReportEntityId;
                    assignmentReport.State = (byte)Enums.Enums.AprobacionPortalDB.Rechazado; //<----
                    assignmentReport.Description = modelAprobador.Description;
                    assignmentReport.DateApprovalCancellation = DateTime.Now;
                    assignmentReport.IdAssignmentReport = Guid.NewGuid();
                    var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                    _dataBaseService.assignmentReports.Add(entity);
                    await _dataBaseService.SaveAsync();



                if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    //actualiza horus con el aprobador q rechazo
                    _dataBaseService.HorusReportEntity.Where(y => y.IdHorusReport == modelAprobador.HorusReportEntityId).ToList().ForEach(x => x.ApproverId = modelAprobador.Aprobador1UserEntityId.ToString());
                    await _dataBaseService.SaveAsync();
                }
                else if (modelAprobador.roleAprobador == "Usuario Aprobador N2")
                {
                    //actualiza horus con el aprobador q rechazo
                    _dataBaseService.HorusReportEntity.Where(y => y.IdHorusReport == modelAprobador.HorusReportEntityId).ToList().ForEach(x => x.ApproverId = modelAprobador.Aprobador2UserEntityId.ToString());
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
