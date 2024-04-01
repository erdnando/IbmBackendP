using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.PortalDB.Commands;
using Algar.Hours.Application.DataBase.PortalDB.Commands.Create;
using Algar.Hours.Application.DataBase.PortalDBAproveHistory.Commands.Create;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.PortalDB;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using static Algar.Hours.Application.Enums.Enums;
using System.Diagnostics;
using System.Linq;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using DocumentFormat.OpenXml.Presentation;
using Algar.Hours.Domain.Entities.AssignmentReport;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Create
{
    public class CreateHorusReportCommand : ICreateHorusReportCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private IEmailCommand _emailCommand;
        private IGetListUsuarioCommand _usuarioCommand;
        private readonly IMapper _mapper;
        private ICreateLogCommand _logCommand;

        public CreateHorusReportCommand(IDataBaseService dataBaseService, IMapper mapper, IEmailCommand emailCommand, IGetListUsuarioCommand usuarioCommand, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _emailCommand = emailCommand;
            _usuarioCommand = usuarioCommand;
            _logCommand = logCommand;
        }

        public async Task<HorusReportModel> Execute(CreateHorusReportModel model)
        {
            //AQUI!!!!!
            var horusModel = _mapper.Map<HorusReportModel>(model);
         
            horusModel.IdHorusReport = Guid.NewGuid();
            //horusModel.StrCreationDate = DateTime.Now;
            horusModel.strCreationDate= DateTime.Now.ToString("dd/MM/yyyy HH:mm"); 
            horusModel.DateApprovalSystem = DateTime.Now;
            Boolean canSendAgainHours = false;

            var startTime = DateTime.Parse(model.StartTime);
            var endTime = DateTime.Parse(model.EndTime);
            var semanahorario = new DateTimeOffset();
            CultureInfo cul = CultureInfo.CurrentCulture;


            DateTime fechaHoraOriginal = DateTime.ParseExact(model.StartDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("yyyy-MM-dd 00:00:00");


            //TODO
            //Validar que el usuario tenga un horario existente
            // Si no tiene horario, generar notificacion y se cancela el registro d ehoras


            //obtine datos para validar si existe un registro previo (OVERLAPPING)
            var data = _dataBaseService.HorusReportEntity
                .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId== model.UserEntityId)
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, model.StartTime, model.EndTime) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                 TimeInRange(h.EndTime, startTime, endTime)))
                .ToList();


            if (data.Count != 0)
            {
                var horusReportRef = _mapper.Map<Domain.Entities.HorusReport.HorusReportEntity>(data[0]);
                var assignmentRef = _dataBaseService.assignmentReports.
                     Where(x => x.HorusReportEntityId == horusReportRef.IdHorusReport)
                     .FirstOrDefault();

                //si hay datos, valida si esta en estado Pendiente (3)
                //OVERLAPING permitido, por q ya esta APROBADO o RECHAZADO
                if (assignmentRef!.State == 3)
                {
                    //is pending
                    canSendAgainHours = false;
                }
                else
                {
                    // it has been approved or rejected
                    canSendAgainHours = true;
                }
                

            }
            else
            {
                canSendAgainHours = true;
            }



            if (!canSendAgainHours)
            {
                return null;
            }







            if (horusModel.ClientEntityId == Guid.Empty || string.IsNullOrEmpty(horusModel.ClientEntityId.ToString()))
            {
                horusModel.ClientEntityId =Guid.Parse("DC606C5A-149E-4F9B-80B3-BA555C7689B9");
            }


            var entity = _mapper.Map<HorusReportEntity>(horusModel);

            var horusreporcount = _dataBaseService.HorusReportEntity.Count();
            var Maxen = 0;

            if (horusreporcount > 0)
            {
                Maxen = _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
            }

            if (Maxen == 0)
            {
                Maxen = 1;
            }

            entity.NumberReport = Maxen + 1;
            entity.StrReport = "STANDBY00" + (Maxen + 1).ToString();
            entity.StrStartDate = nuevaFechaHoraFormato;// DateTime.Parse(nuevaFechaHoraFormato).Date;
            _dataBaseService.HorusReportEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();

            CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
            assignmentReport.IdAssignmentReport = Guid.NewGuid();
            assignmentReport.HorusReportEntityId = entity.IdHorusReport;
            //assignmentReport.UserEntityId = Guid.Parse(entity.ApproverId);
            assignmentReport.Description = horusModel.Description;
            assignmentReport.State = (byte)Enums.Enums.Aprobacion.Pendiente;

            var assigmentreportentity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
            _dataBaseService.assignmentReports.Add(assigmentreportentity);

            await _dataBaseService.SaveAsync();


            



            return horusModel;
        }

        public async Task<PortalDBModel> ExecutePortal(CreateHorusReportModel model)
        {

            PortalDBModel returnPortalDB = new();

            //---------------------------heredado------------------------------------------------------------------------------------
            var horusModel = _mapper.Map<HorusReportModel>(model);
            

            horusModel.IdHorusReport = Guid.NewGuid();//ok
            horusModel.strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            horusModel.DateApprovalSystem = DateTime.Now;
            Boolean canSendAgainHours = false;

            var startTime = DateTime.Parse(model.StartTime);
            var endTime = DateTime.Parse(model.EndTime);
            var semanahorario = new DateTimeOffset();
            CultureInfo cul = CultureInfo.CurrentCulture;


            DateTime fechaHoraOriginal = DateTime.ParseExact(model.StartDate.ToString(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");
            var dateTime = DateTime.ParseExact(model.StartDate.ToString(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            horusModel.StrStartDate = dateTime.ToString("dd/MM/yyyy");

            //--------------------------------------------------------------------------------------------------------------------------
            var parametrosCountry = _dataBaseService.UserEntity.Include("CountryEntity").FirstOrDefault(x => x.IdUser == horusModel.UserEntityId);

            var fechaReportada = DateTimeOffset.Parse(model.StartDate.ToString());
            var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();
            var HoraInicioReportado = DateTimeOffset.Parse(model.StartTime);
            var HoraFinReportado = DateTimeOffset.Parse(model.EndTime);

            int Semana = cul.Calendar.GetWeekOfYear(fechaReportada.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            //Obtiene horario para este empleado en la fecha del evento
            var fechaformateadaReporta = fechaReportada.DateTime.ToString("yyyy-MM-dd 00:00:00");
            var horarioAsignado = Lsthorario.FirstOrDefault(x => x.UserEntity.IdUser == model.UserEntityId && x.week == Semana.ToString() && x.Ano == fechaReportada.UtcDateTime.Year.ToString() && x.FechaWorking.UtcDateTime.DayOfWeek == fechaReportada.DayOfWeek );
             
            var isFestivo = _dataBaseService.FestivosEntity.Where(x => x.DiaFestivo == DateTimeOffset.ParseExact(fechaformateadaReporta, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) && x.CountryId == parametrosCountry.CountryEntityId).Count() > 0;
              
            //Validación del Horario reportado
            //================================================================================================================
            if (horarioAsignado != null && !isFestivo)
            {
                var validaHorario = ValidarHorarioEmployee(horarioAsignado, fechaReportada, HoraInicioReportado, HoraFinReportado);
                if (!validaHorario)
                {
                    // State 100 es por que trata de asignar un standby durante horario laboral
                    // agregar notificacion email
                    returnPortalDB.State = 100;
                    returnPortalDB.Error = true;
                    returnPortalDB.Message = "Error en el registro de horas, por tratar de asignar un horario standby durante horario laboral";
                    _emailCommand.SendEmail(new EmailModel
                    {
                        To = (await _usuarioCommand.GetByUsuarioId(new Guid(model.ApproverId.ToString()))).Email,
                        Plantilla = "14"
                    });
                    return returnPortalDB;
                }
            }
            /*else
            {
                // State 99 es por que no tiene horario para comparar
                // agregar notificacion email
                returnPortalDB.State = 99;
                _emailCommand.SendEmail(new EmailModel
                {
                    To = (await _usuarioCommand.GetByUsuarioId(new Guid(model.ApproverId.ToString()))).Email,
                    Plantilla = "8"
                });
                return returnPortalDB;
            }*/

            //Validacion de limites y excepciones
            //=================================================================================================================


            var limitesCountry = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == parametrosCountry.CountryEntityId && x.TypeHours==0);
            var HorasLimiteDia = limitesCountry.TargetTimeDay;
            var HorasLimiteSemanales = limitesCountry.TargetHourWeek;
            var HorasLimiteMensuales = limitesCountry.TargetHourMonth;
            var HorasLimiteAnuales = limitesCountry.TargetHourYear;

            TimeSpan tsReportado = HoraFinReportado - HoraInicioReportado;
            var listExeptios = _dataBaseService.UsersExceptions.ToList();
            var exceptionUser = listExeptios.FirstOrDefault(x => x.UserId == horusModel.UserEntityId && x.StartDate.UtcDateTime.ToString("dd/MM/yyyy") == horusModel.StrStartDate);
            var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;

            int[] status = [(byte)AprobacionPortalDB.AprobadoN0, (byte)AprobacionPortalDB.AprobadoN1, (byte)AprobacionPortalDB.AprobadoN2];
            var HorasPortalDBTDia = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{horusModel.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy")} 00:00', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy")} 23:59', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();
            var dateTimeInicioSemana = DateTime.ParseExact($"{dateTime.ToString("yyyy-MM-dd")} 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(-((int) dateTime.DayOfWeek));
            var dateTimeFinSemana = DateTime.ParseExact($"{dateTimeInicioSemana.ToString("yyyy-MM-dd")} 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(6);
            var HorasPortalDBTSemana = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{horusModel.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTimeInicioSemana.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTimeFinSemana.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();
            var dateTimeInicioMes = DateTime.ParseExact($"{dateTime.ToString("yyyy-MM-")}01 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            var dateTimeFinMes = DateTime.ParseExact($"{dateTimeInicioMes.ToString("yyyy-MM-")}{DateTime.DaysInMonth(dateTimeInicioMes.Year, dateTimeInicioMes.Month)} 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            var HorasPortalDBTMes = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{horusModel.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTimeInicioMes.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTimeFinMes.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();
            var dateTimeInicioAno = DateTime.ParseExact($"{dateTime.ToString("yyyy-")}01-01 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            var dateTimeFinAno = DateTime.ParseExact($"{dateTime.ToString("yyyy-")}12-31 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            var HorasPortalDBTAno = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h WHERE h.\"UserEntityId\" = '{horusModel.UserEntityId}' AND h.\"Estado\" IN ({string.Join(",", status)}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTimeInicioAno.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') <= TO_TIMESTAMP('{dateTimeFinAno.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI')").ToList().Select(x => double.Parse(x.CountHours)).Sum();
            
            var limiteDiaExcedido = (HorasLimiteDia != 0 && (tsReportado.TotalHours + HorasPortalDBTDia) > (HorasLimiteDia + horasExceptuada));
            var limiteSemanalExcedido = (HorasLimiteSemanales != 0 && (tsReportado.TotalHours + HorasPortalDBTSemana) > (HorasLimiteSemanales + horasExceptuada));
            var limiteMensualExcedido = (HorasLimiteMensuales != 0 && (tsReportado.TotalHours + HorasPortalDBTMes) > (HorasLimiteMensuales + horasExceptuada));
            var limiteAnualExcedido = (HorasLimiteAnuales != 0 && (tsReportado.TotalHours + HorasPortalDBTAno) > (HorasLimiteAnuales + horasExceptuada));
            if (limiteDiaExcedido || limiteSemanalExcedido || limiteMensualExcedido || limiteAnualExcedido) {
                var tipoLimite = limiteDiaExcedido? "diario" : (limiteSemanalExcedido ? "semanal" : (limiteMensualExcedido ? "mensual" : "anual"));
                //Se ha superado el límite de horas para StandBy
                // agregar notificacion email
                returnPortalDB.State = 101;
                returnPortalDB.Error = true;
                returnPortalDB.Message = $"Error en el registro de horas, el registro supera el límite {tipoLimite} de horas para StandBy";
                _emailCommand.SendEmail(new EmailModel
                {
                    To = (await _usuarioCommand.GetByUsuarioId(new Guid(model.ApproverId.ToString()))).Email,
                    Plantilla = "13"
                });
                return returnPortalDB;
            }



            //validacion OVERLAPPING
            //------------------------------------------------------------------------------------


            //obtine datos para validar si existe un registro previo (OVERLAPPING)
            var data = _dataBaseService.HorusReportEntity
                .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == model.UserEntityId && (h.EstatusFinal!="RECHAZADO" && h.EstatusFinal != "DESCARTADO"))
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, model.StartTime, model.EndTime) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                 TimeInRange(h.EndTime, startTime, endTime)))
                .ToList();


            if (data.Count > 0)
            {
                returnPortalDB.State = 102;
                returnPortalDB.Error = true;
                returnPortalDB.Message = "Error en el registro de horas, existe Overlaping";
                _emailCommand.SendEmail(new EmailModel
                {
                    To = (await _usuarioCommand.GetByUsuarioId(new Guid(model.ApproverId.ToString()))).Email,
                    Plantilla = "10"
                });
                return returnPortalDB;
            }
            



            //------------------------------------------Working on Horus report and assignment tables-----------------------------------------------------
            if (horusModel.ClientEntityId == Guid.Empty || string.IsNullOrEmpty(horusModel.ClientEntityId.ToString()))
            {
                horusModel.ClientEntityId = Guid.Parse("DC606C5A-149E-4F9B-80B3-BA555C7689B9");
            }


            var entityHorus = _mapper.Map<HorusReportEntity>(horusModel);
            var horusreporcount = _dataBaseService.HorusReportEntity.Count();


            var Maxen = 0;
            if (horusreporcount > 0)
            {
                Maxen = _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
            }

            if (Maxen == 0)
            {
                Maxen = 1;
            }

            if (horusModel.ApproverId == "53765c41-411f-4add-9034-7debaf04f276")//sistema
            {
                //============Pase directo a aprobado=========================================
                entityHorus.Acitivity = 0;//standby
                entityHorus.NumberReport = Maxen + 1;
                entityHorus.StrReport = "STANDBY00" + (Maxen + 1).ToString();
                entityHorus.StrStartDate = nuevaFechaHoraFormato;
                entityHorus.ARPLoadingId = "0";
                entityHorus.Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                entityHorus.EstatusOrigen = "STANDBY";
                entityHorus.EstatusFinal = "APROBADO";
                entityHorus.DetalleEstatusFinal = "";
                entityHorus.Origen = "STANDBY";
                entityHorus.Semana = getWeek(nuevaFechaHoraFormato);

                _dataBaseService.HorusReportEntity.AddAsync(entityHorus);


                //ASIGNMENT
                //=================================================================================
                var assignmentReport = new CreateAssignmentReportModel();
                assignmentReport.IdAssignmentReport = Guid.NewGuid();
                assignmentReport.HorusReportEntityId = entityHorus.IdHorusReport;
                assignmentReport.UserEntityId = Guid.Parse(horusModel.ApproverId);
                assignmentReport.TipoAssignment = "Approver";
                assignmentReport.Description = horusModel.Description + "(Aprobado por sistema)";
                assignmentReport.State = 1;
                assignmentReport.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                assignmentReport.Resultado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                assignmentReport.Nivel = 2;


                var assigmentreportentity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                _dataBaseService.assignmentReports.Add(assigmentreportentity);

                await _dataBaseService.SaveAsync();
            }
            else
            {
                entityHorus.Acitivity = 0;//standby
                entityHorus.NumberReport = Maxen + 1;
                entityHorus.StrReport = "STANDBY00" + (Maxen + 1).ToString();
                entityHorus.StrStartDate = nuevaFechaHoraFormato;
                entityHorus.ARPLoadingId = "0";
                entityHorus.Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN0;  
                entityHorus.EstatusOrigen = "STANDBY";
                entityHorus.EstatusFinal = "ENPROGRESO";
                entityHorus.DetalleEstatusFinal = "";
                entityHorus.Origen = "STANDBY";
                entityHorus.Semana = getWeek(nuevaFechaHoraFormato);


                _dataBaseService.HorusReportEntity.AddAsync(entityHorus);


                //ASIGNMENT
                //=================================================================================
                var assignmentReport = new CreateAssignmentReportModel();
                assignmentReport.IdAssignmentReport = Guid.NewGuid();
                assignmentReport.HorusReportEntityId = entityHorus.IdHorusReport;
                assignmentReport.UserEntityId = Guid.Parse(horusModel.ApproverId);
                assignmentReport.TipoAssignment = "Approver";
                assignmentReport.Description = horusModel.Description;
                assignmentReport.State = 0;
                assignmentReport.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                assignmentReport.Resultado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
                assignmentReport.Nivel = 0;


                var assigmentreportentity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                _dataBaseService.assignmentReports.Add(assigmentreportentity);

                await _dataBaseService.SaveAsync();


            }



            await _logCommand.Log(model.UserEntityId.ToString(), "Crea Reporte STANDBY", model);

            return returnPortalDB;
        }

        private string getWeek(string strStartDate)
        {
            try
            {
                CultureInfo cul = CultureInfo.CurrentCulture;
                var semanahorario = new DateTimeOffset();

                semanahorario = DateTimeOffset.ParseExact(strStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);


                return cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString();
            }
            catch (Exception)
            {

                return "0";
            }

        }

        private bool TimeRangesOverlap(string existingStartTime, string existingEndTime, string newStartTime, string newEndTime)
        {
            DateTime startTimeExisting = DateTime.Parse(existingStartTime);
            DateTime endTimeExisting = DateTime.Parse(existingEndTime);
            DateTime startTimeNew = DateTime.Parse(newStartTime);
            DateTime endTimeNew = DateTime.Parse(newEndTime);

            return (startTimeNew < endTimeExisting && endTimeNew > startTimeExisting);
        }

        private bool TimeInRange(string timeString, DateTime rangeStart, DateTime rangeEnd)
        {
            DateTime time = DateTime.Parse(timeString);
            return time >= rangeStart && time <= rangeEnd;
        }

        private bool ValidarHorarioEmployee(workinghoursEntity horarioAsigando, DateTimeOffset _fechaReportada, DateTimeOffset _HoraInicioReportado, DateTimeOffset _HoraFinReportado )
        {
            try
            {
                TimeSpan tsReportado = _HoraFinReportado - _HoraInicioReportado;
                TimeSpan tsAsignado = (DateTimeOffset.Parse(horarioAsigando.HoraFin) - DateTimeOffset.Parse(horarioAsigando.HoraInicio));
                double totalHoursReportadas = tsReportado.TotalHours;
                double totalHoursAsignadas = tsAsignado.TotalHours;
                if (_HoraInicioReportado.TimeOfDay < DateTimeOffset.Parse(horarioAsigando.HoraFin).TimeOfDay && DateTimeOffset.Parse(horarioAsigando.HoraInicio).TimeOfDay < _HoraFinReportado.TimeOfDay)
                {
                    return false;
                }
                /*else if (totalHoursReportadas >= totalHoursAsignadas)
                {
                    return false;
                }*/
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
    }
}
