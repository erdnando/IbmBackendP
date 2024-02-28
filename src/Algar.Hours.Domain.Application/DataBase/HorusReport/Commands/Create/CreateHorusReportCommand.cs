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
            //---------------------------heredado----------------------------------------------------------------
            var horusModel = _mapper.Map<HorusReportModel>(model);
            

            horusModel.IdHorusReport = Guid.NewGuid();//ok
            //horusModel.strCreationDate = DateTime.Now.ToString();
            horusModel.strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            horusModel.DateApprovalSystem = DateTime.Now;
            Boolean canSendAgainHours = false;

            var startTime = DateTime.Parse(model.StartTime);
            var endTime = DateTime.Parse(model.EndTime);
            var semanahorario = new DateTimeOffset();
            CultureInfo cul = CultureInfo.CurrentCulture;


            DateTime fechaHoraOriginal = DateTime.ParseExact(model.StartDate.ToString(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");
            horusModel.StrStartDate = DateTime.ParseExact(model.StartDate.ToString(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).ToString();


            //TODO
            //Validar que el usuario tenga un horario existente
            // Si no tiene horario, generar notificacion y se cancela el registro d ehoras
            //------------------------------------------------------------------------------------


            var fechaReportada = DateTimeOffset.Parse(model.StartDate.ToString());
            var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();
            var HoraInicioReportado = DateTimeOffset.Parse(model.StartTime);
            var HoraFinReportado = DateTimeOffset.Parse(model.EndTime);

            int Semana = cul.Calendar.GetWeekOfYear(fechaReportada.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            //Obtiene horario para este empleado en la fecha del evento
            var fechaformateadaReporta = fechaReportada.DateTime.ToString("yyyy-MM-dd 00:00:00");
            var horarioAsignado = Lsthorario.FirstOrDefault(x => x.UserEntity.IdUser == model.UserEntityId && x.week == Semana.ToString() && x.Ano == fechaReportada.DateTime.Year.ToString());


            //Validación del Horario reportado
            //================================================================================================================
            if (horarioAsignado != null)
            {
                var validaHorario = ValidarHorarioEmployee(horarioAsignado, fechaReportada, HoraInicioReportado, HoraFinReportado);
                if (!validaHorario)
                {
                    // State 100 es por que trata de asignar un standby durante horario laboral
                    // agregar notificacion email

                    returnPortalDB.State = 100;
                    _emailCommand.SendEmail(new EmailModel
                    {
                        To = (await _usuarioCommand.GetByUsuarioId(new Guid(model.ApproverId.ToString()))).Email,
                        Plantilla = "14"
                    });
                    return returnPortalDB;
                }
            }
            else
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
            }

            //Validacion de limites y excepciones
            //=================================================================================================================


            var parametrosCountry = _dataBaseService.UserEntity.Include("CountryEntity").FirstOrDefault(x => x.IdUser == horusModel.UserEntityId);
            var limitesCountry = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == parametrosCountry.CountryEntityId);
            var HorasLimiteDia = limitesCountry.TargetTimeDay;

            TimeSpan tsReportado = HoraFinReportado - HoraInicioReportado;
            var listExeptios = _dataBaseService.UsersExceptions.ToList();
            var exceptionUser = listExeptios.FirstOrDefault(x => x.UserId == horusModel.UserEntityId && x.StartDate.ToString("dd/MM/yyyy") == horusModel.StrStartDate);
            var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
            var infoQuery = _dataBaseService.assignmentReports.Include("HorusReportEntity").
                Where(op => (op.State == 0 || op.State == 1) &&  (op.HorusReportEntity.StrStartDate == horusModel.StrStartDate   && op.UserEntityId==horusModel.UserEntityId   )).ToList();

            

            var HorasPortalDBT = infoQuery.Select(x => double.Parse(x.HorusReportEntity.CountHours)).Sum();
            if (HorasLimiteDia != 0 && (tsReportado.TotalHours + HorasPortalDBT) > (HorasLimiteDia + horasExceptuada))
            {
                //Se ha superado el límite de horas para StandBy
                // agregar notificacion email
                returnPortalDB.State = 101;
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
                .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == model.UserEntityId)
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, model.StartTime, model.EndTime) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                 TimeInRange(h.EndTime, startTime, endTime)))
                .ToList();


            if (data.Count > 0)
            {
                returnPortalDB.State = 102;
                _emailCommand.SendEmail(new EmailModel
                {
                    To = (await _usuarioCommand.GetByUsuarioId(new Guid(model.ApproverId.ToString()))).Email,
                    Plantilla = "10"
                });
                return returnPortalDB;
            }
            



         


          


            //------------------------------------------heredado final-----------------------------------------------------
            if (horusModel.ClientEntityId == Guid.Empty || string.IsNullOrEmpty(horusModel.ClientEntityId.ToString()))
            {
                horusModel.ClientEntityId = Guid.Parse("DC606C5A-149E-4F9B-80B3-BA555C7689B9");
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
            entity.Acitivity = 0;//standby
            entity.NumberReport = Maxen + 1;
            entity.StrReport = "STANDBY00"+(Maxen + 1).ToString();
            entity.StrStartDate = nuevaFechaHoraFormato;
            entity.ARPLoadingId = "0";
            entity.Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;
            entity.EstatusOrigen = "STANDBY";
            entity.EstatusFinal = "ENPROGRESO";
            entity.DetalleEstatusFinal = "";


            _dataBaseService.HorusReportEntity.AddAsync(entity);
            //await _dataBaseService.SaveAsync();


            //ASIGNMENT
            //=================================================================================
            var assignmentReport = new CreateAssignmentReportModel();
            assignmentReport.IdAssignmentReport = Guid.NewGuid();
            assignmentReport.HorusReportEntityId = entity.IdHorusReport;
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



            await _logCommand.Log(model.UserEntityId.ToString(), "Crea Reporte STANDBY", model);


            return returnPortalDB;


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
                if (_HoraInicioReportado.TimeOfDay >= DateTimeOffset.Parse(horarioAsigando.HoraInicio).TimeOfDay && _HoraFinReportado.TimeOfDay <= DateTimeOffset.Parse(horarioAsigando.HoraFin).TimeOfDay)
                {
                    return false;
                }
                else if (totalHoursReportadas >= totalHoursAsignadas)
                {
                    return false;
                }
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
