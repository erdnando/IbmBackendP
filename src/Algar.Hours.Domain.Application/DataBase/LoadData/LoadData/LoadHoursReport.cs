using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Algar.Hours.Domain.Entities.UsersExceptions;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.ObjectModel;
using System.Globalization;
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
                List<ParametersArpInitialEntity> listParametersInitialEntity = new List<ParametersArpInitialEntity>();
                


                #region por cada registro obtenido en el excel, se evalua
                foreach (var entity in model)
                    {
                    counter++;
                    #region Validaciones de datos

                            var convert = Newtonsoft.Json.JsonConvert.DeserializeObject<ARPLoadDetailEntity>(entity.ToJsonString());
                               // convert.IdDetail = Guid.Empty;
                                convert.IdDetail = Guid.NewGuid();//ok
                                convert.ARPLoadEntityId = aRPLoadEntity.IdArpLoad;

                                if (convert.ESTADO.Trim() == "EXTRACTED") { convert.ESTADO = "Extracted"; }
                                if (convert.ESTADO.Trim() == "FINAL") { convert.ESTADO = "Submitted"; }
                                if (string.IsNullOrEmpty(convert.NUMERO_CLIENTE)) { convert.NUMERO_CLIENTE = "1234"; }


                                if (!string.IsNullOrEmpty(convert.FECHA_REP))
                                {
                                    try
                                    {
                                        string fecha = convert.FECHA_REP;
                                        DateTimeOffset fechaRep;


                                        if (DateTimeOffset.TryParseExact(fecha, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                                        {
                                            convert.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                                        }
                                        else if (DateTimeOffset.TryParseExact(fecha, "M/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                                        {
                                            convert.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                                        }
                                        else if (DateTimeOffset.TryParseExact(fecha, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                                        {
                                            convert.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                                        }
                                        else if (DateTimeOffset.TryParseExact(fecha, "MM/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                                        {
                                            convert.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            return false;
                                        }



                                    }
                                    catch (FormatException)
                                    {
                                        if (DateTime.TryParseExact(convert.FECHA_REP, "MM/dd/yy", null, DateTimeStyles.None, out DateTime fechaConvertida) ||
                                            DateTime.TryParseExact(convert.FECHA_REP, "M/d/yy", null, DateTimeStyles.None, out fechaConvertida))
                                        {
                                            convert.FECHA_REP = fechaConvertida.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            Console.WriteLine("No se pudo convertir la cadena de fecha");
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(convert.FECHA_EXTRATED))
                            {

                                convert.FECHA_EXTRATED = convert.FECHA_REP;
                            }

                    await _dataBaseService.ARPLoadDetailEntity.AddAsync(convert);
                    //await _dataBaseService.SaveAsync();

                    #endregion

                    // var ArpLoading = _dataBaseService.ARPLoadDetailEntity.ToList();

                    #region evaluacion contra ARPLoadDetailEntity

                    var arp = convert;
                        //foreach (var arp in ArpLoading)
                        //{
                                //var semanahorario = DateTimeOffset.Parse(arp.FECHA_REP);
                                var semanahorario = new DateTimeOffset();// arp.FECHA_REP;
                                DateTimeOffset fechaRepx;

                                if (DateTimeOffset.TryParseExact(arp.FECHA_REP, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRepx))
                                {
                                    semanahorario = fechaRepx;
                                }
                                else if (DateTimeOffset.TryParseExact(arp.FECHA_REP, "M/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRepx))
                                {
                                    semanahorario = fechaRepx;
                                }
                                else if (DateTimeOffset.TryParseExact(arp.FECHA_REP, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRepx))
                                {
                                    semanahorario = fechaRepx;
                                }
                                else if (DateTimeOffset.TryParseExact(arp.FECHA_REP, "MM/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRepx))
                                {
                                    semanahorario = fechaRepx;
                                }
                                else if (DateTimeOffset.TryParseExact(arp.FECHA_REP, "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRepx))
                                {
                                    semanahorario = fechaRepx;
                                }




                                CultureInfo cul = CultureInfo.CurrentCulture;
                                List<HorarioReturn> fueraH = new List<HorarioReturn>();
                                var esfestivo = new FestivosEntity();

                                int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                                
                                //Busca horarios configurados para este empleado en la semana y dia obtenido del excel de carga
                                workinghoursEntity horario = _dataBaseService.workinghoursEntity.Where(x => x.UserEntity.EmployeeCode == arp.ID_EMPLEADO && 
                                                                                                            x.week == Semana.ToString() && 
                                                                                                            x.FechaWorking == semanahorario).FirstOrDefault();
                                
                                if (horario != null)
                                {
                                    esfestivo = _dataBaseService.FestivosEntity.Where(x => x.DiaFestivo == semanahorario).FirstOrDefault(); //&& x.CountryId == "");
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
                                }

                                ParametersArpInitialEntity parametersInitialEntity = new ParametersArpInitialEntity();

                                parametersInitialEntity.IdParametersInitialEntity = Guid.NewGuid();
                                if (horario != null)
                                {
                                    parametersInitialEntity.HoraInicio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                                    parametersInitialEntity.HoraFin = horario.HoraFin == null ? "0" : horario.HoraFin;
                                    parametersInitialEntity.Estado = horario.HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";

                                }
                                else
                                {
                                    workinghoursEntity workinghoursEntity = new workinghoursEntity();
                                    workinghoursEntity.HoraInicio = "0";
                                    workinghoursEntity.HoraFin = "0";

                                    parametersInitialEntity.HoraInicio = workinghoursEntity.HoraInicio;
                                    parametersInitialEntity.HoraFin = workinghoursEntity.HoraFin;

                                }


                                parametersInitialEntity.OutIme = fueraH.Count > 0 ? "Y" : "N";
                                parametersInitialEntity.OverTime = fueraH.Count > 0 ? "Y" : "N";
                                parametersInitialEntity.Semana = Semana;
                                parametersInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                                parametersInitialEntity.ARPLoadDetailEntityId = arp.IdDetail;
                                parametersInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";


                                listParametersInitialEntity.Add(parametersInitialEntity);
                                //_dataBaseService.ParametersArpInitialEntity.Add(parametersInitialEntity);
                                // await _dataBaseService.SaveAsync();


                               // _dataBaseService.ParametersArpInitialEntity.AddRange(listParametersInitialEntity);//EF
                               //  await _dataBaseService.SaveAsync();
                    //_dataBaseService.BulkInsertParametersArpInitialEntity(listParametersInitialEntity);//EF BI

                    // }

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

                var horus = _dataBaseService.HorusReportEntity.Where(x => x.UserEntityId == Guid.NewGuid()).Sum(i=> double.Parse(i.CountHours));    
               



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
           return  _dataBaseService.UsersExceptions.Where(x => x.IdUsersExceptions == Guid.NewGuid()).FirstOrDefault();

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
                foreach (var item in model)
                {
                    var convert = Newtonsoft.Json.JsonConvert.DeserializeObject<TSELoadEntity>(item.ToJsonString());
                    convert.IdTSELoad = Guid.NewGuid();
                    if (convert.Status.Trim() == "EXTRACTED") { convert.Status = "Extracted"; }
                    if (convert.Status.Trim() == "SUBMITTED") { convert.Status = "Submitted"; }
                    if (string.IsNullOrEmpty(convert.AccountCMRNumber) || convert.AccountCMRNumber == "N/A") { convert.AccountCMRNumber = "1234"; }


                    /* string fecha = convert.StartTime;
                     DateTimeOffset fechaRep;
                     string fechaEND = convert.EndTime;
                     DateTimeOffset fechaRep2;

                     if (convert.StartTime != null)
                     {
                         if (DateTimeOffset.TryParseExact(fecha, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                         {
                             convert.StartTime = fechaRep.ToString("dd/MM/yyyy h:mm tt");
                         }
                         else if (DateTimeOffset.TryParseExact(fecha, "M/dd/yy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                         {
                             convert.StartTime = fechaRep.ToString("dd/MM/yyyy h:mm tt");
                         }
                         else if (DateTimeOffset.TryParseExact(fecha, "M/d/yy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                         {
                             convert.StartTime = fechaRep.ToString("dd/MM/yyyy h:mm tt");
                         }
                         else if (DateTimeOffset.TryParseExact(fecha, "MM/d/yy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                         {
                             convert.StartTime = fechaRep.ToString("dd/MM/yyyy h:mm tt");
                         }

                     }

                     if (convert.EndTime != null)
                     {
                         if (DateTimeOffset.TryParseExact(fecha, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep2))
                         {
                             convert.EndTime = fechaRep2.ToString("dd/MM/yyyy h:mm tt");
                         }
                         else if (DateTimeOffset.TryParseExact(fecha, "M/dd/yy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep2))
                         {
                             convert.EndTime = fechaRep2.ToString("dd/MM/yyyy h:mm tt");
                         }
                         else if (DateTimeOffset.TryParseExact(fecha, "M/d/yy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep2))
                         {
                             convert.EndTime = fechaRep2.ToString("dd/MM/yyyy h:mm tt");
                         }
                         else if (DateTimeOffset.TryParseExact(fecha, "MM/d/yy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep2))
                         {
                             convert.EndTime = fechaRep2.ToString("dd/MM/yyyy h:mm tt");
                         }

                     } */

                    DateTimeOffset dateTimeOffset = new DateTimeOffset();

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
                            if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
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
                            if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
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

                    //convert.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy");

                    await _dataBaseService.SaveAsync();

                    var TSELoading = _dataBaseService.TSELoadEntity.ToList();

                    foreach (var tse in TSELoading)
                    {

                        var semanahorario = DateTimeOffset.Parse(tse.StartTime);
                        CultureInfo cul = CultureInfo.CurrentCulture;
                        List<HorarioReturn> fueraH = new List<HorarioReturn>();
                        var esfestivo = new FestivosEntity();

                        int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                        workinghoursEntity horario = _dataBaseService.workinghoursEntity.Where(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString() && x.FechaWorking == semanahorario).FirstOrDefault();
                        if (horario != null)
                        {
                            esfestivo = _dataBaseService.FestivosEntity.Where(x => x.DiaFestivo == semanahorario).FirstOrDefault();
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


                        _dataBaseService.ParametersTseInitialEntity.Add(parametersTseInitialEntity);
                        await _dataBaseService.SaveAsync();
                    }
                }
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
                foreach (var entity in model)
                {
                    var convert2 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(entity.ToJsonString());
                    var convert = Newtonsoft.Json.JsonConvert.DeserializeObject<STELoadEntity>(entity.ToJsonString());
                    convert.IdSTELoad = Guid.NewGuid();
                    if (string.IsNullOrEmpty(convert.AccountCMRNumber)) { convert.AccountCMRNumber = "1234"; }
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

                    //convert.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy");

                    await _dataBaseService.SaveAsync();

                    var sTELoads = _dataBaseService.STELoadEntity.ToList();

                    foreach (var ste in sTELoads)
                    {

                        if (string.IsNullOrEmpty(ste.StartDateTime))
                        {
                            return false;
                        }

                        var semanahorario = DateTimeOffset.Parse(ste.StartDateTime);
                        CultureInfo cul = CultureInfo.CurrentCulture;
                        List<HorarioReturn> fueraH = new List<HorarioReturn>();
                        var esfestivo = new FestivosEntity();

                        int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                        workinghoursEntity horario = null;
                        try
                        {
                            horario = _dataBaseService.workinghoursEntity.Where(x => x.UserEntity.EmployeeCode == ste.AccountCMRNumber && x.week == Semana.ToString() && x.FechaWorking == semanahorario).FirstOrDefault();
                        }
                        catch (Exception e)
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

                        _dataBaseService.ParametersSteInitialEntity.Add(parametersTseInitialEntity);
                        await _dataBaseService.SaveAsync();

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
