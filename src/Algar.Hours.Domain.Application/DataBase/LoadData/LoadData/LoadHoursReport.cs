using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.PortalDB.Commands;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Domain.Entities.Aprobador;
using Algar.Hours.Domain.Entities.AssignmentReport;
using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.PaisRelacionGMT;
using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Algar.Hours.Domain.Entities.User;
using Algar.Hours.Domain.Entities.UsersExceptions;
using AutoMapper;
using EFCore.BulkExtensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using NetTopologySuite.Index.HPRtree;
using Org.BouncyCastle.Utilities;
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
        private IConsultCountryCommand _consultCountryCommand;
        private IEmailCommand _emailCommand;

        public LoadHoursReport(IDataBaseService dataBaseService, IMapper mapper, IConsultCountryCommand consultCountryCommand, IEmailCommand emailCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _consultCountryCommand = consultCountryCommand;
            _emailCommand = emailCommand;
        }

        ARPLoadDetailEntity validaFormatosFecha(ARPLoadDetailEntity arp)
        {
            arp.IdDetail = Guid.Empty;
            arp.IdDetail = Guid.NewGuid();
           // registroARP.ARPLoadEntityId = aRPLoadEntity.IdArpLoad;

            if (!string.IsNullOrEmpty(arp.FECHA_REP))
            {
                try
                {
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
                    else
                    {
                        return arp;
                    }



                    /*if (DateTimeOffset.TryParseExact(fecha, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        registroARP.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                    }
                    else if (DateTimeOffset.TryParseExact(fecha, "M/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        registroARP.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                    }
                    else if (DateTimeOffset.TryParseExact(fecha, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        registroARP.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                    }
                    else if (DateTimeOffset.TryParseExact(fecha, "MM/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRep))
                    {
                        registroARP.FECHA_REP = fechaRep.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        return registroARP;
                    }*/



                }
                catch (FormatException)
                {
                    if (DateTime.TryParseExact(arp.FECHA_REP, "MM/dd/yy", null, DateTimeStyles.None, out DateTime fechaConvertida) ||
                        DateTime.TryParseExact(arp.FECHA_REP, "M/d/yy", null, DateTimeStyles.None, out fechaConvertida))
                    {
                        arp.FECHA_REP = fechaConvertida.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        Console.WriteLine("No se pudo convertir la cadena de fecha");
                    }
                }
            }

            if (!string.IsNullOrEmpty(arp.FECHA_EXTRATED))
            {
                arp.FECHA_EXTRATED = arp.FECHA_REP;
            }

            return arp;
        }

        public async Task<string> LoadARP(LoadJsonPais model)
        {
            try
            {
                //deleting loads previous processing..
                _dataBaseService.ParametersArpInitialEntity.ExecuteDelete();
                _dataBaseService.ARPLoadDetailEntity.ExecuteDelete();
                _dataBaseService.ARPLoadEntity.ExecuteDelete();
                await _dataBaseService.SaveAsync();

            }
            catch (Exception ex)
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
                    userEntityId = Guid.Parse("3696718D-D05A-4831-96CE-ED500C5BBC97"),
                    ARPCarga="0",
                    ARPOmitidos = "0",
                    ARPXHorario = "0",
                    ARPXOverlaping = "0",
                    ARPXOvertime = "0",
                    STECarga = "0",
                    STEEXOvertime = "0",
                    STEOmitidos = "0",
                    STEXHorario = "0",
                    STEXOverlaping = "0",
                    TSECarga = "0",
                    TSEOmitidos = "0",
                    TSEXHorario = "0",
                    TSEXOverlaping  = "0",
                    TSEXOvertime = "0",
                    ARPXProceso = "0",
                    STEXProceso = "0",
                    TSEXProceso = "0"

                };

                await _dataBaseService.ARPLoadEntity.AddAsync(aRPLoadEntity);
                await _dataBaseService.SaveAsync();
                #endregion

                Int64 counter = 0;
                List<ParametersArpInitialEntity> listParametersInitialEntity = new();


                //Serializa la carga excel en un obj
                List<ARPLoadDetailEntity> datosARPExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ARPLoadDetailEntity>>(model.Data.ToJsonString());


               
                
                // datosARPExcel.Where(e => e.ESTADO.Trim() == "EXTRACTED").ToList().ForEach(x => x.ESTADO = "Extracted");
                //datosARPExcel.Where(e => e.ESTADO.Trim() == "FINAL").ToList().ForEach(x => x.ESTADO = "Submitted");

                List<ARPLoadDetailEntity> datosARPExcel = datosARPExcelFull!.Where(x => x.ESTADO != "EXTRACTED" && x.ESTADO != "FINAL").ToList();


                 _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == aRPLoadEntity.IdArpLoad).ToList().
                                              ForEach(x => x.ARPCarga = datosARPExcelFull.Count.ToString());

                 _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == aRPLoadEntity.IdArpLoad).ToList().
                                              ForEach(x => x.ARPOmitidos = (datosARPExcelFull.Count - datosARPExcel.Count).ToString() );

                await _dataBaseService.SaveAsync();

                List<string> politicaOvertime = new() { "Vacations", "Absence", "Holiday", "Stand By" };

                var semanahorario = new DateTimeOffset();

                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();

                List<FestivosEntity> esfestivos = new();

                //Para ARP, busca festivos de Colombia solamente
                esfestivos = _dataBaseService.FestivosEntity.Where(x => x.CountryId == new Guid("908465f1-4848-4c86-9e30-471982c01a2d")).ToList(); //&& x.CountryId == "");
               var horariosGMT = await _dataBaseService.PaisRelacionGMTEntity.Where(e=>e.NameCountrySelected==model.PaisSel).ToListAsync();


                //Busca horarios configurados para este empleado en la semana y dia obtenido del excel de carga
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();

                //busca Gral Paises
                var paisGeneral = await _dataBaseService.CountryEntity.ToListAsync();

                foreach (var entity in datosARPExcel)
                {

                    
                    var arp = validaFormatosFecha(entity);
                    //var arp = validaHoraGMT(arpFecha, model.PaisSel);
                    //var arp = validaHoraGMT(arpFecha, horariosGMT, paisGeneral);


                    arp.ARPLoadEntityId = aRPLoadEntity.IdArpLoad;//check it!!!
                    semanahorario = DateTimeOffset.Parse(arp.FECHA_REP);

                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                    //Obtiene horario para este empleado en la fecha del evento
                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == arp.ID_EMPLEADO && x.week == Semana.ToString());// && x.FechaWorking == semanahorario.DateTime);


                    //Valida si el dia del evento es un festivo del pais Colombia
                    var esfestivo = esfestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario);


                    ParametersArpInitialEntity parametersInitialEntity = new ParametersArpInitialEntity();
                    parametersInitialEntity.IdParametersInitialEntity = Guid.NewGuid();
                    parametersInitialEntity.FECHA_REP = arp.FECHA_REP;
                    parametersInitialEntity.TOTAL_MINUTOS = arp.TOTAL_MINUTOS;
                    parametersInitialEntity.totalHoras = getHoras(arp.TOTAL_MINUTOS);
                    parametersInitialEntity.EstatusProceso = "EN_OVERTIME";
                    parametersInitialEntity.HorasInicio = 0;
                    parametersInitialEntity.HorasFin = 0;
                    parametersInitialEntity.EmployeeCode = arp.ID_EMPLEADO;
                    parametersInitialEntity.Anio = semanahorario.Year.ToString();
                    parametersInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                    parametersInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";
                    parametersInitialEntity.HoraInicio = arp.HORA_INICIO;//hora reportada inicio
                    parametersInitialEntity.HoraFin = arp.HORA_FIN;//hora reportada fin
                    parametersInitialEntity.OverTime = "N";
                    parametersInitialEntity.OutIme = "N";
                    parametersInitialEntity.Semana = Semana;
                    parametersInitialEntity.HoraInicioHoraio = "0";
                    parametersInitialEntity.HoraFinHorario = "0";
                    parametersInitialEntity.IdCarga =arp.ARPLoadEntityId;




                    if (arp.ESTADO == "Extracted" || arp.ESTADO == "EXTRACTED")
                    {
                        parametersInitialEntity.EstatusProceso = "NO_APLICA_X_EXTRACTED";
                        listParametersInitialEntity.Add(parametersInitialEntity);
                        continue;
                    }

                    var previosAndPos = new List<double>();
                    if (horario != null)
                    {
                        previosAndPos = getPreviasAndPosHorario(arp.HORA_INICIO, arp.HORA_FIN, horario.HoraInicio, horario.HoraFin);
                    }
                    else
                    {
                        previosAndPos.Add(0.0);
                        previosAndPos.Add(0.0);
                    }

                    if (horario != null)
                    {
                        //fechas registadas en el horario asignado
                        parametersInitialEntity.HoraInicioHoraio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                        parametersInitialEntity.HoraFinHorario = horario.HoraFin == null ? "0" : horario.HoraFin;

                        parametersInitialEntity.OverTime = horario.HoraInicio == null ? "N" : politicaOvertime.IndexOf(arp.ACTIVIDAD.ToUpper()) == -1 ? "N" : "S";
                        parametersInitialEntity.EstatusProceso = parametersInitialEntity.OverTime == "N" ? parametersInitialEntity.EstatusProceso : "NO_APLICA_X_OVERTIME";
                        parametersInitialEntity.HorasInicio = previosAndPos[0];
                        parametersInitialEntity.HorasFin = previosAndPos[1];

                       // parametersInitialEntity.HorarioExistenteInicio=horario

                        if (parametersInitialEntity.OverTime == "S")
                        {
                            //---------------NO_APLICA_X_OVERTIME----------
                            listParametersInitialEntity.Add(parametersInitialEntity);
                            continue;
                        }
                    }
                    else
                    {
                        //NO hay horario
                        parametersInitialEntity.HoraInicioHoraio = "0";
                        parametersInitialEntity.HoraFinHorario = "0";

                        parametersInitialEntity.EstatusProceso = "NO_APLICA_X_HORARIO";
                        listParametersInitialEntity.Add(parametersInitialEntity);
                        continue;
                    }


                    listParametersInitialEntity.Add(parametersInitialEntity);
                }



                _dataBaseService.ParametersArpInitialEntity.AddRange(listParametersInitialEntity);
                await _dataBaseService.SaveAsync();



                return aRPLoadEntity.IdArpLoad.ToString();





            }
            catch (Exception ex){
                return "00000000-0000-0000-0000-000000000000";
            }
            
        }

        private ARPLoadDetailEntity validaHoraGMT(ARPLoadDetailEntity arpRegistro, List<PaisRelacionGMTEntity> paisGMT, List<CountryEntity> paisGeneral)
        {
            var paisByCode = paisGeneral.FirstOrDefault(x => x.CodigoPais.Trim().ToUpper() == arpRegistro.PAIS.Trim().ToUpper());
            var paisComparacion = paisGMT.FirstOrDefault(e => e.NameCountryCompare == paisByCode.NameCountry);
            try
            {
                var HoraInicioOrigin = DateTime.Parse(arpRegistro.HORA_INICIO);
                var horaActualizada = HoraInicioOrigin.AddHours(paisComparacion.TimeDifference);
                arpRegistro.HORA_INICIO = horaActualizada.ToString("HH:mm:ss");

            }
            catch (Exception ex)
            {
                arpRegistro.HORA_INICIO = "0";
            }

            try
            {
                var HoraFinOrigin = DateTime.Parse(arpRegistro.HORA_FIN);
                var horaFinActualizada = HoraFinOrigin.AddHours(paisComparacion.TimeDifference);
                arpRegistro.HORA_FIN = horaFinActualizada.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                arpRegistro.HORA_FIN = "0";
            }

            return arpRegistro;
        }

        private string getHoras(string mins)
        {
            string totalHrs = "0";
            double minutos = 0.0;
            if (mins.Trim().Equals("")) return totalHrs;

            try
            {
                minutos = Convert.ToDouble(mins)/60;
                totalHrs = (Math.Round(minutos, 2)).ToString();
            }
            catch(Exception ex)
            {
                totalHrs = "0";
            }

            return totalHrs;
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

        public async Task<string> LoadTSE(LoadJsonPais model)
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
            try { 
            
                List<TSELoadEntity> datosTSEExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TSELoadEntity>>(model.Data.ToJsonString());
                //datosTSEExcel.Where(e => e.Status.Trim() == "EXTRACTED").ToList().ForEach(x => x.Status = "Extracted");
               // datosTSEExcel.Where(e => e.Status.Trim() == "SUBMITTED").ToList().ForEach(x => x.Status = "Submitted");

                List<TSELoadEntity> datosTSEExcel = datosTSEExcelFull!.Where(x => x.Status != "Extracted" && x.Status != "Final" && x.Status != "Submitted").ToList();






                try
                {
                    _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                                                 ForEach(x => x.TSEOmitidos = (datosTSEExcelFull.Count - datosTSEExcel.Count).ToString());
                    await _dataBaseService.SaveAsync();

                    //Setting number of register on STE
                    _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                                  ForEach(x => x.TSECarga = datosTSEExcelFull.Count().ToString());
                    await _dataBaseService.SaveAsync();
                }
                catch(Exception ex)
                {

                }
                



                datosTSEExcel.Where(e => string.IsNullOrEmpty(e.AccountCMRNumber) == true || e.AccountCMRNumber == "N/A").ToList().ForEach(x => x.AccountCMRNumber = "1234");
                int counter = 0;
        
                var semanahorario = new DateTimeOffset();

                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();



                var listaCountries = await _consultCountryCommand.List();

                List<FestivosEntity> listFestivos = new();
                listFestivos = _dataBaseService.FestivosEntity.ToList(); //&& x.CountryId == "");
               
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();
                var horariosGMT = await _dataBaseService.PaisRelacionGMTEntity.Where(e => e.NameCountrySelected == model.PaisSel).ToListAsync();
                var paisGeneral = await _dataBaseService.CountryEntity.ToListAsync();

                List<ParametersTseInitialEntity> listParametersInitialEntity = new();

                foreach (var registro in datosTSEExcel)
                {
                    var tseFecha = validaFormatosFechaTSE(registro);
                    var paisRegistro = listaCountries.FirstOrDefault(e=>e.CodigoPais== tseFecha.NumeroEmpleado.Substring(tseFecha.NumeroEmpleado.Length - 3));
                    //var tse = validaHoraTSEGMT(tseFecha, model.PaisSel);
                    var tse = validaHoraTSEGMT(tseFecha, horariosGMT, paisRegistro);

                    //semanahorario = DateTimeOffset.Parse(tse.StartTime);
                    try
                    {
                        
                        semanahorario = DateTimeOffset.ParseExact(tse.StartTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    }
                    catch (Exception exx)
                    {
                        try
                        {
                            semanahorario = DateTimeOffset.ParseExact(tse.StartTime, "d/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                        }catch(Exception ex)
                        {
                            semanahorario = DateTimeOffset.ParseExact(tse.StartTime, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                        }
                        
                    }

                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                    bool bValidacionHorario = false;

                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString());// && x.FechaWorking== semanahorario.DateTime);
                    //var horario = _dataBaseService.workinghoursEntity.FirstOrDefault(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString() && x.FechaWorking == semanahorario);
                    var esfestivo = listFestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario && x.CountryId== paisRegistro!.IdCounty);

                    /*if (horario != null)
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
                    }*/


                   


                    ParametersTseInitialEntity parametersTseInitialEntity = new ParametersTseInitialEntity();

                    parametersTseInitialEntity.IdParamTSEInitialId = Guid.NewGuid();
                    parametersTseInitialEntity.EstatusProceso = "EN_OVERTIME";

                    parametersTseInitialEntity.FECHA_REP = tse.StartTime;
                    parametersTseInitialEntity.TOTAL_MINUTOS = getMins(tse.DurationInHours);
                    parametersTseInitialEntity.totalHoras = tse.DurationInHours;

                    parametersTseInitialEntity.HorasInicio = 0;
                    parametersTseInitialEntity.HorasFin = 0;
                    parametersTseInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                    parametersTseInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";
                    parametersTseInitialEntity.HoraInicio = tse.StartTime;
                    parametersTseInitialEntity.HoraFin = tse.EndTime;
                    parametersTseInitialEntity.OverTime = "N";
                    parametersTseInitialEntity.OutIme = "N";
                    parametersTseInitialEntity.Semana = Semana;
                    parametersTseInitialEntity.HoraInicioHoraio = "0";
                    parametersTseInitialEntity.HoraFinHorario = "0";

                    //parametersTseInitialEntity.OutIme = fueraH.Count > 0 ? "Y" : "N";
                    // parametersTseInitialEntity.OverTime = fueraH.Count > 0 ? "Y" : "N";
                    parametersTseInitialEntity.Anio = semanahorario.Year.ToString();
                    parametersTseInitialEntity.EmployeeCode = tse.NumeroEmpleado;
                    parametersTseInitialEntity.IdCarga = new Guid(model.IdCarga);


                    if (tse.Status == "Extracted" || tse.Status == "EXTRACTED")
                    {
                        parametersTseInitialEntity.EstatusProceso = "NO_APLICA_X_EXTRACTED";
                        listParametersInitialEntity.Add(parametersTseInitialEntity);
                        continue;
                    }

                    var previosAndPos = new List<double>();
                    if (horario != null)
                    {
                        previosAndPos = getPreviasAndPosHorario(tse.HoraInicio, tse.HoraFin, horario.HoraInicio, horario.HoraFin);
                        //bValidacionHorario = true;
                    }
                    else
                    {
                        previosAndPos.Add(0.0);
                        previosAndPos.Add(0.0);
                        //bValidacionHorario = false;
                    }

                    if (horario != null)
                    {
                        
                        parametersTseInitialEntity.HoraInicioHoraio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                        parametersTseInitialEntity.HoraFinHorario = horario.HoraFin == null ? "0" : horario.HoraFin;
                        parametersTseInitialEntity.Estado = horario.HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";
                        //Para TSE, no aplica la politica por overtime

                        parametersTseInitialEntity.HorasInicio = previosAndPos[0];
                        parametersTseInitialEntity.HorasFin = previosAndPos[1];

                    }
                    else
                    {
                        //NO hay horario
                        parametersTseInitialEntity.HoraInicioHoraio = "0";
                        parametersTseInitialEntity.HoraFinHorario = "0";
                        parametersTseInitialEntity.EstatusProceso = "NO_APLICA_X_HORARIO";
                        listParametersInitialEntity.Add(parametersTseInitialEntity);
                        continue;
                    }
                   

                  



                    listParametersInitialEntity.Add(parametersTseInitialEntity);

                }

                await _dataBaseService.ParametersTseInitialEntity.AddRangeAsync(listParametersInitialEntity);
                await _dataBaseService.SaveAsync();

                return model.IdCarga.ToString();

            }
            catch (Exception ex)
            {
                return "00000000-0000-0000-0000-000000000000";

            }
        }

        private TSELoadEntity validaHoraTSEGMT(TSELoadEntity tseRegistro, List<PaisRelacionGMTEntity> paisGMT, CountryModel paisEntidad)
        {
            var paisComparacion = paisGMT.FirstOrDefault(e => e.NameCountryCompare == paisEntidad.NameCountry);
            try
            {
               // var HoraInicioOrigin = DateTimeOffset.ParseExact(tseRegistro.StartTime, "dd/MM/yyyy h:mm tt",);
                var HoraInicioOrigin = DateTimeOffset.ParseExact(tseRegistro.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

                var horaActualizada = HoraInicioOrigin.AddHours(paisComparacion.TimeDifference);
                tseRegistro.StartTime = horaActualizada.ToString("dd/MM/yyyy HH:mm:ss");// dd/MM/yyyy HH:mm:ss");

            }
            catch (Exception ex)
            {
               tseRegistro.StartTime= tseRegistro.StartTime;
            }

            try
            {
               // var HoraFinOrigin = DateTimeOffset.Parse(tseRegistro.EndTime);
                var HoraFinOrigin = DateTimeOffset.ParseExact(tseRegistro.EndTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                var horaFinActualizada = HoraFinOrigin.AddHours(paisComparacion.TimeDifference);
                tseRegistro.EndTime = horaFinActualizada.ToString("dd/MM/yyyy HH:mm:ss");// dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
               return tseRegistro;
            }

            return tseRegistro;
        }

        //private TSELoadEntity validaHoraTSEGMT(TSELoadEntity tseRegistro, string horasDiff)
        //{
        //    //var paisComparacion = paisGMT.FirstOrDefault(e => e.NameCountryCompare == paisEntidad.NameCountry);
        //    try
        //    {
        //        var HoraInicioOrigin = DateTime.Parse(tseRegistro.StartTime);
        //        var horaActualizada = HoraInicioOrigin.AddHours(Int32.Parse(horasDiff));
        //        tseRegistro.StartTime = horaActualizada.ToString("dd/MM/yyyy HH:mm:ss");

        //    }
        //    catch (Exception ex)
        //    {
        //        tseRegistro.StartTime = tseRegistro.StartTime;
        //    }

        //    try
        //    {
        //        var HoraFinOrigin = DateTime.Parse(tseRegistro.EndTime);
        //        var horaFinActualizada = HoraFinOrigin.AddHours(Int32.Parse(horasDiff));
        //        tseRegistro.EndTime = horaFinActualizada.ToString("dd/MM/yyyy HH:mm:ss");
        //    }
        //    catch (Exception ex)
        //    {
        //        tseRegistro.HoraFin = "0";
        //    }

        //    return tseRegistro;
        //}

        private TSELoadEntity validaFormatosFechaTSE(TSELoadEntity item)
        {
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
                            return convert;
                        }

                    }
                    catch (Exception ex)
                    {
                        throw;
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
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "M/dd/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "M/dd/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "MM/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "MM/dd/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "MM/dd/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "M/dd/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "MM/d/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "MM/d/yyyy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "M/dd/yyyy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yyyy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yyyy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndHours = dt.ToString("HH:mm");
                        }
                        else
                        {
                            convert.EndHours = "00:00";
                            return convert;
                        }


                    }
                    catch (Exception ex)
                    {
                        throw;
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
                        throw;
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
                        throw;
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
                            throw;
                        }
                        catch (TimeZoneNotFoundException e)
                        {
                            //return $"Error: no se pudo encontrar la zona horaria especificada. Detalles: {e.Message}";
                            throw;
                        }
                        catch (InvalidTimeZoneException e)
                        {
                            //return $"Error: la zona horaria especificada no es válida. Detalles: {e.Message}";
                            throw;
                        }
                        catch (Exception e)
                        {
                            //return $"Error inesperado: {e.Message}";
                            throw;
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
                        throw;
                    }
                    catch (InvalidTimeZoneException)
                    {
                        //return "La zona horaria especificada no es válida.";
                        throw;
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
                        throw;
                    }

                }

                var fechaRegistro = new DateTimeOffset();

                DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRegistro);

                convert.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy");

                return convert;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task<SummaryLoad> LoadSTE(LoadJsonPais model)
        {
         

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
                List<STELoadEntity> datosSTEExcel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STELoadEntity>>(model.Data.ToJsonString());
                datosSTEExcel.Where(e => string.IsNullOrEmpty(e.AccountCMRNumber) == true).ToList().ForEach(x => x.AccountCMRNumber = "1234");


                //Setting number of register on STE
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                              ForEach(x => x.STECarga = datosSTEExcel.Count().ToString());
                await _dataBaseService.SaveAsync();


                var semanahorario = new DateTimeOffset();

                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();

                var listaCountries = await _consultCountryCommand.List();

                List<FestivosEntity> listFestivos = new();
                listFestivos = _dataBaseService.FestivosEntity.ToList();

                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();

                var horariosGMT = await _dataBaseService.PaisRelacionGMTEntity.Where(e => e.NameCountrySelected == model.PaisSel).ToListAsync();
                var paisGeneral = await _dataBaseService.CountryEntity.ToListAsync();

                List<ParametersSteInitialEntity> listParametersInitialEntity = new();

                foreach (var registro in datosSTEExcel)
                {
                    var stefecha= validaFormatosFechaSTE(registro);
                    var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == stefecha.SessionEmployeeSerialNumber.Substring(stefecha.SessionEmployeeSerialNumber.Length - 3));
                    //var ste = validaHoraSTEGMT(stefecha, model.PaisSel);
                    var ste = validaHoraSTEGMT(stefecha, horariosGMT, paisRegistro);

                    // semanahorario = DateTimeOffset.Parse(ste.StartDateTime);
                    try
                    {
                        semanahorario = DateTimeOffset.ParseExact(ste.StartDateTime, "d/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch(Exception exx) {
                       
                        try
                        {
                            semanahorario = DateTimeOffset.ParseExact(ste.StartDateTime, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            semanahorario = DateTimeOffset.ParseExact(ste.StartDateTime, "d/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                        }
                    }
                    
                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                    bool bValidacionHorario = false;
                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == ste.SessionEmployeeSerialNumber && x.week == Semana.ToString());// && x.FechaWorking== semanahorario.DateTime);
                    
                    var esfestivo = listFestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario && x.CountryId == paisRegistro!.IdCounty);

                    /*if (string.IsNullOrEmpty(ste.StartDateTime))
                    {
                        return false;
                    }*/

                    /*if (horario != null)
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
                    }*/

                  

                    ParametersSteInitialEntity parametersSTEInitialEntity = new ParametersSteInitialEntity();

                    parametersSTEInitialEntity.IdParamSTEInitialId = Guid.NewGuid();
                    parametersSTEInitialEntity.EstatusProceso = "EN_OVERTIME";

                    parametersSTEInitialEntity.FECHA_REP = ste.StartDateTime;
                    parametersSTEInitialEntity.TOTAL_MINUTOS = getMins(ste.TotalDuration);
                    parametersSTEInitialEntity.totalHoras = ste.TotalDuration; //getHoras(arp.TOTAL_MINUTOS);
                    parametersSTEInitialEntity.HorasInicio = 0;
                    parametersSTEInitialEntity.HorasFin = 0;
                    parametersSTEInitialEntity.EmployeeCode = ste.SessionEmployeeSerialNumber;
                    parametersSTEInitialEntity.Anio = semanahorario.Year.ToString();
                    parametersSTEInitialEntity.Festivo = esfestivo != null ? "Y" : "N";
                    parametersSTEInitialEntity.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";
                    parametersSTEInitialEntity.HoraInicio = ste.StartDateTime;
                    parametersSTEInitialEntity.HoraFin = ste.EndDateTime;
                    parametersSTEInitialEntity.OverTime = "N";
                    parametersSTEInitialEntity.OutIme = "N";
                    parametersSTEInitialEntity.Semana = Semana;
                    parametersSTEInitialEntity.HoraInicioHoraio = "0";
                    parametersSTEInitialEntity.HoraFinHorario = "0";
                    parametersSTEInitialEntity.IdCarga = new Guid(model.IdCarga);

                    // parametersSTEInitialEntity.OutIme = fueraH.Count > 0 ? "Y" : "N";
                    /// parametersSTEInitialEntity.OverTime = fueraH.Count > 0 ? "Y" : "N";



                    var previosAndPos = new List<double>();
                    if (horario != null)
                    {
                        previosAndPos = getPreviasAndPosHorario(ste.StartHours, ste.EndHours, horario.HoraInicio, horario.HoraFin);
                        bValidacionHorario = true;
                    }
                    else
                    {
                        previosAndPos.Add(0.0);
                        previosAndPos.Add(0.0);
                        bValidacionHorario = false;
                    }

                    if (horario != null)
                    {

                        parametersSTEInitialEntity.HoraInicioHoraio = horario.HoraInicio == null ? "0" : horario.HoraInicio;
                        parametersSTEInitialEntity.HoraFinHorario = horario.HoraFin == null ? "0" : horario.HoraFin;
                        parametersSTEInitialEntity.Estado = horario.HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";

                        //Para STE, no aplica la politica por overtime

                        parametersSTEInitialEntity.HorasInicio = previosAndPos[0];

                        //agregar validacion para horasFin
                        parametersSTEInitialEntity.HorasFin = previosAndPos[1];

                    }
                    else
                    {
                        //NO hay horario
                        /*workinghoursEntity workinghoursEntity = new workinghoursEntity();
                        workinghoursEntity.HoraInicio = "0";
                        workinghoursEntity.HoraFin = "0";*/
                        parametersSTEInitialEntity.HoraInicioHoraio = "0";
                        parametersSTEInitialEntity.HoraFinHorario = "0";

                        parametersSTEInitialEntity.EstatusProceso = "NO_APLICA_X_HORARIO";
                        listParametersInitialEntity.Add(parametersSTEInitialEntity);
                        continue;

                    }

                    /*if (ste.ESTADO == "Extracted" || tse.ESTADO == "EXTRACTED")
                    {
                        parametersTseInitialEntity.EstatusProceso = "NO_APLICA_X_EXTRACTED";
                    }*/

                  
                    listParametersInitialEntity.Add(parametersSTEInitialEntity);
                }

                await _dataBaseService.ParametersSteInitialEntity.AddRangeAsync(listParametersInitialEntity);
                await _dataBaseService.SaveAsync();














                
                //PROCESO DE VALIDACION OVER-LAPING!!!!
                var rowARPGral = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();
                var rowSTEGral = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();
                var rowTSEGral = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();

                var registrosOverlaped = from arp in rowARPGral
                                         join ste in rowSTEGral on arp.EmployeeCode equals ste.EmployeeCode
                                         join tse in rowTSEGral on ste.EmployeeCode equals tse.EmployeeCode
                                         where (arp.Anio == ste.Anio || arp.Anio == tse.Anio || ste.Anio==tse.Anio)
                                         where (arp.Semana == ste.Semana || arp.Semana == tse.Semana || ste.Semana==tse.Semana)
                                         where (arp.HoraInicio == ste.HoraInicio || arp.HoraInicio == tse.HoraInicio || ste.HoraInicio==tse.HoraInicio)
                                         where (arp.HoraFin == ste.HoraFin || arp.HoraFin == tse.HoraFin || ste.HoraFin == tse.HoraFin)
                                         where (arp.EmployeeCode == ste.EmployeeCode || arp.EmployeeCode == tse.EmployeeCode || ste.EmployeeCode==tse.EmployeeCode)
                                         select new
                                         {
                                             CodeUser = arp.IdParametersInitialEntity,
                                             Categoria = arp.Estado,
                                             EstatusProceso=arp.EstatusProceso,
                                             EmployeCode = arp.EmployeeCode,
                                             Anio = arp.Anio,
                                             Semana = arp.Semana,
                                             HoraInicio = arp.HoraInicio,
                                             HoraFin=arp.HoraFin,
                                         };

                foreach (var item in registrosOverlaped)
                {
                    var itemARP = _dataBaseService.ParametersArpInitialEntity.FirstOrDefault(e => (e.EmployeeCode == item.EmployeCode && e.Semana== item.Semana && e.HoraInicio== item.HoraInicio && e.HoraFin== item.HoraFin && e.IdCarga==new Guid(model.IdCarga)));
                    itemARP.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                    var itemTSE = _dataBaseService.ParametersSteInitialEntity.FirstOrDefault(e => e.EmployeeCode == item.EmployeCode && e.Semana == item.Semana && e.HoraInicio == item.HoraInicio && e.HoraFin == item.HoraFin && e.IdCarga == new Guid(model.IdCarga));
                    itemTSE.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                    var itemSTE = _dataBaseService.ParametersTseInitialEntity.FirstOrDefault(e => e.EmployeeCode == item.EmployeCode && e.Semana == item.Semana && e.HoraInicio == item.HoraInicio && e.HoraFin == item.HoraFin && e.IdCarga == new Guid(model.IdCarga));
                    itemSTE.EstatusProceso = "NO_APLICA_X_OVERLAPING"; 

                    if(itemARP != null) _dataBaseService.ParametersArpInitialEntity.Update(itemARP);
                    if (itemTSE != null) _dataBaseService.ParametersSteInitialEntity.Update(itemTSE);
                    if (itemSTE != null) _dataBaseService.ParametersTseInitialEntity.Update(itemSTE);
                    await _dataBaseService.SaveAsync();
                }


                //getting metrics
                int arpNoAplicaXHorario = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_HORARIO" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXOverttime = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXOverLaping = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpEnProceso = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                
                var cargaRegistro = _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).FirstOrDefault();
                

                int tseNoAplicaXHorario = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_HORARIO" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXOverttime = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXOverLaping = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseEnProceso = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                
                //string tseOmitidos = _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).FirstOrDefault().TSEOmitidos;
                //string arpCarga = _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).FirstOrDefault().TSEOmitidos;

                int steNoAplicaXHorario = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_HORARIO" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXOverttime = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXOverLaping = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steEnProceso = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                string steOmitidos = "0";


                //updatting metrics
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXHorario = arpNoAplicaXHorario.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXOvertime = arpNoAplicaXOverttime.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXOverlaping = arpNoAplicaXOverLaping.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXProceso = arpEnProceso.ToString());

                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXHorario = tseNoAplicaXHorario.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXOvertime = tseNoAplicaXOverttime.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXOverlaping = tseNoAplicaXOverLaping.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXProceso = tseEnProceso.ToString());

                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXHorario = steNoAplicaXHorario.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEEXOvertime = steNoAplicaXOverttime.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXOverlaping = steNoAplicaXOverLaping.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXProceso = steEnProceso.ToString());

                await _dataBaseService.SaveAsync();




                //return summary

                SummaryLoad summary = new SummaryLoad();
                summary.Mensaje = "Carga procesada";
                summary.ARP_CARGA = cargaRegistro.ARPCarga;
                summary.TSE_CARGA = cargaRegistro.TSECarga;
                summary.STE_CARGA = cargaRegistro.STECarga;

                summary.NO_APLICA_X_HORARIO_ARP = arpNoAplicaXHorario.ToString();
                summary.NO_APLICA_X_OVERTIME_ARP = arpNoAplicaXOverttime.ToString();
                summary.NO_APLICA_X_OVERLAPING_ARP = arpNoAplicaXOverLaping.ToString();
                summary.EN_PROCESO_ARP = arpEnProceso.ToString();
                summary.ARP_OMITIDOS = cargaRegistro.ARPOmitidos;

                summary.NO_APLICA_X_HORARIO_TSE = tseNoAplicaXHorario.ToString();
                summary.NO_APLICA_X_OVERTIME_TSE = tseNoAplicaXOverttime.ToString();
                summary.NO_APLICA_X_OVERLAPING_TSE = tseNoAplicaXOverLaping.ToString();
                summary.EN_PROCESO_TSE = tseEnProceso.ToString();
                summary.TSE_OMITIDOS = cargaRegistro.TSEOmitidos;

                summary.NO_APLICA_X_HORARIO_STE =steNoAplicaXHorario.ToString();
                summary.NO_APLICA_X_OVERTIME_STE = steNoAplicaXOverttime.ToString();
                summary.NO_APLICA_X_OVERLAPING_STE = steNoAplicaXOverLaping.ToString();
                summary.EN_PROCESO_STE =steEnProceso.ToString();
                summary.IdCarga = model.IdCarga;
                summary.STE_OMITIDOS = steOmitidos;

                return summary;


            }
            catch (Exception ex)
            {
                SummaryLoad summary = new SummaryLoad();
                summary.Mensaje = "Carga en error, verifique logs";
                summary.NO_APLICA_X_HORARIO_ARP = "0";
                summary.NO_APLICA_X_OVERTIME_ARP = "0";
                summary.NO_APLICA_X_OVERLAPING_ARP = "0";
                summary.EN_PROCESO_ARP = "0";
                summary.ARP_OMITIDOS = "0";

                summary.NO_APLICA_X_HORARIO_TSE = "0";
                summary.NO_APLICA_X_OVERTIME_TSE = "0";
                summary.NO_APLICA_X_OVERLAPING_TSE = "0";
                summary.EN_PROCESO_TSE = "0";
                summary.TSE_OMITIDOS = "0";

                summary.NO_APLICA_X_HORARIO_STE = "0";
                summary.NO_APLICA_X_OVERTIME_STE = "0";
                summary.NO_APLICA_X_OVERLAPING_STE = "0";
                summary.EN_PROCESO_STE = "0";
                summary.STE_OMITIDOS = "0";

                summary.IdCarga = model.IdCarga;

                return summary;
            }
        }

        private STELoadEntity validaHoraSTEGMT(STELoadEntity tseRegistro, List<PaisRelacionGMTEntity> paisGMT, CountryModel paisEntidad)
        {
            var paisComparacion = paisGMT.FirstOrDefault(e => e.NameCountryCompare == paisEntidad.NameCountry);
            try
            {
                //var HoraInicioOrigin = DateTime.Parse(tseRegistro.StartDateTime);
                var HoraInicioOrigin = DateTimeOffset.ParseExact(tseRegistro.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                var horaActualizada = HoraInicioOrigin.AddHours(paisComparacion.TimeDifference);
                tseRegistro.StartDateTime = horaActualizada.ToString("dd/MM/yyyy HH:mm:ss");

            }
            catch (Exception ex)
            {
                tseRegistro.StartDateTime = tseRegistro.StartDateTime;
            }

            try
            {
                //var HoraFinOrigin = DateTime.Parse(tseRegistro.EndDateTime);
                var HoraFinOrigin = DateTimeOffset.ParseExact(tseRegistro.EndDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                var horaFinActualizada = HoraFinOrigin.AddHours(paisComparacion.TimeDifference);
                tseRegistro.EndDateTime = horaFinActualizada.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                tseRegistro.EndDateTime = tseRegistro.EndDateTime;
            }

            return tseRegistro;
        }

        private STELoadEntity validaHoraSTEGMT(STELoadEntity tseRegistro, string horasDiff)
        {
            //var paisComparacion = paisGMT.FirstOrDefault(e => e.NameCountryCompare == paisEntidad.NameCountry);
            try
            {
                // var HoraInicioOrigin = DateTime.Parse(tseRegistro.StartDateTime);
                var HoraInicioOrigin = DateTime.ParseExact(tseRegistro.StartDateTime, "dd/MM/yyyy HH:mm tt",CultureInfo.InvariantCulture);
                var horaActualizada = HoraInicioOrigin.AddHours(Int32.Parse(horasDiff));
                tseRegistro.StartDateTime = horaActualizada.ToString("dd/MM/yyyy HH:mm:ss");
                //dd/MM/yyyy h:mm tt

            }
            catch (Exception ex)
            {
                tseRegistro.StartDateTime = tseRegistro.StartDateTime;
            }

            try
            {
                // var HoraFinOrigin = DateTime.Parse(tseRegistro.EndDateTime);
                var HoraFinOrigin = DateTime.ParseExact(tseRegistro.EndDateTime, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);// DateTime.ParseExact(tseRegistro.EndDateTime, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
                var horaFinActualizada = HoraFinOrigin.AddHours(Int32.Parse(horasDiff));
                tseRegistro.EndDateTime= horaFinActualizada.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                tseRegistro.EndDateTime = tseRegistro.EndDateTime;
            }

            return tseRegistro;
        }

        private string getMins(string hrs)
        {
            string totalMins = "0";
            double horas = 0.0;
            if (hrs.Trim().Equals("")) return totalMins;

            try
            {
                horas = Convert.ToDouble(hrs) * 60;
                totalMins = (Math.Round(horas, 2)).ToString();
            }
            catch (Exception ex)
            {
                totalMins = "0";
            }

            return totalMins;
        }

        private STELoadEntity validaFormatosFechaSTE(STELoadEntity entity)
        {
            var convert = entity;
            convert.IdSTELoad = Guid.NewGuid();
            if (string.IsNullOrEmpty(convert.StartHours))
            {

                if (string.IsNullOrEmpty(convert.StartDateTime))
                {
                    return convert;
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
                    return convert;
                }
            }
            if (string.IsNullOrEmpty(convert.EndHours))
            {

                if (string.IsNullOrEmpty(convert.EndDateTime))
                {
                    return convert;
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
                    return convert;
                }
            }

            var fechaRegistro = new DateTimeOffset();

            DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRegistro);

            convert.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy");

            return convert;
        }

        public async Task<bool> NotificacionesProceso1(string idCarga)
        {

            var usersTLS = _dataBaseService.UserEntity.ToList();

            var ARPGral = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso != "EN_PROCESO" && e.IdCarga == new Guid(idCarga)).ToList();
            var STEGral = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso != "EN_PROCESO" && e.IdCarga == new Guid(idCarga)).ToList();
            var TSEGral = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso != "EN_PROCESO" && e.IdCarga == new Guid(idCarga)).ToList();

            var eventosNotificables = from arp in ARPGral
                                     join ste in STEGral on arp.IdCarga equals ste.IdCarga
                                     join tse in TSEGral on ste.IdCarga equals tse.IdCarga
                                     select new
                                     {
                                         CodeUser = arp.IdParametersInitialEntity,
                                         EstatusProceso = arp.EstatusProceso,
                                         EmployeCode = arp.EmployeeCode,
                                         Anio = arp.Anio,
                                         Semana = arp.Semana,
                                         HoraInicio = arp.HoraInicio,
                                         HoraFin = arp.HoraFin,
                                     };

            

            foreach (var evento in eventosNotificables)
            {
                var userData = usersTLS.FirstOrDefault(e => e.EmployeeCode == evento.EmployeCode);
                if (userData == null)
                {
                    //no se pudo enviar el correo, por q no hay datos registrados en el sistema

                    //notificar al admin
                    _emailCommand.SendEmail(new EmailModel { To = "santiagoael@algartech.com", Plantilla = "11" });
                    Thread.Sleep(300);
                    continue;
                }
                
                try
                {
                    //TODO Definir plantilla de correo
                    switch (evento.EstatusProceso)
                    {
                        case "NO_APLICA_X_HORARIO": _emailCommand.SendEmail(new EmailModel { To = userData!.Email, Plantilla = "8" });break;
                        case "NO_APLICA_X_OVERTIME": _emailCommand.SendEmail(new EmailModel { To = userData!.Email, Plantilla = "9" }); break;
                        case "NO_APLICA_X_OVERLAPING": _emailCommand.SendEmail(new EmailModel { To = userData!.Email, Plantilla = "10" }); break;
                        default: break; 
                    }
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }


            return true;
        }

        public async Task<bool> ValidaLimitesExcepcionesOverlapping(string idCarga)
        {

            //aqui
            var rowARPParameter = _dataBaseService.ParametersArpInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso== "EN_OVERTIME").ToList();
            var rowTSEParameter = _dataBaseService.ParametersTseInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
            var rowSTEParameter = _dataBaseService.ParametersSteInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();

            var limitesCountryARP = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == Guid.Parse("908465f1-4848-4c86-9e30-471982c01a2d"));
            var HorasLimiteDia = limitesCountryARP.TargetTimeDay;
            var listaCountries = await _consultCountryCommand.List();
            

            foreach (var itemARP in rowARPParameter)
            {
                TimeSpan tsReportado = DateTimeOffset.Parse(itemARP.HorasFin.ToString()).TimeOfDay - DateTimeOffset.Parse(itemARP.HoraInicio).TimeOfDay;
                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemARP.EmployeeCode);
                if (UserRow != null)
                {
                    var exceptionUser = _dataBaseService.UsersExceptions.FirstOrDefault(x => x.AssignedUserId == UserRow.IdUser && x.StartDate == DateTimeOffset.Parse(itemARP.FECHA_REP));
                    var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
                    var HorasARP = rowARPParameter.Where(co => DateTimeOffset.Parse(co.FECHA_REP)== DateTimeOffset.Parse(itemARP.FECHA_REP)).ToList();
                    var HorasARPGral = HorasARP.Select(x => double.Parse(x.totalHoras)).Sum();
                    if ((tsReportado.TotalHours + HorasARPGral) > (HorasLimiteDia + horasExceptuada))
                    {
                        itemARP.EstatusProceso = "NO_APLICA_X_LIMITE_HORAS";
                    }
                }
                else
                {
                    itemARP.EstatusProceso = "NO_APLICA_NO_USUARIO";
                }
            }

            foreach (var itemTSE in rowTSEParameter)
            {
                var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == itemTSE.EmployeeCode.Substring(itemTSE.EmployeeCode.Length - 3));
                var limitesCountryTSE = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == paisRegistro.IdCounty);
                var HorasLimiteDiaTSE = limitesCountryTSE.TargetTimeDay;
                TimeSpan tsReportadoTSE = DateTimeOffset.Parse(itemTSE.HorasFin.ToString()).TimeOfDay - DateTimeOffset.Parse(itemTSE.HoraInicio).TimeOfDay;
                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemTSE.EmployeeCode);
                if (UserRow != null)
                {
                    var exceptionUser = _dataBaseService.UsersExceptions.FirstOrDefault(x => x.AssignedUserId == UserRow.IdUser && x.StartDate == DateTimeOffset.Parse(itemTSE.FECHA_REP));
                    var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
                    var HorasTSE = rowTSEParameter.Where(co => DateTimeOffset.Parse(co.FECHA_REP) == DateTimeOffset.Parse(itemTSE.FECHA_REP)).ToList();
                    var HorasTSEGral = HorasTSE.Select(x => double.Parse(x.totalHoras)).Sum();
                    if ((tsReportadoTSE.TotalHours + HorasTSEGral) > (HorasLimiteDia + horasExceptuada))
                    {
                        itemTSE.EstatusProceso = "NO_APLICA_X_LIMITE_HORAS";
                    }
                }
                else
                {
                    itemTSE.EstatusProceso = "NO_APLICA_NO_USUARIO";
                }
            }

            foreach (var itemSTE in rowSTEParameter)
            {
                var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == itemSTE.EmployeeCode.Substring(itemSTE.EmployeeCode.Length - 3));
                var limitesCountrySTE = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == paisRegistro.IdCounty);
                var HorasLimiteDiaSTE = limitesCountrySTE.TargetTimeDay;
                TimeSpan tsReportadoSTE = DateTimeOffset.Parse(itemSTE.HorasFin.ToString()).TimeOfDay - DateTimeOffset.Parse(itemSTE.HoraInicio).TimeOfDay;
                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemSTE.EmployeeCode);
                if (UserRow != null)
                {
                    var exceptionUser = _dataBaseService.UsersExceptions.FirstOrDefault(x => x.AssignedUserId == UserRow.IdUser && x.StartDate == DateTimeOffset.Parse(itemSTE.FECHA_REP));
                    var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
                    var HorasSTE = rowSTEParameter.Where(co => DateTimeOffset.Parse(co.FECHA_REP) == DateTimeOffset.Parse(itemSTE.FECHA_REP)).ToList();
                    var HorasSTEGral = HorasSTE.Select(x => double.Parse(x.totalHoras)).Sum();
                    if ((tsReportadoSTE.TotalHours + HorasSTEGral) > (HorasLimiteDia + horasExceptuada))
                    {
                        itemSTE.EstatusProceso = "NO_APLICA_X_LIMITE_HORAS";
                    }
                }
                else
                {
                    itemSTE.EstatusProceso = "NO_APLICA_NO_USUARIO";
                }
            }

            await _dataBaseService.SaveAsync();

            //los sobrantes estan en overtime, insertar en portaldb y su history, como se hace en reporte de horas stand by
            //TODO

            var rowARPParameterFinal = _dataBaseService.ParametersArpInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
            var rowTSEParameterFinal = _dataBaseService.ParametersTseInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
            var rowSTEParameterFinal = _dataBaseService.ParametersSteInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();

            foreach (var itemARP in rowARPParameterFinal)
            {
                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemARP.EmployeeCode);
                var data = _dataBaseService.HorusReportEntity
                .Where(h => h.StartDate == DateTime.Parse(itemARP.FECHA_REP) && h.UserEntityId == UserRow.IdUser)
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemARP.HoraInicio, itemARP.HoraFin) ||
                (TimeInRange(h.StartTime, DateTime.Parse(itemARP.HoraInicio), DateTime.Parse(itemARP.HoraFin)) &&
                 TimeInRange(h.EndTime, DateTime.Parse(itemARP.HoraInicio), DateTime.Parse(itemARP.HoraFin))))
                .ToList();
                if (data.Count > 0)
                {
                    var horusReportRef = _mapper.Map<Domain.Entities.HorusReport.HorusReportEntity>(data[0]);
                    var assignmentRef = _dataBaseService.assignmentReports.
                         Where(x => x.HorusReportEntityId == horusReportRef.IdHorusReport)
                         .FirstOrDefault();

                    if (assignmentRef!.State != 3)
                    {
                        itemARP.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                    }
                }
            }

            foreach (var itemTSE in rowTSEParameterFinal)
            {
                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemTSE.EmployeeCode);
                var data = _dataBaseService.HorusReportEntity
                .Where(h => h.StartDate == DateTime.Parse(itemTSE.FECHA_REP) && h.UserEntityId == UserRow.IdUser)
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemTSE.HoraInicio, itemTSE.HoraFin) ||
                (TimeInRange(h.StartTime, DateTime.Parse(itemTSE.HoraInicio), DateTime.Parse(itemTSE.HoraFin)) &&
                 TimeInRange(h.EndTime, DateTime.Parse(itemTSE.HoraInicio), DateTime.Parse(itemTSE.HoraFin))))
                .ToList();
                if (data.Count > 0)
                {
                    var horusReportRef = _mapper.Map<Domain.Entities.HorusReport.HorusReportEntity>(data[0]);
                    var assignmentRef = _dataBaseService.assignmentReports.
                         Where(x => x.HorusReportEntityId == horusReportRef.IdHorusReport)
                         .FirstOrDefault();

                    if (assignmentRef!.State != 3)
                    {
                        itemTSE.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                    }
                }
            }

            foreach (var itemSTE in rowSTEParameterFinal)
            {
                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemSTE.EmployeeCode);
                var data = _dataBaseService.HorusReportEntity
                .Where(h => h.StartDate == DateTime.Parse(itemSTE.FECHA_REP) && h.UserEntityId == UserRow.IdUser)
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemSTE.HoraInicio, itemSTE.HoraFin) ||
                (TimeInRange(h.StartTime, DateTime.Parse(itemSTE.HoraInicio), DateTime.Parse(itemSTE.HoraFin)) &&
                 TimeInRange(h.EndTime, DateTime.Parse(itemSTE.HoraInicio), DateTime.Parse(itemSTE.HoraFin))))
                .ToList();
                if (data.Count > 0)
                {
                    var horusReportRef = _mapper.Map<Domain.Entities.HorusReport.HorusReportEntity>(data[0]);
                    var assignmentRef = _dataBaseService.assignmentReports.
                         Where(x => x.HorusReportEntityId == horusReportRef.IdHorusReport)
                         .FirstOrDefault();

                    if (assignmentRef!.State != 3)
                    {
                        itemSTE.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                    }
                }
            }


            await _dataBaseService.SaveAsync();

            var rowARPParameterGral = _dataBaseService.ParametersArpInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
            var rowTSEParameterGral = _dataBaseService.ParametersTseInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
            var rowSTEParameterGral = _dataBaseService.ParametersSteInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();

            List<HorusReportEntity> rowsHorusNew = new();
            HorusReportEntity rowAdd = new();
            List<Domain.Entities.AssignmentReport.AssignmentReport> rowAssignments = new();
            Domain.Entities.AssignmentReport.AssignmentReport rowAddAssig = new();

            //ARP
            //--------------------------------------------------------------------------
            foreach (var itemARPNew in rowARPParameterGral)
            {
                var Maxen = 0;
                Maxen = _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
                rowAdd = new() {
                    IdHorusReport = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    DateApprovalSystem = DateTime.Now,
                    NumberReport = Maxen + 1,
                    StartDate = DateTimeOffset.Parse(itemARPNew.FECHA_REP).Date
                };
                rowsHorusNew.Add(rowAdd);

                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemARPNew.EmployeeCode);

                rowAddAssig = new() {
                    IdAssignmentReport = Guid.NewGuid(),
                    HorusReportEntityId = rowAdd.IdHorusReport,
                    UserEntityId = UserRow.IdUser,
                    Description = "PROCESO_OVERTIME",
                    State= (byte)Enums.Enums.AprobacionPortalDB.Pendiente

                };
                rowAssignments.Add(rowAddAssig);
            }

            //TSE
            //------------------------------------------------------------------------
            foreach (var itemTSENew in rowTSEParameterGral)
            {
                var Maxen = 0;
                Maxen = _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
                rowAdd = new()
                {
                    IdHorusReport = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    DateApprovalSystem = DateTime.Now,
                    NumberReport = Maxen + 1,
                    StartDate = DateTimeOffset.Parse(itemTSENew.FECHA_REP).Date
                };
                rowsHorusNew.Add(rowAdd);

                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemTSENew.EmployeeCode);

                rowAddAssig = new()
                {
                    IdAssignmentReport = Guid.NewGuid(),
                    HorusReportEntityId = rowAdd.IdHorusReport,
                    UserEntityId = UserRow.IdUser,
                    Description = "PROCESO_OVERTIME",
                    State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente

                };
                rowAssignments.Add(rowAddAssig);
            }
            //STE
            //------------------------------------------------------------------------
            foreach (var itemSTENew in rowSTEParameterGral)
            {
                var Maxen = 0;
                Maxen = _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
                rowAdd = new()
                {
                    IdHorusReport = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    DateApprovalSystem = DateTime.Now,
                    NumberReport = Maxen + 1,
                    StartDate = DateTimeOffset.Parse(itemSTENew.FECHA_REP).Date
                };
                rowsHorusNew.Add(rowAdd);

                var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemSTENew.EmployeeCode);

                rowAddAssig = new()
                {
                    IdAssignmentReport = Guid.NewGuid(),
                    HorusReportEntityId = rowAdd.IdHorusReport,
                    UserEntityId = UserRow.IdUser,
                    Description = "PROCESO_OVERTIME",
                    State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente

                };
                rowAssignments.Add(rowAddAssig);
            }

            _dataBaseService.HorusReportEntity.AddRange(rowsHorusNew);
            await _dataBaseService.SaveAsync();

            
            _dataBaseService.assignmentReports.AddRange(rowAssignments);

            await _dataBaseService.SaveAsync();

            return true;
        }
    }
}
