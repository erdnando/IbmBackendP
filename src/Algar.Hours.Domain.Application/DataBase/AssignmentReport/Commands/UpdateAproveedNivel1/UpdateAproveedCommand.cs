using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.AssignmentReport;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubiety.Dns.Core.Records;
using static Algar.Hours.Application.Enums.Enums;

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
                    //Verificar limites Overtime
                    var dateTime = DateTime.ParseExact(currentHReport.StrStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string[] r1 = currentHReport.StartTime.Split(":");
                    string[] r2 = currentHReport.EndTime.Split(":");
                    TimeSpan tsReportado = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                    var parametrosCountry = _dataBaseService.UserEntity.Include("CountryEntity").FirstOrDefault(x => x.IdUser == currentHReport.UserEntityId);
                    var limitesCountry = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == parametrosCountry.CountryEntityId && x.TypeHours == 1);
                    var HorasLimiteDia = limitesCountry.TargetTimeDay;
                    var HorasLimiteSemanales = limitesCountry.TargetHourWeek;
                    var HorasLimiteMensuales = limitesCountry.TargetHourMonth;
                    var HorasLimiteAnuales = limitesCountry.TargetHourYear;

                    var listExeptios = _dataBaseService.UsersExceptions.ToList();
                    //var exceptionUser = listExeptios.FirstOrDefault(x => x.UserId == currentHReport.UserEntityId && x.StartDate.UtcDateTime.ToString("dd/MM/yyyy") == dateTime.ToString("dd/MM/yyyy"));
                    //var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;


                    var exceptionUserOr = listExeptios.Where(x => x.UserId == currentHReport.UserEntityId && x.StartDate.UtcDateTime.ToString("dd/MM/yyyy") == dateTime.ToString("dd/MM/yyyy") && x.ReportType.Trim().ToUpper().Equals(("OVERTIME").Trim().ToUpper())).ToList();
                    var exceptionUser = exceptionUserOr.Sum(op => op.horas);
                    var horasExceptuada = exceptionUser == 0 ? 0 : exceptionUser;

                    int[] status = [(byte)AprobacionPortalDB.AprobadoN0, (byte)AprobacionPortalDB.AprobadoN1, (byte)AprobacionPortalDB.AprobadoN2];
                    var HorasPortalDBTDia = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{currentHReport.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND h.\"IdHorusReport\" != '{currentHReport.IdHorusReport}' AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy")} 00:00', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy")} 23:59', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();
                    var dateTimeInicioSemana = DateTime.ParseExact($"{dateTime.ToString("yyyy-MM-dd")} 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(-((int)dateTime.DayOfWeek));
                    var dateTimeFinSemana = DateTime.ParseExact($"{dateTimeInicioSemana.ToString("yyyy-MM-dd")} 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(6);
                    var HorasPortalDBTSemana = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{currentHReport.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND h.\"IdHorusReport\" != '{currentHReport.IdHorusReport}' AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTimeInicioSemana.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTimeFinSemana.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();
                    var dateTimeInicioMes = DateTime.ParseExact($"{dateTime.ToString("yyyy-MM-")}01 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    var dateTimeFinMes = DateTime.ParseExact($"{dateTimeInicioMes.ToString("yyyy-MM-")}{DateTime.DaysInMonth(dateTimeInicioMes.Year, dateTimeInicioMes.Month)} 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    var HorasPortalDBTMes = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{currentHReport.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND h.\"IdHorusReport\" != '{currentHReport.IdHorusReport}' AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTimeInicioMes.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTimeFinMes.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();
                    var dateTimeInicioAno = DateTime.ParseExact($"{dateTime.ToString("yyyy-")}01-01 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    var dateTimeFinAno = DateTime.ParseExact($"{dateTime.ToString("yyyy-")}12-31 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    var HorasPortalDBTAno = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{currentHReport.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND h.\"IdHorusReport\" != '{currentHReport.IdHorusReport}' AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTimeInicioAno.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTimeFinAno.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();

                    var limiteDiaExcedido = (HorasLimiteDia != 0 && (tsReportado.TotalHours + HorasPortalDBTDia) > (HorasLimiteDia + horasExceptuada));
                    var limiteSemanalExcedido = (HorasLimiteSemanales != 0 && (tsReportado.TotalHours + HorasPortalDBTSemana) > (HorasLimiteSemanales + horasExceptuada));
                    var limiteMensualExcedido = (HorasLimiteMensuales != 0 && (tsReportado.TotalHours + HorasPortalDBTMes) > (HorasLimiteMensuales + horasExceptuada));
                    var limiteAnualExcedido = (HorasLimiteAnuales != 0 && (tsReportado.TotalHours + HorasPortalDBTAno) > (HorasLimiteAnuales + horasExceptuada));
                    if (limiteDiaExcedido || limiteSemanalExcedido || limiteMensualExcedido || limiteAnualExcedido)
                    {
                        var tipoLimite = limiteDiaExcedido ? "diario" : (limiteSemanalExcedido ? "semanal" : (limiteMensualExcedido ? "mensual" : "anual"));
                        modelAprobador.Error = true;
                        modelAprobador.Message = $"Error en el registro de horas, el registro supera el límite {tipoLimite} de horas para Overtime";
                        return modelAprobador;

                    }

                    //Se maerca reporte como pendiente y en progreso en el flujo de aprobacion
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN0;
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

                    //Se maerca reporte como aprobado N1
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1;
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
                    currentHReport.DetalleEstatusFinal = "RECHAZADO POR EL EMPLEADO";
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

                    await _logCommand.Log(modelAprobador.UserId.ToString(), "ESTANDAR Rechaza reporte", modelAprobador);




                } else if (modelAprobador.roleAprobador == "Usuario Aprobador N1")
                {
                    //Se rechaza el reporte de hrs 
                    currentHReport.Estado = (byte)Enums.Enums.AprobacionPortalDB.Rechazado;
                    currentHReport.DateApprovalSystem = DateTime.Now;
                    currentHReport.EstatusFinal = "RECHAZADO";
                    currentHReport.DetalleEstatusFinal = "RECHAZADO POR N1";
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
                    currentHReport.DetalleEstatusFinal = "RECHAZADO POR N2";
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

                // Excepciones de Reportes
                /*if ((modelAprobador.roleAprobador == "Usuario Aprobador N1" || modelAprobador.roleAprobador == "Usuario Aprobador N2")) {
                    var reportException = _dataBaseService.ReportExceptionEntity.
                        Where(x => x.Report == currentHReport.StrReport)
                        .FirstOrDefault();

                    if (reportException != null) {
                        currentHReport.EstatusFinal = "APROBADO";
                        currentHReport.Estado = currentAssignment.Nivel == 1 ? (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1 : (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                        _dataBaseService.HorusReportEntity.Update(currentHReport);
                        await _dataBaseService.SaveAsync();

                        reportException.ExceptionDate = DateTime.Now;
                        reportException.ExceptionUserEntityId = currentAssignment.UserEntityId;
                        _dataBaseService.ReportExceptionEntity.Update(reportException);
                        await _dataBaseService.SaveAsync();

                        var newAssignment = new Domain.Entities.AssignmentReport.AssignmentReport();
                        newAssignment.IdAssignmentReport = Guid.NewGuid();
                        newAssignment.UserEntityId = Guid.Parse("53765c41-411f-4add-9034-7debaf04f276");
                        newAssignment.HorusReportEntityId = currentHReport.IdHorusReport;
                        newAssignment.State = 1;
                        newAssignment.Resultado = currentAssignment.Nivel == 1? (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1 : (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                        newAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        newAssignment.Description = "APROBADO POR EXCEPCION";
                        newAssignment.Nivel = currentAssignment.Nivel;
                        _dataBaseService.assignmentReports.Add(newAssignment);
                        await _dataBaseService.SaveAsync();
                    }

                }*/
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
