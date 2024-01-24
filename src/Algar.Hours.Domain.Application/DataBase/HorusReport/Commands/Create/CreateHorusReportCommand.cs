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

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Create
{
    public class CreateHorusReportCommand : ICreateHorusReportCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateHorusReportCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<HorusReportModel> Execute(CreateHorusReportModel model)
        {
            //AQUI!!!!!
            var horusModel = _mapper.Map<HorusReportModel>(model);
         
            horusModel.IdHorusReport = Guid.NewGuid();
            horusModel.CreationDate = DateTime.Now;
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
                .Where(h => h.StartDate == DateTime.Parse(nuevaFechaHoraFormato) && h.UserEntityId== model.UserEntityId)
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
                //Si el OVERLAPPING es sobre un caso PENDIENTE, se cancela y/o actualiza y se genera uno nuevo

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
            entity.StartDate = DateTime.Parse(nuevaFechaHoraFormato).Date;
            _dataBaseService.HorusReportEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();

            CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
            assignmentReport.IdAssignmentReport = Guid.NewGuid();
            assignmentReport.HorusReportEntityId = entity.IdHorusReport;
            assignmentReport.UserEntityId = Guid.Parse(entity.ApproverId);
            assignmentReport.Description = horusModel.Description;
            assignmentReport.State = (byte)Enums.Enums.Aprobacion.Pendiente;

            var assigmentreportentity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
            _dataBaseService.assignmentReports.Add(assigmentreportentity);

            await _dataBaseService.SaveAsync();


            



            return horusModel;
        }

        public async Task<PortalDBModel> ExecutePortal(CreatePortalDBModel model)
        {
            PortalDBModel portalModel = new();
            //DateTime now = DateTime.Now;
            //DateTime then = DateTime.Now.AddHours(-7);

            //TimeSpan ts = now.Subtract(then);
            //var re = ts.TotalHours;

            
            try
            {
                portalModel = _mapper.Map<PortalDBModel>(model);
                //Extraemos Horarios
                //Busca horarios configurados para este empleado en la semana y dia
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();
                var fechaReportada = DateTimeOffset.Parse(model.StartDate.ToString());
                var HoraInicioReportado = DateTimeOffset.Parse(model.StartTime);
                var HoraFinReportado = DateTimeOffset.Parse(model.EndTime);

                CultureInfo cul = CultureInfo.CurrentCulture;
                int Semana = cul.Calendar.GetWeekOfYear(fechaReportada.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                //Obtiene horario para este empleado en la fecha del evento
                var fechaformateadaReporta = fechaReportada.DateTime.ToString("yyyy/MM/dd");
                var horarioAsignado = Lsthorario.FirstOrDefault(x => x.UserEntity.IdUser == model.UserEntityId && x.week == Semana.ToString() && x.FechaWorking.ToString("yyy/MM/dd") == fechaformateadaReporta);

                //Validación del Horario reportado
                if (horarioAsignado != null)
                {
                    var validaHorario = ValidarHorarioEmployee(horarioAsignado, fechaReportada, HoraInicioReportado, HoraFinReportado);
                    if (!validaHorario)
                    {
                        // State 100 es por que trata de asignar un standby durante horario laboral
                        // agregar notificacion email
                        portalModel.State = 100;
                        return portalModel;
                    }
                }
                else
                {
                    // State 99 es por que no tiene horario para comparar
                    // agregar notificacion email
                    portalModel.State = 99;
                    return portalModel;
                }

                //Validacion de limites

                var parametrosCountry = _dataBaseService.UserEntity.Include("CountryEntity").FirstOrDefault(x => x.IdUser == portalModel.UserEntityId);
                var limitesCountry = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == parametrosCountry.CountryEntityId);
                var HorasLimiteDia = limitesCountry.TargetTimeDay;

                TimeSpan tsReportado = HoraFinReportado - HoraInicioReportado;
                var exceptionUser = _dataBaseService.UsersExceptions.FirstOrDefault(x => x.AssignedUserId == portalModel.UserEntityId && x.StartDate == DateTimeOffset.Parse(portalModel.StartDate));
                var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
                var HorasPortalDB = _dataBaseService.PortalDBEntity.Where(co => co.StartDate== DateTime.Parse(portalModel.StartDate) && co.State ==0 || co.State== 1).ToList();
                var HorasPortalDBT= HorasPortalDB.Select(x => double.Parse(x.CountHours)).Sum();
                if ((tsReportado.TotalHours + HorasPortalDBT) > (HorasLimiteDia + horasExceptuada))
                {
                    //Se ha superado el límite de horas para StandBy
                    // agregar notificacion email
                    portalModel.State = 101;
                    return portalModel;
                }


                var rowGralPortalDB = _dataBaseService.PortalDBEntity.Where(op => op.StartDate == DateTime.Parse(portalModel.StartDate) && op.State == 0 || op.State == 1).Count();

                if (rowGralPortalDB>0)
                {
                    //el Registro se encuentra en Overlapi StandBy
                    // agregar notificacion email
                    portalModel.State = 102;
                    return portalModel;
                }




                //AQUI!!!!!


                portalModel.IdPortalDb = Guid.NewGuid();
                portalModel.CreationDate = DateTime.Now;
                Boolean canSendAgainHours = false;

                var startTime = DateTime.Parse(model.StartTime);
                var endTime = DateTime.Parse(model.EndTime);


                DateTime fechaHoraOriginal = DateTime.ParseExact(model.StartDate.ToString(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("yyyy-MM-dd HH:mm:ss");


                //TODO
                //Validar que el usuario tenga un horario existente
                // Si no tiene horario, generar notificacion y se cancela el registro d ehoras


                //obtine datos para validar si existe un registro previo (OVERLAPPING)
                var data = _dataBaseService.PortalDBEntity
                    .Where(h => h.StartDate == DateTime.Parse(nuevaFechaHoraFormato) && h.UserEntityId == model.UserEntityId)
                    .AsEnumerable()
                    .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, model.StartTime, model.EndTime) ||
                    (TimeInRange(h.StartTime, startTime, endTime) &&
                     TimeInRange(h.EndTime, startTime, endTime)))
                    .ToList();


                if (data.Count != 0)
                {
                    var horusReportRef = _mapper.Map<PortalDBEntity>(data[0]);
                    var assignmentRef = _dataBaseService.PortalDBAproveHistoryEntity.
                         Where(x => x.IdPortalDB == horusReportRef.IdPortalDb)
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
                    //Si el OVERLAPPING es sobre un caso PENDIENTE, se cancela y/o actualiza y se genera uno nuevo

                }
                else
                {
                    canSendAgainHours = true;
                }



                if (!canSendAgainHours)
                {
                    return null;
                }

                if (portalModel.ClientEntityId == Guid.Empty || string.IsNullOrEmpty(portalModel.ClientEntityId.ToString()))
                {
                    portalModel.ClientEntityId = Guid.Parse("DC606C5A-149E-4F9B-80B3-BA555C7689B9");
                }

                var entity = _mapper.Map<PortalDBEntity>(portalModel);

                var horusreporcount = _dataBaseService.PortalDBEntity.Count();
                var Maxen = 0;

                if (horusreporcount > 0)
                {
                    Maxen = _dataBaseService.PortalDBEntity.Max(x => x.NumberReport);
                }

                if (Maxen == 0)
                {
                    Maxen = 1;
                }

                entity.NumberReport = Maxen + 1;
                entity.StartDate = DateTimeOffset.Parse(model.StartDate).Date;
                _dataBaseService.PortalDBEntity.AddAsync(entity);
                await _dataBaseService.SaveAsync();

                CreatePortalDBAproveHistoryModel portalDBAproveHistoryReport = new CreatePortalDBAproveHistoryModel();
                portalDBAproveHistoryReport.IdPortalDBAproveHistory = Guid.NewGuid();
                portalDBAproveHistoryReport.IdPortalDB = portalModel.IdPortalDb;
                portalDBAproveHistoryReport.ApproverId = entity.ApproverId;
                portalDBAproveHistoryReport.Description = portalModel.Description;
                portalDBAproveHistoryReport.State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente;

                var portalDBAproveHistoryreportentity = _mapper.Map<PortalDBAproveHistoryEntity>(portalDBAproveHistoryReport);
                _dataBaseService.PortalDBAproveHistoryEntity.Add(portalDBAproveHistoryreportentity);

                await _dataBaseService.SaveAsync();




                return portalModel;
            }
            catch (Exception ex)
            {

                return null;
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
