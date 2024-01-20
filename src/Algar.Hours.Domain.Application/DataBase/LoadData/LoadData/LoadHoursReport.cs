using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Algar.Hours.Domain.Entities.UsersExceptions;
using AutoMapper;
using EFCore.BulkExtensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Sustainsys.Saml2.Metadata;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public class LoadHoursReport : ILoadHoursReport
    {

        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public LoadHoursReport(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }


        public async Task<bool> Load(JsonArray model1, JsonArray model2, JsonArray model3)
        {
            var resultadoARP = await LoadARP(model1);
            var resultadoTSE = await LoadTSE(model2);
            var resultadoSTE = await LoadSTE(model3);

            return resultadoARP && resultadoTSE && resultadoSTE;
        }

        public async Task<bool> LoadARP(JsonArray model)
        {
            try
            {
                //deleting loads previous processing..
                 _dataBaseService.ParametersArpInitialEntity.ExecuteDelete();
                _dataBaseService.ARPLoadDetailEntity.ExecuteDelete();
                _dataBaseService.ARPLoadEntity.ExecuteDelete();
                await _dataBaseService.SaveAsync();

            }
            catch(Exception ex)
            {

            }


            try
            {

                #region Se registra la carga en ARPLoadEntity
                ARPLoadEntity aRPLoadEntity = new ARPLoadEntity
                {
                    Estado = 1,
                    FechaCreacion = DateTime.Now,
                    IdArpLoad = Guid.NewGuid(),
                    userEntityId = Guid.Parse("3696718D-D05A-4831-96CE-ED500C5BBC97")
                };

                await _dataBaseService.ARPLoadEntity.AddAsync(aRPLoadEntity);
                await _dataBaseService.SaveAsync();
                #endregion

                Int64 counter = 0;
                List<ParametersArpInitialEntity> listParametersInitialEntity = new();



                List<ARPLoadDetailEntity> convertModSerialize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ARPLoadDetailEntity>>(model.ToJsonString());
                convertModSerialize.Where(e => e.ESTADO.Trim() == "EXTRACTED").ToList().ForEach(x => x.ESTADO = "Extracted");
                convertModSerialize.Where(e => e.ESTADO.Trim() == "FINAL").ToList().ForEach(x => x.ESTADO = "Submitted");

                foreach (var itemSerialice in convertModSerialize)
                {
                    itemSerialice.IdDetail = Guid.Empty;
                    itemSerialice.IdDetail = Guid.NewGuid();
                    itemSerialice.ARPLoadEntityId = aRPLoadEntity.IdArpLoad;
                    if (!string.IsNullOrEmpty(itemSerialice.FECHA_REP))
                    {
                        try
                        {
                            string fecha = itemSerialice.FECHA_REP;
                            DateTimeOffset fechaRep;


                            if (DateTimeOffset.TryParseExact(fecha, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                            {
                                itemSerialice.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                            }
                            else if (DateTimeOffset.TryParseExact(fecha, "M/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                            {
                                itemSerialice.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                            }
                            else if (DateTimeOffset.TryParseExact(fecha, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                            {
                                itemSerialice.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                            }
                            else if (DateTimeOffset.TryParseExact(fecha, "MM/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                            {
                                itemSerialice.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                return false;
                            }



                        }
                        catch (FormatException)
                        {
                            if (DateTime.TryParseExact(itemSerialice.FECHA_REP, "MM/dd/yy", null, DateTimeStyles.None, out DateTime fechaConvertida) ||
                                DateTime.TryParseExact(itemSerialice.FECHA_REP, "M/d/yy", null, DateTimeStyles.None, out fechaConvertida))
                            {
                                itemSerialice.FECHA_REP = fechaConvertida.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                Console.WriteLine("No se pudo convertir la cadena de fecha");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(itemSerialice.FECHA_EXTRATED))
                    {
                        itemSerialice.FECHA_EXTRATED = itemSerialice.FECHA_REP;
                    }
                }

                await _dataBaseService.ARPLoadDetailEntity.AddRangeAsync(convertModSerialize);
                await _dataBaseService.SaveAsync();


                List<string> valOvertime = new() { "Vacations", "Absence", "Holiday", "Stand By" };

                var semanahorario = new DateTimeOffset();// arp.FECHA_REP;

                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();
                //var esfestivo = new FestivosEntity();
                List<FestivosEntity> esfestivos = new();

                //Para ARP, busca festivos de Colombia solamente
                esfestivos = _dataBaseService.FestivosEntity.Where(x => x.DiaFestivo == semanahorario && x.CountryId == new Guid("908465f1-4848-4c86-9e30-471982c01a2d")).ToList(); //&& x.CountryId == "");
                //esfestivos = _dataBaseService.FestivosEntity.ToList(); //&& x.CountryId == "");



                //int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                //Busca horarios configurados para este empleado en la semana y dia obtenido del excel de carga
                //var Lsthorario = _dataBaseService.workinghoursEntity.Where(x => x.week == Semana.ToString() && x.FechaWorking == semanahorario).ToList();
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").Where(x => x.UserEntity.EmployeeCode != null).ToList();




                #region por cada registro obtenido en el excel, se evalua
                foreach (var entity in convertModSerialize)
                {

                    #region evaluacion contra ARPLoadDetailEntity

                    var arp = entity;
                    //add validation fecha_rep
                    string fecha = arp.FECHA_REP;
                    DateTimeOffset fechaRep;


                    if (DateTimeOffset.TryParseExact(fecha, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        arp.FECHA_REP = fechaRep.ToString();
                    }
                    else if (DateTimeOffset.TryParseExact(fecha, "M/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        arp.FECHA_REP = fechaRep.ToString();
                    }
                    else if (DateTimeOffset.TryParseExact(fecha, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        arp.FECHA_REP = fechaRep.ToString();
                    }
                    else if (DateTimeOffset.TryParseExact(fecha, "MM/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        arp.FECHA_REP = fechaRep.ToString();
                    }
                    else if (DateTimeOffset.TryParseExact(fecha, "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        arp.FECHA_REP = fechaRep.ToString();
                    }


                    semanahorario = DateTimeOffset.Parse(arp.FECHA_REP);
                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    bool bValidacionHorario = false;

                    //Obtiene horario para este empleado en la fecha del evento
                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == arp.ID_EMPLEADO && x.week == Semana.ToString() && x.FechaWorking== semanahorario.DateTime);
                    
                    //var horario = _dataBaseService.workinghoursEntity.FirstOrDefault(x => x.UserEntity.EmployeeCode == arp.ID_EMPLEADO && x.week == Semana.ToString() && x.FechaWorking== semanahorario);

                    //Valida si el dia del evento es un festivo del pais Colombia
                    var esfestivo = esfestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario);


                    /*if (horario != null)
                    {

                        if (esfestivo == null)
                        {
                            fueraH = FueraHorario(arp.HORA_INICIO, arp.HORA_FIN, horario.HoraInicio, horario.HoraFin);
                            if (fueraH != null && fueraH.Count > 0)
                            {
                                foreach (var nquence in fueraH)
                                {

                                    QueuesAcceptanceEntity queuesAcceptanceEntity = new QueuesAcceptanceEntity();
                                    queuesAcceptanceEntity.IdQueuesAcceptanceEntity = Guid.NewGuid();
                                    queuesAcceptanceEntity.ARPLoadDetailEntityId = arp.IdDetail;
                                    queuesAcceptanceEntity.Id_empleado = arp.ID_EMPLEADO;
                                    queuesAcceptanceEntity.AprobadoSistema = DateTime.Now;
                                    queuesAcceptanceEntity.Hora_Inicio = nquence.Inicio;
                                    queuesAcceptanceEntity.Hora_Fin = nquence.Fin;
                                    queuesAcceptanceEntity.Horas_Total = nquence.Total;
                                    queuesAcceptanceEntity.FechaRe = arp.FECHA_REP;
                                    queuesAcceptanceEntity.Comentario = "";
                                    queuesAcceptanceEntity.Estado = (byte)Enums.Enums.Aprobacion.Aprobado;

                                }

                            }
                        }
                    }*/

                    


                    var previosAndPos = new List<double>();
                    if (horario != null)
                    {
                         previosAndPos = getPreviasAndPosHorario(arp.HORA_INICIO, arp.HORA_FIN, horario.HoraInicio, horario.HoraFin);
                        bValidacionHorario = true;
                    }
                    else
                    {
                        previosAndPos.Add(0.0);
                        previosAndPos.Add(0.0);
                        //Validacion horario NO Aplica este registro por que no tiene horario
                        bValidacionHorario = false;
                    }
                    
                        

                    ParametersArpInitialEntity parametersInitialEntity = new ParametersArpInitialEntity();

                    if (horario != null)
                    {
                        
                        parametersInitialEntity.IdParametersInitialEntity = Guid.NewGuid();
                        parametersInitialEntity.HoraInicio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                        parametersInitialEntity.HoraFin = horario.HoraFin == null ? "0" : horario.HoraFin;
                     
                        //agregar validacion para saber si tiene horas fuera de horario
                        parametersInitialEntity.OutIme = "N";

                        // agregar validación
                        parametersInitialEntity.OverTime = horario.HoraInicio == null ? "N" : valOvertime.IndexOf(arp.ACTIVIDAD.ToUpper()) >= 0 ? "N" : "S";

                        //agregar validación para HorasInicio
                        parametersInitialEntity.HorasInicio = (int)Math.Ceiling(previosAndPos[0]);

                        //agregar validacion para horasFin
                        parametersInitialEntity.HorasFin = (int)Math.Ceiling(previosAndPos[1]);
                    }
                    else
                    {
                        parametersInitialEntity.IdParametersInitialEntity = Guid.NewGuid();
                        parametersInitialEntity.HoraInicio = "0";
                        parametersInitialEntity.HoraFin = "0";
                       

                        //agregar validacion para saber si tiene horas fuera de horario
                        parametersInitialEntity.OutIme = "N";

                        // agregar validación
                        parametersInitialEntity.OverTime ="N";

                        //agregar validación para HorasInicio
                        parametersInitialEntity.HorasInicio = 0;

                        //agregar validacion para horasFin
                        parametersInitialEntity.HorasFin = 0;
                    }


                    parametersInitialEntity.Semana = Semana;
                    parametersInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                    parametersInitialEntity.ARPLoadDetailEntityId = arp.IdDetail;
                    parametersInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";




                    listParametersInitialEntity.Add(parametersInitialEntity);

                }//end for
                #endregion

                #endregion
                _dataBaseService.ParametersArpInitialEntity.AddRange(listParametersInitialEntity);//EF
                await _dataBaseService.SaveAsync();
                

                //_dataBaseService.BulkInsert(listParametersInitialEntity.ToList());
                //await _dataBaseService.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public class TimeRange
        {
            public TimeSpan Start { get; }
            public TimeSpan End { get; }

            public TimeRange(TimeSpan start, TimeSpan end)
            {
                if (end < start)
                {
                    throw new ArgumentException("El tiempo de inicio debe ser menor que el tiempo de fin.");
                }

                Start = start;
                End = end;
            }

            public bool overlap(TimeRange other)
            {
                return other.Start != Start || End != other.End;
            }


            public bool TimeInRange(TimeRange other)
            {
                return (other.Start <= Start && End <= other.End) || (Start < other.End && other.Start < End && Start != other.End && End != other.Start);
            }
        }


        public List<HorarioReturn> FueraHorario(string horarexcelinicio, string horarexcelfinal, string iniciohorario, string finhorario)
        {
            var nuevohorarioiniciotime = DateTimeOffset.Parse(horarexcelinicio);
            var nuevohorariofntime = DateTimeOffset.Parse(horarexcelfinal);
            var fechainicio = DateTimeOffset.Parse(iniciohorario);
            var fechafin = DateTimeOffset.Parse(finhorario);
            double hours1 = 0;
            double hours2 = 0;
            List<HorarioReturn> hours = new List<HorarioReturn>();


            if ((nuevohorarioiniciotime < fechainicio))
            {
                HorarioReturn Teamhours = new HorarioReturn();
                TimeSpan tsinicio = nuevohorarioiniciotime.Subtract(fechainicio);
                hours1 = tsinicio.TotalHours;
                Teamhours.Inicio = nuevohorarioiniciotime.ToString();
                Teamhours.Fin = nuevohorariofntime.ToString();
                Teamhours.Total = hours1;
                hours.Add(Teamhours);

            }

            if (nuevohorariofntime > fechafin)
            {
                HorarioReturn Teamhoursfin = new HorarioReturn();
                TimeSpan tsfin = nuevohorariofntime.Subtract(fechafin);
                hours2 = tsfin.TotalHours;
                Teamhoursfin.Inicio = nuevohorarioiniciotime.ToString();
                Teamhoursfin.Fin = nuevohorariofntime.ToString();
                Teamhoursfin.Total = hours2;
                hours.Add(Teamhoursfin);
            }
            return hours;
        }

        public List<double> getPreviasAndPosHorario(string horarexcelinicio, string horarexcelfinal, string iniciohorario, string finhorario)
        {
            var nuevohorarioiniciotime = DateTimeOffset.Parse(horarexcelinicio);
            var nuevohorariofntime = DateTimeOffset.Parse(horarexcelfinal);
            var fechainicio = DateTimeOffset.Parse(iniciohorario);
            var fechafin = DateTimeOffset.Parse(finhorario);
            double hours1 = 0;
            double hours2 = 0;
            var hours = new List<double>();


            if ((nuevohorarioiniciotime < fechainicio))
            {
                HorarioReturn Teamhours = new HorarioReturn();
                TimeSpan tsinicio = nuevohorarioiniciotime.Subtract(fechainicio);
                hours1 = tsinicio.TotalHours;
                Teamhours.Inicio = nuevohorarioiniciotime.ToString();
                Teamhours.Fin = nuevohorariofntime.ToString();
                Teamhours.Total = hours1;
                
                hours.Add(Math.Abs(hours1));

            }
            else { hours.Add(0.0); }

            if (nuevohorariofntime > fechafin)
            {
                HorarioReturn Teamhoursfin = new HorarioReturn();
                TimeSpan tsfin = nuevohorariofntime.Subtract(fechafin);
                hours2 = tsfin.TotalHours;
                Teamhoursfin.Inicio = nuevohorarioiniciotime.ToString();
                Teamhoursfin.Fin = nuevohorariofntime.ToString();
                Teamhoursfin.Total = hours2;
                if (hours1 < 0) hours1 = hours1 * -1;
                hours.Add(Math.Abs(hours2)); ;
            }
            else
            {
                hours.Add(0.0);
            }
            return hours;
        }

        public void Overload(string horainicio, string horafin, string idempleado, string fechareps)
        {
            var startTime = DateTime.Parse(horainicio);
            var endTime = DateTime.Parse(horafin);

            //consultar  QueuesAcceptanceEntity en arrp 
            var ArpConsult = _dataBaseService.QueuesAcceptanceEntity.Where(h => h.Id_empleado == idempleado && h.FechaRe == fechareps && TimeRangesOverlap(h.Hora_Inicio, h.Hora_Fin, horainicio, horafin) ||
                (TimeInRange(h.Hora_Inicio, startTime, endTime) &&
                 TimeInRange(h.Hora_Fin, startTime, startTime))).ToList();

            //consultar  QueuesAcceptanceEntityTSE en tse
            var TseConsult = _dataBaseService.QueuesAcceptanceEntityTSE.Where(h => h.Id_empleado == idempleado && h.FechaRe == fechareps && TimeRangesOverlap(h.Hora_Inicio, h.Hora_Fin, horainicio, horafin) ||
                (TimeInRange(h.Hora_Inicio, startTime, endTime) &&
                 TimeInRange(h.Hora_Fin, startTime, startTime))).ToList();

            // consultar QueuesAcceptanceEntitySTE en ste 
            var SteConsult = _dataBaseService.QueuesAcceptanceEntitySTE.Where(h => h.Id_empleado == idempleado && h.FechaRe == fechareps && TimeRangesOverlap(h.Hora_Inicio, h.Hora_Fin, horainicio, horafin) ||
                (TimeInRange(h.Hora_Inicio, startTime, endTime) &&
                 TimeInRange(h.Hora_Fin, startTime, startTime))).ToList();

            if ((ArpConsult.Count == 0 && TseConsult.Count == 0 && SteConsult.Count == 0) || (ArpConsult.Count != 0 && TseConsult.Count != 0))
            {

                var limitsCountry = ConsultarLimities();

                var horus = _dataBaseService.HorusReportEntity.Where(x => x.UserEntityId == Guid.NewGuid()).Sum(i => double.Parse(i.CountHours));




                consultUserExeptions();

                HorusReportEntity horusReportEntity = new HorusReportEntity();

                //horusReportEntity.Acitivity = 1;
                //horusReportEntity.ClientEntity = null;
                //horusReportEntity.Acitivity = 1;
                //horusReportEntity.UserEntity = null;
                //horusReportEntity.ApproverId = Guid.NewGuid().ToString();
                //horusReportEntity.NumberReport = 1;
                //horusReportEntity.StartDate = DateTime.Now;
                //horusReportEntity.EndTime = DateTime.Now;
                //horusReportEntity.ApproverId


            }
            else
            {

                //ActualizarRegister();


            }
            //verificar que si se sobrepone alguna hora y enviar el correo electronico 
            //si tiene la misma hora y fecha y id empleado se actualiza la hora

            //en caso que no se almacena en la base de datos
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

        public UsersExceptions consultUserExeptions()
        {
            return _dataBaseService.UsersExceptions.Where(x => x.IdUsersExceptions == Guid.NewGuid()).FirstOrDefault();

        }
        public void ActualizarRegister(string horainicio, string horafin)
        {


        }
        public ParametersEntity ConsultarLimities()
        {
            var Limites = _dataBaseService.ParametersEntity.Where(x => x.CountryEntityId == Guid.NewGuid()).FirstOrDefault();
            return Limites;
        }

        public async Task<bool> LoadTSE(JsonArray model)
        {
            try
            {
                //deleting loads previous processing..
                _dataBaseService.TSELoadEntity.ExecuteDelete();
                _dataBaseService.ParametersTseInitialEntity.ExecuteDelete();
                await _dataBaseService.SaveAsync();

            }
            catch (Exception ex)
            {

            }
            try
            {
                List<TSELoadEntity> convertModSerialize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TSELoadEntity>>(model.ToJsonString());
                convertModSerialize.Where(e => e.Status.Trim() == "EXTRACTED").ToList().ForEach(x => x.Status = "Extracted");
                convertModSerialize.Where(e => e.Status.Trim() == "SUBMITTED").ToList().ForEach(x => x.Status = "Submitted");
                convertModSerialize.Where(e => string.IsNullOrEmpty(e.AccountCMRNumber) == true || e.AccountCMRNumber == "N/A").ToList().ForEach(x => x.AccountCMRNumber = "1234");
                int counter = 0;
                foreach (var item in convertModSerialize)
                {

                    counter++;
                    try
                    {
                        var convert = item;
                        convert.IdTSELoad = Guid.NewGuid();

                        DateTimeOffset dateTimeOffset = new DateTimeOffset();
                        var StartTimels = DateTimeOffset.TryParseExact(item.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset).ToString();

                        if (convert.EndTime == null)
                        {
                            convert.EndTime = DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset).ToString();
                        }
                        if (convert.StartTime == null)
                        {
                            convert.StartTime = DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset).ToString();
                        }
                        if (string.IsNullOrEmpty(convert.StartHours))
                        {
                            try
                            {
                                var dt = new DateTimeOffset();
                                if (convert.StartTime == null)
                                {
                                    convert.StartHours = "00:00";
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.StartTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.StartHours = dt.ToString("HH:mm");
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            catch (Exception ex)
                            {
                                return false;
                            }
                        }
                        if (string.IsNullOrEmpty(convert.EndHours))
                        {
                            try
                            {
                                var dt = new DateTimeOffset();
                                if (convert.EndTime == "False")
                                {
                                    convert.EndHours = "00:00";
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                {
                                    convert.EndHours = dt.ToString("HH:mm");
                                }
                                else
                                {
                                    return false;
                                }


                            }
                            catch (Exception ex)
                            {
                                return false;
                            }

                        }

                        if (!string.IsNullOrEmpty(convert.ZonaHoraria))
                        {
                            var startTimeC = false;
                            var EndTimeC = false;
                            var dateTimeOffset2 = new DateTimeOffset();
                            var dateTimeOffset3 = new DateTimeOffset();
                            string trimmedValue = "";
                            try
                            {


                                startTimeC = DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset2);
                                EndTimeC = DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset3);

                            }
                            catch (Exception e)
                            {
                                return false;
                            }


                            try
                            {
                                string pattern2 = @"\(([^)]+\/[^)]+)\)";

                                Match match2 = Regex.Match(convert.ZonaHoraria, pattern2);

                                if (match2.Success)
                                {
                                    trimmedValue = match2.Groups[1].Value;
                                }

                                if (convert.ZonaHoraria == "(GMT+00:00) hora del meridiano de Greenwich (GMT)")
                                {
                                    trimmedValue = "GMT";
                                }
                            }
                            catch
                            {
                                return false;
                            }

                            TimeZoneInfo otrazonaDestino;
                            TimeZoneInfo otrazonaOrigen;
                            var HoraInicioDestino = new DateTimeOffset();
                            var HoraInicioOrigen = new DateTimeOffset();
                            var zonaDeprocedencia = convert.ZonaHoraria;
                            try
                            {
                                otrazonaDestino = TimeZoneInfo.FindSystemTimeZoneById(trimmedValue);
                                otrazonaOrigen = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");
                                try
                                {
                                    HoraInicioDestino = TimeZoneInfo.ConvertTime(dateTimeOffset2, otrazonaDestino);
                                    HoraInicioOrigen = TimeZoneInfo.ConvertTime(dateTimeOffset3, otrazonaOrigen);
                                }
                                catch (ArgumentNullException e)
                                {
                                    //return $"Error: uno de los argumentos proporcionados es nulo. Detalles: {e.Message}";
                                    return false;
                                }
                                catch (TimeZoneNotFoundException e)
                                {
                                    //return $"Error: no se pudo encontrar la zona horaria especificada. Detalles: {e.Message}";
                                    return false;
                                }
                                catch (InvalidTimeZoneException e)
                                {
                                    //return $"Error: la zona horaria especificada no es válida. Detalles: {e.Message}";
                                    return false;
                                }
                                catch (Exception e)
                                {
                                    //return $"Error inesperado: {e.Message}";
                                    return false;
                                }
                            }
                            catch (TimeZoneNotFoundException)
                            {
                                var lista = new List<string>();
                                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                                foreach (TimeZoneInfo timeZone in timeZones)
                                {
                                    lista.Add(timeZone.Id);
                                }

                                string listaComoCadena = string.Join(", ", lista);
                                //return $"Hola mundo cruel, estas son las zonas: {listaComoCadena}";
                                return false;
                            }
                            catch (InvalidTimeZoneException)
                            {
                                //return "La zona horaria especificada no es válida.";
                                return false;
                            }

                            try
                            {
                                var hours = (HoraInicioDestino - HoraInicioOrigen).TotalHours;
                                var horaRealIni = dateTimeOffset2.AddHours(hours);
                                var HoraRealFin = dateTimeOffset3.AddHours(hours);

                                convert.HoraInicio = horaRealIni.ToString();
                                convert.HoraFin = HoraRealFin.ToString();
                            }
                            catch
                            {
                                //return "cae en lo ultimo ";
                                return false;
                            }

                        }

                        var fechaRegistro = new DateTimeOffset();

                        DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRegistro);

                        convert.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy");
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }

                await _dataBaseService.TSELoadEntity.AddRangeAsync(convertModSerialize);
                await _dataBaseService.SaveAsync();


                var semanahorario = new DateTimeOffset();

                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();
                List<FestivosEntity> esfestivos = new();
                esfestivos = _dataBaseService.FestivosEntity.ToList(); //&& x.CountryId == "");
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").Where(x => x.UserEntity.EmployeeCode != null).ToList();

                List<ParametersTseInitialEntity> listParametersInitialEntity = new();

                foreach (var tse in convertModSerialize)
                {
                    semanahorario = DateTimeOffset.Parse(tse.StartTime);
                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString() && x.FechaWorking== semanahorario.DateTime);
                    //var horario = _dataBaseService.workinghoursEntity.FirstOrDefault(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString() && x.FechaWorking == semanahorario);
                    var esfestivo = esfestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario);

                    if (horario != null)
                    {
                        if (esfestivo == null)
                        {
                            fueraH = FueraHorario(tse.StartHours, tse.EndHours, horario.HoraInicio, horario.HoraFin);
                            if (fueraH != null && fueraH.Count > 0)
                            {
                                foreach (var aceepent in fueraH)
                                {

                                    QueuesAcceptanceEntityTSE queuesAcceptanceEntity = new QueuesAcceptanceEntityTSE();
                                    queuesAcceptanceEntity.IdQueuesAcceptanceEntityTSE = Guid.NewGuid();
                                    queuesAcceptanceEntity.TSELoadEntityId = tse.IdTSELoad;
                                    queuesAcceptanceEntity.Id_empleado = tse.NumeroEmpleado;
                                    queuesAcceptanceEntity.AprobadoSistema = DateTime.Now;
                                    queuesAcceptanceEntity.Hora_Inicio = aceepent.Inicio;
                                    queuesAcceptanceEntity.Hora_Fin = aceepent.Fin;
                                    queuesAcceptanceEntity.Horas_Total = aceepent.Total;
                                    queuesAcceptanceEntity.Comentario = "";
                                    queuesAcceptanceEntity.Estado = 1;
                                    queuesAcceptanceEntity.FechaRe = tse.StartTime;

                                }

                            }
                        }
                    }

                    ParametersTseInitialEntity parametersTseInitialEntity = new ParametersTseInitialEntity();

                    parametersTseInitialEntity.IdParamTSEInitialId = Guid.NewGuid();
                    if (horario != null)
                    {
                        parametersTseInitialEntity.HoraInicio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                        parametersTseInitialEntity.HoraFin = horario.HoraFin == null ? "0" : horario.HoraFin;
                        parametersTseInitialEntity.Estado = horario.HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";

                    }
                    else
                    {
                        workinghoursEntity workinghoursEntity = new workinghoursEntity();
                        workinghoursEntity.HoraInicio = "0";
                        workinghoursEntity.HoraFin = "0";

                        parametersTseInitialEntity.HoraInicio = workinghoursEntity.HoraInicio;
                        parametersTseInitialEntity.HoraFin = workinghoursEntity.HoraFin;

                    }


                    parametersTseInitialEntity.OutIme = fueraH.Count > 0 ? "Y" : "N";
                    parametersTseInitialEntity.OverTime = fueraH.Count > 0 ? "Y" : "N";
                    parametersTseInitialEntity.Semana = Semana;
                    parametersTseInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                    parametersTseInitialEntity.TSELoadEntityIdTSELoad = tse.IdTSELoad;
                    parametersTseInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";

                    listParametersInitialEntity.Add(parametersTseInitialEntity);

                }

                await _dataBaseService.ParametersTseInitialEntity.AddRangeAsync(listParametersInitialEntity);
                await _dataBaseService.SaveAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;

            }
        }



        public async Task<bool> LoadSTE(JsonArray model)
        {
            if (model == null || model.Count == 0)
            {
                return false;
            }

            try
            {
                //deleting loads previous processing..
                _dataBaseService.STELoadEntity.ExecuteDelete();
                _dataBaseService.ParametersSteInitialEntity.ExecuteDelete();
                await _dataBaseService.SaveAsync();

            }
            catch (Exception ex)
            {

            }

            try
            {
                List<STELoadEntity> convertModSerialize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STELoadEntity>>(model.ToJsonString());
                convertModSerialize.Where(e => string.IsNullOrEmpty(e.AccountCMRNumber) == true).ToList().ForEach(x => x.AccountCMRNumber = "1234");

                foreach (var entity in convertModSerialize)
                {
                    var convert = entity;
                    convert.IdSTELoad = Guid.NewGuid();
                    if (string.IsNullOrEmpty(convert.StartHours))
                    {

                        if (string.IsNullOrEmpty(convert.StartDateTime))
                        {
                            return false;
                        }

                        DateTimeOffset fecha;

                        if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.StartHours = fecha.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.StartHours = fecha.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.StartHours = fecha.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.StartHours = fecha.ToString("HH:mm");
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (string.IsNullOrEmpty(convert.EndHours))
                    {

                        if (string.IsNullOrEmpty(convert.EndDateTime))
                        {
                            return false;
                        }
                        DateTimeOffset fecha;

                        if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.EndHours = fecha.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.EndHours = fecha.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.EndHours = fecha.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            convert.EndHours = fecha.ToString("HH:mm");
                        }
                        else
                        {
                            return false;
                        }
                    }

                    var fechaRegistro = new DateTimeOffset();

                    DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRegistro);

                    convert.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy");
                }

                await _dataBaseService.STELoadEntity.AddRangeAsync(convertModSerialize);
                await _dataBaseService.SaveAsync();

                var semanahorario = new DateTimeOffset();// arp.FECHA_REP;

                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();
                List<FestivosEntity> esfestivos = new();
                esfestivos = _dataBaseService.FestivosEntity.ToList(); //&& x.CountryId == "");
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").Where(x => x.UserEntity.EmployeeCode != null).ToList();

                List<ParametersSteInitialEntity> listParametersInitialEntity = new();

                foreach (var ste in convertModSerialize)
                {
                    semanahorario = DateTimeOffset.Parse(ste.StartDateTime);
                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == ste.AccountCMRNumber && x.week == Semana.ToString() && x.FechaWorking== semanahorario.DateTime);
                    //var horario = _dataBaseService.workinghoursEntity.FirstOrDefault(x => x.UserEntity.EmployeeCode == ste.AccountCMRNumber && x.week == Semana.ToString() && x.FechaWorking == semanahorario);
                    var esfestivo = esfestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario);

                    if (string.IsNullOrEmpty(ste.StartDateTime))
                    {
                        return false;
                    }

                    if (horario != null)
                    {
                        esfestivo = _dataBaseService.FestivosEntity.Where(x => x.DiaFestivo == semanahorario).FirstOrDefault();
                        if (esfestivo == null)
                        {
                            fueraH = FueraHorario(ste.StartHours, ste.EndHours, horario.HoraInicio, horario.HoraFin);
                            if (fueraH != null && fueraH.Count > 0)
                            {
                                foreach (var aceepent in fueraH)
                                {

                                    QueuesAcceptanceEntitySTE queuesAcceptanceEntity = new QueuesAcceptanceEntitySTE();
                                    queuesAcceptanceEntity.IdQueuesAcceptanceEntitySTE = Guid.NewGuid();
                                    queuesAcceptanceEntity.STELoadEntityId = ste.IdSTELoad;
                                    queuesAcceptanceEntity.Id_empleado = ste.AccountCMRNumber;
                                    queuesAcceptanceEntity.AprobadoSistema = DateTime.Now;
                                    queuesAcceptanceEntity.Hora_Inicio = aceepent.Inicio;
                                    queuesAcceptanceEntity.Hora_Fin = aceepent.Fin;
                                    queuesAcceptanceEntity.Horas_Total = aceepent.Total;
                                    queuesAcceptanceEntity.Comentario = "";
                                    queuesAcceptanceEntity.Estado = 1;
                                    queuesAcceptanceEntity.FechaRe = ste.StartDateTime.ToString();

                                }

                            }
                        }
                    }

                    ParametersSteInitialEntity parametersTseInitialEntity = new ParametersSteInitialEntity();

                    parametersTseInitialEntity.IdParamSTEInitialId = Guid.NewGuid();
                    if (horario != null)
                    {
                        parametersTseInitialEntity.HoraInicio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                        parametersTseInitialEntity.HoraFin = horario.HoraFin == null ? "0" : horario.HoraFin;
                        parametersTseInitialEntity.Estado = horario.HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";

                    }
                    else
                    {
                        workinghoursEntity workinghoursEntity = new workinghoursEntity();
                        workinghoursEntity.HoraInicio = "0";
                        workinghoursEntity.HoraFin = "0";

                        parametersTseInitialEntity.HoraInicio = workinghoursEntity.HoraInicio;
                        parametersTseInitialEntity.HoraFin = workinghoursEntity.HoraFin;

                    }


                    parametersTseInitialEntity.OutIme = fueraH.Count > 0 ? "Y" : "N";
                    parametersTseInitialEntity.OverTime = fueraH.Count > 0 ? "Y" : "N";
                    parametersTseInitialEntity.Semana = Semana;
                    parametersTseInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                    parametersTseInitialEntity.STELoadEntityId = ste.IdSTELoad;
                    parametersTseInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";
                    listParametersInitialEntity.Add(parametersTseInitialEntity);
                }

                await _dataBaseService.ParametersSteInitialEntity.AddRangeAsync(listParametersInitialEntity);
                await _dataBaseService.SaveAsync();

                //proceso de validacion OverLAPI
                var rowARPGral = _dataBaseService.ARPLoadDetailEntity.ToList();
                var rowSTEGral = _dataBaseService.STELoadEntity.ToList();
                var rowTSEGral = _dataBaseService.TSELoadEntity.ToList();

                var rowselect = from roArp in rowARPGral
                                join roSTE in rowSTEGral on roArp.ID_EMPLEADO equals roSTE.AccountCMRNumber
                                join roTSE in rowTSEGral on roSTE.AccountCMRNumber equals roTSE.AccountCMRNumber
                                select new
                                {
                                    CodeUser = roArp.ID_EMPLEADO,
                                    Nombre = roArp.NOMBRE_CLIENTE,
                                    Categoria = roArp.CATEGORIA
                                };

                //var rowselect = from roArp in _dataBaseService.ARPLoadDetailEntity
                //                join roTSE in _dataBaseService.TSELoadEntity on roArp.ID_EMPLEADO equals roTSE.NumeroEmpleado
                //                select new
                //                {
                //                    CodeUser = roArp.ID_EMPLEADO,
                //                    Nombre = roArp.NOMBRE_CLIENTE,
                //                    Categoria = roArp.CATEGORIA
                //                };



                //var rowselectSTE = from roArp in _dataBaseService.ARPLoadDetailEntity
                //                join roSTE in _dataBaseService.STELoadEntity on roArp.ID_EMPLEADO equals roSTE.SessionEmployeeSerialNumber
                //                select new
                //                {
                //                    CodeUser = roArp.ID_EMPLEADO,
                //                    Nombre = roArp.NOMBRE_CLIENTE,
                //                    Categoria = roArp.CATEGORIA
                //                };

                //var tottARPTSE = rowselect.ToList();
                //var tottARPSTE = rowselectSTE.ToList();

                return true;

                //foreach (var entity in model)
                //{
                //    var convert2 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(entity.ToJsonString());
                //    var convert = Newtonsoft.Json.JsonConvert.DeserializeObject<STELoadEntity>(entity.ToJsonString());
                //    convert.IdSTELoad = Guid.NewGuid();
                //    if (string.IsNullOrEmpty(convert.AccountCMRNumber)) { convert.AccountCMRNumber = "1234"; }
                //    if (string.IsNullOrEmpty(convert.StartHours))
                //    {

                //        if (string.IsNullOrEmpty(convert.StartDateTime))
                //        {
                //            return false;
                //        }

                //        DateTimeOffset fecha;

                //        if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.StartHours = fecha.ToString("HH:mm");
                //        }
                //        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.StartHours = fecha.ToString("HH:mm");
                //        }
                //        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.StartHours = fecha.ToString("HH:mm");
                //        }
                //        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.StartHours = fecha.ToString("HH:mm");
                //        }
                //        else
                //        {
                //            return false;
                //        }
                //    }
                //    if (string.IsNullOrEmpty(convert.EndHours))
                //    {

                //        if (string.IsNullOrEmpty(convert.EndDateTime))
                //        {
                //            return false;
                //        }
                //        DateTimeOffset fecha;

                //        if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.EndHours = fecha.ToString("HH:mm");
                //        }
                //        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.EndHours = fecha.ToString("HH:mm");
                //        }
                //        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.EndHours = fecha.ToString("HH:mm");
                //        }
                //        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                //        {
                //            convert.EndHours = fecha.ToString("HH:mm");
                //        }
                //        else
                //        {
                //            return false;
                //        }
                //    }

                //    var fechaRegistro = new DateTimeOffset();

                //    DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRegistro);

                //    //convert.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy");

                //    await _dataBaseService.SaveAsync();

                //    var sTELoads = _dataBaseService.STELoadEntity.ToList();

                //    foreach (var ste in sTELoads)
                //    {

                //        if (string.IsNullOrEmpty(ste.StartDateTime))
                //        {
                //            return false;
                //        }

                //        var semanahorario = DateTimeOffset.Parse(ste.StartDateTime);
                //        CultureInfo cul = CultureInfo.CurrentCulture;
                //        List<HorarioReturn> fueraH = new List<HorarioReturn>();
                //        var esfestivo = new FestivosEntity();

                //        int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                //        workinghoursEntity horario = null;
                //        try
                //        {
                //            horario = _dataBaseService.workinghoursEntity.Where(x => x.UserEntity.EmployeeCode == ste.AccountCMRNumber && x.week == Semana.ToString() && x.FechaWorking == semanahorario).FirstOrDefault();
                //        }
                //        catch (Exception e)
                //        {
                //            return false;
                //        }

                //        if (horario != null)
                //        {
                //            esfestivo = _dataBaseService.FestivosEntity.Where(x => x.DiaFestivo == semanahorario).FirstOrDefault();
                //            if (esfestivo == null)
                //            {
                //                fueraH = FueraHorario(ste.StartHours, ste.EndHours, horario.HoraInicio, horario.HoraFin);
                //                if (fueraH != null && fueraH.Count > 0)
                //                {
                //                    foreach (var aceepent in fueraH)
                //                    {

                //                        QueuesAcceptanceEntitySTE queuesAcceptanceEntity = new QueuesAcceptanceEntitySTE();
                //                        queuesAcceptanceEntity.IdQueuesAcceptanceEntitySTE = Guid.NewGuid();
                //                        queuesAcceptanceEntity.STELoadEntityId = ste.IdSTELoad;
                //                        queuesAcceptanceEntity.Id_empleado = ste.AccountCMRNumber;
                //                        queuesAcceptanceEntity.AprobadoSistema = DateTime.Now;
                //                        queuesAcceptanceEntity.Hora_Inicio = aceepent.Inicio;
                //                        queuesAcceptanceEntity.Hora_Fin = aceepent.Fin;
                //                        queuesAcceptanceEntity.Horas_Total = aceepent.Total;
                //                        queuesAcceptanceEntity.Comentario = "";
                //                        queuesAcceptanceEntity.Estado = 1;
                //                        queuesAcceptanceEntity.FechaRe = ste.StartDateTime.ToString();

                //                    }

                //                }
                //            }
                //        }

                //        ParametersSteInitialEntity parametersTseInitialEntity = new ParametersSteInitialEntity();

                //        parametersTseInitialEntity.IdParamSTEInitialId = Guid.NewGuid();
                //        if (horario != null)
                //        {
                //            parametersTseInitialEntity.HoraInicio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                //            parametersTseInitialEntity.HoraFin = horario.HoraFin == null ? "0" : horario.HoraFin;
                //            parametersTseInitialEntity.Estado = horario.HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";

                //        }
                //        else
                //        {
                //            workinghoursEntity workinghoursEntity = new workinghoursEntity();
                //            workinghoursEntity.HoraInicio = "0";
                //            workinghoursEntity.HoraFin = "0";

                //            parametersTseInitialEntity.HoraInicio = workinghoursEntity.HoraInicio;
                //            parametersTseInitialEntity.HoraFin = workinghoursEntity.HoraFin;

                //        }


                //        parametersTseInitialEntity.OutIme = fueraH.Count > 0 ? "Y" : "N";
                //        parametersTseInitialEntity.OverTime = fueraH.Count > 0 ? "Y" : "N";
                //        parametersTseInitialEntity.Semana = Semana;
                //        parametersTseInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                //        parametersTseInitialEntity.STELoadEntityId = ste.IdSTELoad;
                //        parametersTseInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";

                //        _dataBaseService.ParametersSteInitialEntity.Add(parametersTseInitialEntity);
                //        await _dataBaseService.SaveAsync();

                //    }
                //}

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
