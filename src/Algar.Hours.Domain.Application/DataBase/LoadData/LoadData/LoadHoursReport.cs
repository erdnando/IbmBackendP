using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load;
using Algar.Hours.Application.DataBase.PortalDB.Commands;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.User.Commands.GetManager;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities;
using Algar.Hours.Domain.Entities.Aprobador;
using Algar.Hours.Domain.Entities.AssignmentReport;
using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Festivos;
using Algar.Hours.Domain.Entities.Gerentes;
using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.PaisRelacionGMT;
using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Algar.Hours.Domain.Entities.User;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Algar.Hours.Domain.Entities.WorkdayException;
using AutoMapper;
using Azure;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using EFCore.BulkExtensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Utilities;
using Sustainsys.Saml2.Metadata;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public static class Extensions
    {
        public static List<List<T>> partition<T>(this List<T> values, int chunkSize)
        {
            return values.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }

    public class LoadHoursReport : ILoadHoursReport
    {
       
        private readonly IDataBaseService _dataBaseService;
        private readonly IConfiguration _configuration;


        private readonly IMapper _mapper;
        private IConsultCountryCommand _consultCountryCommand;
        private IEmailCommand _emailCommand;
        private ICreateLogCommand _logCommand;

        public LoadHoursReport(IDataBaseService dataBaseService, IMapper mapper, IConsultCountryCommand consultCountryCommand, IEmailCommand emailCommand, IConfiguration configuration, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _configuration = configuration;
            _mapper = mapper;
            _consultCountryCommand = consultCountryCommand;
            _emailCommand = emailCommand;
            _logCommand = logCommand;
        }

        public async Task<List<ARPLoadEntity>> List()
        {
            var entities = await _dataBaseService.ARPLoadEntity.Include(x => x.userEntity).OrderByDescending(x => x.FechaCreacion)
                .ToListAsync();
            return entities;
        }

        public async Task<List<ParametersArpInitialEntity>> ArpParametersList(Guid idLoad)
        {
            var entities = await _dataBaseService.ParametersArpInitialEntity.Where(x => x.IdCarga == idLoad)
                .ToListAsync();
            return entities;
        }

        public async Task<List<ParametersTseInitialEntity>> TseParametersList(Guid idLoad)
        {
            var entities = await _dataBaseService.ParametersTseInitialEntity.Where(x => x.IdCarga == idLoad)
                .ToListAsync();
            return entities;
        }

        public async Task<List<ParametersSteInitialEntity>> SteParametersList(Guid idLoad)
        {
            var entities = await _dataBaseService.ParametersSteInitialEntity.Where(x => x.IdCarga == idLoad)
                .ToListAsync();
            return entities;
        }

        public async Task<ARPLoadEntity> Consult(Guid id)
        {
            var loadEntity = _dataBaseService.ARPLoadEntity.Where(d => d.IdArpLoad == id).FirstOrDefault();
            /*var entity = _mapper.Map<HorusReportModel>(data);*/
            return loadEntity;
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

        


        public async Task<CountsCarga> CargaAvance(string idCarga)
        {
            var contadores = new CountsCarga();
            if (idCarga== "00000000-0000-0000-0000-000000000000")
            {
                contadores.arp =0;
                contadores.tse = 0;
                contadores.ste = 0;
                contadores.total = 0;
                contadores.estadoCarga =0;
                return contadores;
            }
           


            int arpEnProceso = _dataBaseService.ParametersArpInitialEntity.Where(e => e.IdCarga == new Guid(idCarga)).ToList().Count();
            int tseEnProceso = _dataBaseService.ParametersTseInitialEntity.Where(e => e.IdCarga == new Guid(idCarga)).ToList().Count();
            int steEnProceso = _dataBaseService.ParametersSteInitialEntity.Where(e => e.IdCarga == new Guid(idCarga)).ToList().Count();
            var cargaRef = _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(idCarga)).FirstOrDefault();

            string mensaje = cargaRef!.EstadoCarga != null ? cargaRef.EstadoCarga : "Cargando...";

            var aRPCarga = Int32.Parse(cargaRef!.ARPCarga == "" ? "0" : cargaRef.ARPCarga);
            var tSECarga = Int32.Parse(cargaRef!.TSECarga == "" ? "0" : cargaRef.TSECarga);
            var sTECarga = Int32.Parse(cargaRef!.STECarga == "" ? "0" : cargaRef.STECarga);

            contadores.arp = aRPCarga != 0 ? (Int32)Math.Ceiling((double)arpEnProceso * 100 / aRPCarga) : 0;
            contadores.tse = tSECarga != 0 ? (Int32)Math.Ceiling((double)tseEnProceso * 100 / tSECarga) : 0;
            contadores.ste = sTECarga != 0 ? (Int32)Math.Ceiling((double)steEnProceso * 100 / sTECarga) : 0;
            contadores.total = (contadores.arp + contadores.tse + contadores.ste) / 3;
            contadores.estadoCarga = cargaRef.Estado;

            contadores.mensaje = mensaje;

            return contadores;
        }


        public async Task<ResponseData<string>> GeneraCarga()
        {
            try
            {
                var hasTimeZones = _dataBaseService.UserZonaHoraria.Count() > 0;
                if (!hasTimeZones) {
                    return new ResponseData<string> { 
                        Data = null,
                        Error = true,
                        Message = "No cuenta con zonas horarias cargadas"
                    };
                }

                ARPLoadEntity aRPLoadEntity = new ARPLoadEntity
                {
                    Estado = 1,//1 corriendo 2 terminado  3 error 
                    FechaCreacion = DateTime.Now,
                    IdArpLoad = Guid.NewGuid(),
                    userEntityId = Guid.Parse("3696718D-D05A-4831-96CE-ED500C5BBC97"),
                    ARPCarga = "0",
                    ARPXHorario = "0",
                    ARPXOverlaping = "0",
                    ARPXOvertime = "0",
                    STECarga = "0",
                    STEEXOvertime = "0",
                    STEXHorario = "0",
                    STEXOverlaping = "0",
                    TSECarga = "0",
                    TSEXHorario = "0",
                    TSEXOverlaping = "0",
                    TSEXOvertime = "0",
                    ARPXProceso = "0",
                    STEXProceso = "0",
                    TSEXProceso = "0",
                    ARPOmitidosXDuplicidad="0",
                    TSEOmitidosXDuplicidad="0",
                    STEOmitidosXDuplicidad="0",
                    ARPOmitidos="0",
                    TSEOmitidos="0",
                    STEOmitidos="0",
                    STEXDatosNovalidos="0",
                    ARPXDatosNovalidos="0",
                    TSEXDatosNovalidos = "0",
                    EstadoCarga="Preparando carga...",

                };

                 _dataBaseService.ARPLoadEntity.AddAsync(aRPLoadEntity);
                await _dataBaseService.SaveAsync();

                return new ResponseData<string> {
                    Data = aRPLoadEntity.IdArpLoad.ToString(),
                    Error = false,
                    Message = ""
                };
            }catch(Exception ex)
            {
                return new ResponseData<string> {
                    Data = "-1",
                    Error = true,
                    Message = ""
                };
            }
        }

        public async Task<ResponseData<ARPLoadEntity>> CancelarCarga(string idCarga)
        {
            try
            {
                ARPLoadEntity? arpLoad = _dataBaseService.ARPLoadEntity.Find(Guid.Parse(idCarga));
                if (arpLoad == null) {
                    return new ResponseData<ARPLoadEntity>
                    {
                        Data = null,
                        Error = false,
                        Message = "La carga no existe"
                    };
                }

                arpLoad.Estado = 4; // Cancelada
                await _dataBaseService.SaveAsync();

                return new ResponseData<ARPLoadEntity>
                {
                    Data = arpLoad,
                    Error = false,
                    Message = "Carga cancelada"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<ARPLoadEntity>
                {
                    Data = null,
                    Error = true,
                    Message = "Error al cancelar carga"
                };
            }
        }

        public async Task<string> LoadARP(LoadJsonPais model)
        {
            await _logCommand.Log(model.idUserEntiyId,"Ejecuta carga OVERTIME ARP",model);

            await updateCargaStatus(model.IdCarga, "Procesando ARP...");



            try
            {

                Int64 counter = 0;
                List<ParametersArpInitialEntity> listParametersInitialEntity = new();
                List<HorusReportEntity> listARPHorusReportEntity = new();
                var semanahorario = new DateTimeOffset();
                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();
                List<FestivosEntity> esfestivos = new();


                //Serializa la carga completa del excel
                List<ARPLoadDetailEntity> datosARPExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ARPLoadDetailEntity>>(model.Data.ToJsonString());

                //Remueve duplicados 
                var datosARPExcel = datosARPExcelFull.DistinctBy(m => new { m.ID_EMPLEADO, m.FECHA_REP, m.HORA_INICIO, m.HORA_FIN,m.ESTADO }).ToList();

                //Setting duplicados metrica en ARP
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPOmitidosXDuplicidad = (datosARPExcelFull.Count() - datosARPExcel.Count()).ToString());
                await _dataBaseService.SaveAsync();


                //Set metrica de la carga en ARPLoadEntity
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == Guid.Parse(model.IdCarga)).ToList().
                                              ForEach(x => x.ARPCarga = datosARPExcel.Count.ToString());

                //SEt metrica de omitidos en ARPLoadEntity
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == Guid.Parse(model.IdCarga)).ToList().
                                              ForEach(x => x.ARPOmitidos = (datosARPExcelFull.Count - datosARPExcel.Count).ToString());

                await _dataBaseService.SaveAsync();

                //Obteniendo politicas overtime
                List<string> politicaOvertime = new() { "VACATIONS", "ABSENCE", "HOLIDAY", "STAND BY" };


                //Obteniendo listado de paises
                var listaCountries = await _consultCountryCommand.List();

                //Obtiene listado de festivos
                esfestivos = _dataBaseService.FestivosEntity.ToList();


                var horariosGMT = await _dataBaseService.PaisRelacionGMTEntity.Where(e => e.NameCountrySelected == model.PaisSel).ToListAsync();


                //Busca horarios configurados para este empleado en la semana y dia obtenido del excel de carga
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();

                //busca Gral Paises
                var paisGeneral = await _dataBaseService.CountryEntity.ToListAsync();


                //Aplica validaciones del PROCESO TLS para overtime
                foreach (var entity in datosARPExcel)
                {
                    var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == entity.PAIS);  // entity.ID_EMPLEADO.Substring(entity.ID_EMPLEADO.Length - 3));
                    var arpx = fHomologaARP(entity!);
                    arpx.ID_EMPLEADO= entity.ID_EMPLEADO+ entity.PAIS;
                    var parametersARP = new ParametersArpInitialEntity();

                    parametersARP.IdParametersInitialEntity = Guid.NewGuid();
                    parametersARP.EstatusProceso = "";
                    parametersARP.FECHA_REP = arpx.FECHA_REP;
                    parametersARP.TOTAL_MINUTOS = arpx.TOTAL_MINUTOS;
                    parametersARP.totalHoras = getHoras(arpx.TOTAL_MINUTOS);
                    parametersARP.HorasInicio = 0;
                    parametersARP.HorasFin = 0;
                    parametersARP.EmployeeCode = arpx.ID_EMPLEADO;//ARP le falta el codigo de pais
                    parametersARP.IdCarga = new Guid(model.IdCarga);
                    parametersARP.HoraInicio = arpx.HORA_INICIO;//hora reportada inicio
                    parametersARP.HoraFin = arpx.HORA_FIN;//hora reportada fin
                    parametersARP.OutIme = "N";
                    parametersARP.Semana = 0;
                    parametersARP.HoraInicioHoraio = "0";
                    parametersARP.HoraFinHorario = "0";
                    parametersARP.OverTime = "N";
                    parametersARP.Anio = semanahorario.Year.ToString();
                    parametersARP.Festivo = "N";
                    parametersARP.Estado = "";
                    parametersARP.Reporte = arpx.DOC_NUM;
                    parametersARP.EstatusOrigen = arpx.ESTADO.Trim().ToUpper();
                    parametersARP.Actividad = arpx.ACTIVIDAD;

                    if (politicaOvertime.IndexOf(arpx.ACTIVIDAD.Trim().ToUpper()) >= 0)
                    {
                        //---------------NO_APLICA_X_OVERTIME----------
                        parametersARP.EstatusProceso = "NO_APLICA_X_OVERTIME";
                        listParametersInitialEntity.Add(parametersARP);
                        continue;
                    }

                    if (paisRegistro == null)
                    {
                        parametersARP.EstatusProceso = "NO_APLICA_X_PAIS";
                        listParametersInitialEntity.Add(parametersARP);
                        continue;
                    }


                    if (arpx.FECHA_REP == null)
                    {
                        parametersARP.EstatusProceso = "NO_APLICA_X_FALTA_DATOS";
                        listParametersInitialEntity.Add(parametersARP);
                        continue;
                    }


                    var arp = validaHoraGMT(arpx, horariosGMT, paisGeneral);
                    arp.ARPLoadEntityId = new Guid(model.IdCarga);


                    try
                    {
                        //semanahorario = DateTimeOffset.ParseExact(arp.FECHA_REP, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        semanahorario = DateTimeOffset.ParseExact(arp.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            semanahorario = DateTimeOffset.ParseExact(arp.FECHA_REP, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {

                            parametersARP.EstatusProceso = "NO_APLICA_X_FORMATO_FECHA";
                            listParametersInitialEntity.Add(parametersARP);
                            continue;
                        }
                       
                    }

                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                    //Obtiene horario para este empleado en la fecha del evento
                    var horario = Lsthorario.FindAll(x => x.UserEntity.EmployeeCode == arp.ID_EMPLEADO && x.week == Semana.ToString());// && x.FechaWorking == semanahorario.DateTime);
                    int indexHorario = -1;
                    for (int i=0;i<horario.Count;i++)
                    {
                        if (horario[i].FechaWorking.UtcDateTime.ToString("dd/MM/yyyy 00:00:00") == arp.FECHA_REP)
                        {
                            indexHorario = i;
                        }
                        
                    }

                    var esfestivo = esfestivos.FirstOrDefault(x => x.DiaFestivo.UtcDateTime.ToString("dd/MM/yyyy") == semanahorario.UtcDateTime.ToString("dd/MM/yyyy") && x.CountryId == paisRegistro!.IdCounty);

                    parametersARP.FECHA_REP = arp.FECHA_REP;
                    parametersARP.EstatusProceso = "EN_OVERTIME";
                    parametersARP.Anio = semanahorario.Year.ToString();
                    parametersARP.Festivo = esfestivo != null ? "Y" : "N";
                    parametersARP.Semana = Semana;

                    if (parametersARP.Festivo == "Y")
                    {
                        listParametersInitialEntity.Add(parametersARP);
                        continue;
                    }


                    var hasHorarioSemana = false;
                    var hasHorarioDia = false;
                    var previosAndPos = new List<double>();
                    if (horario.Count >0)
                    {
                        hasHorarioSemana = true;
                        hasHorarioDia = indexHorario >= 0;

                        if (indexHorario>=0)
                        {
                            previosAndPos = getPreviasAndPosHorario(arp.HORA_INICIO, arp.HORA_FIN, horario[indexHorario].HoraInicio, horario[indexHorario].HoraFin);
                        }
                        /*else
                        {
                            parametersARP.EstatusProceso = "NO_APLICA_X_HORARIO";
                            listParametersInitialEntity.Add(parametersARP);
                            continue;
                        }*/
                        
                    }
                    else
                    {
                        previosAndPos.Add(0.0);
                        previosAndPos.Add(0.0);
                    }

                    if (horario.Count > 0)
                    {
                        /*if (indexHorario == -1)
                        {
                            //NO hay coincidencia en la semana del horario
                            parametersARP.EstatusProceso = "NO_APLICA_X_HORARIO";
                            listParametersInitialEntity.Add(parametersARP);
                            continue;
                        }*/

                        if (indexHorario >= 0) {
                            //fechas registadas en el horario asignado
                            parametersARP.HoraInicioHoraio = horario[indexHorario].HoraInicio == null ? "0" : horario[indexHorario].HoraInicio;
                            parametersARP.HoraFinHorario = horario[indexHorario].HoraFin == null ? "0" : horario[indexHorario].HoraFin;
                        }
                        
                    }

                    if (!hasHorarioSemana && !hasHorarioDia) {
                        //NO hay horario
                        parametersARP.EstatusProceso = "NO_APLICA_X_HORARIO";
                        parametersARP.Problemas = $"El empleado no cuenta con registro de horarios para la fecha de la actividad"; 
                        parametersARP.Acciones = $"Solicite a un aprobador actualizar sus horarios de trabajo";
                        listParametersInitialEntity.Add(parametersARP);
                        continue;
                    }

                    
                    //Evaluating schedullers
                    //-----------------------------------------------------------------------------------------------

                    var excelStartTime = TimeSpan.Parse(parametersARP.HoraInicio);
                    var excelEndTime = TimeSpan.Parse(parametersARP.HoraFin);

                    if (hasHorarioSemana && !hasHorarioDia)
                    {
                        parametersARP.HoraInicio = excelStartTime.ToString(@"hh\:mm");
                        parametersARP.HoraFin = excelEndTime.ToString(@"hh\:mm");
                        parametersARP.Reporte = parametersARP.Reporte;
                        listParametersInitialEntity.Add(parametersARP);
                        continue;
                    }

                    var scheduleStartTime = TimeSpan.Parse(horario[indexHorario].HoraInicio);
                    var scheduleEndTime = TimeSpan.Parse(horario[indexHorario].HoraFin);
                    var beforeTime = scheduleStartTime - excelStartTime;
                    var afterTime = excelEndTime - scheduleEndTime;

                    bool bOvertime = false;
                    //listParametersInitialEntity.Add(parametersARP);
                    if (beforeTime.TotalMinutes > 0)
                    {

                        //OVERTIME 1
                        parametersARP.HoraInicio = excelStartTime.ToString(@"hh\:mm");
                        parametersARP.HoraFin = (excelEndTime < scheduleStartTime ? excelEndTime : scheduleStartTime).ToString(@"hh\:mm");
                        string[] r1 = parametersARP.HoraInicio.Split(":");
                        string[] r2 = parametersARP.HoraFin.Split(":");
                        TimeSpan overtimeTime = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                        parametersARP.TOTAL_MINUTOS = overtimeTime.TotalMinutes.ToString();
                        parametersARP.totalHoras = overtimeTime.TotalHours.ToString();
                        parametersARP.Reporte = parametersARP.Reporte;
                        listParametersInitialEntity.Add(parametersARP);
                        bOvertime = true;
                    }
                    
                    if (afterTime.TotalMinutes > 0)
                    {
                        TimeSpan horaInicio = excelStartTime > scheduleEndTime ? excelStartTime : scheduleEndTime;
                        //OVERTIME2
                        var parametersARP2 = new ParametersArpInitialEntity();
                        parametersARP2.HoraInicio = horaInicio.ToString(@"hh\:mm");
                        parametersARP2.HoraFin = excelEndTime.ToString(@"hh\:mm");
                        string[] r1 = parametersARP2.HoraInicio.Split(":");
                        string[] r2 = parametersARP2.HoraFin.Split(":");
                        TimeSpan overtimeTime = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                        parametersARP2.TOTAL_MINUTOS = overtimeTime.TotalMinutes.ToString();
                        parametersARP2.totalHoras = overtimeTime.TotalHours.ToString();
                        parametersARP2.Reporte = parametersARP.Reporte + (bOvertime ? "-R2": "");
                        parametersARP2.Actividad = parametersARP.Actividad;

                        parametersARP2.Anio = parametersARP.Anio;
                        parametersARP2.FECHA_REP = parametersARP.FECHA_REP;
                        parametersARP2.EstatusProceso = parametersARP.EstatusProceso;
                        parametersARP2.Estado = parametersARP.Estado;
                        parametersARP2.EmployeeCode = parametersARP.EmployeeCode;
                        parametersARP2.EstatusOrigen = parametersARP.EstatusOrigen;
                        parametersARP2.Festivo = parametersARP.Festivo;
                        parametersARP2.HoraFinHorario = parametersARP.HoraFinHorario;
                        parametersARP2.HoraInicioHoraio = parametersARP.HoraInicioHoraio;
                        parametersARP2.HorasFin = parametersARP.HorasFin;
                        parametersARP2.HorasInicio = parametersARP.HorasInicio;
                        parametersARP2.IdCarga = parametersARP.IdCarga;
                        parametersARP2.IdParametersInitialEntity= Guid.NewGuid();
                        parametersARP2.OutIme = parametersARP.OutIme;
                        parametersARP2.OverTime = parametersARP.OverTime;
                        parametersARP2.Semana = parametersARP.Semana;

                        listParametersInitialEntity.Add(parametersARP2);
                        bOvertime = true;
                    }
                    
                    if(!bOvertime)
                    {
                        //NO hay horario
                        parametersARP.EstatusProceso = "NO_APLICA_X_HORARIO_EMPLEADO";
                        parametersARP.Problemas = $"El registro se encuentra dentro del horario de trabajo del empleado";
                        parametersARP.Acciones = $"Verifique y actualice el reporte en ARP";

                        listParametersInitialEntity.Add(parametersARP);
                        continue;
                    }
                    //--------------------------------------------------------------------------------------------------
                   // listParametersInitialEntity.Add(parametersARP);




                }

                await updateCargaStatus(model.IdCarga, "Procesando " + listParametersInitialEntity.Count() + " de " + listParametersInitialEntity.Count + " registros ARP...");
                _dataBaseService.Context().BulkInsert(listParametersInitialEntity);
                await updateCargaStatus(model.IdCarga, "ARP terminado...");

                return model.IdCarga.ToString();

            }
            catch (Exception ex)
            {
                await updateCargaStatus(model.IdCarga, "Error carga en ARP..." + ex.Message);
                return "00000000-0000-0000-0000-000000000000";
            }

        }

        


        private async Task<bool> LoadGenericTse(List<ParametersTseInitialEntity> partition, string endpoint)
        {


            try
            {
                HttpClient client = new HttpClient();



                var opt = new JsonSerializerOptions() { WriteIndented = true };
                string strJson = System.Text.Json.JsonSerializer.Serialize<List<ParametersTseInitialEntity>>(partition, opt);


                HttpResponseMessage responseMessage = client.PostAsync(_configuration["Url"] + "Load/UploadHorizontalTSE" + endpoint, new StringContent(strJson, Encoding.UTF8, "application/json")).Result;


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        private async Task<bool> LoadGenericSte(List<ParametersSteInitialEntity> partition, string endpoint)
        {


            try
            {
                HttpClient client = new HttpClient();



                var opt = new JsonSerializerOptions() { WriteIndented = true };
                string strJson = System.Text.Json.JsonSerializer.Serialize<List<ParametersSteInitialEntity>>(partition, opt);


                HttpResponseMessage responseMessage = client.PostAsync(_configuration["Url"] + "Load/UploadHorizontalSTE" + endpoint, new StringContent(strJson, Encoding.UTF8, "application/json")).Result;


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private async Task<bool> LoadGeneric(List<ParametersArpInitialEntity> partition,string endpoint)
        {
            
            
            try
            {
                HttpClient client = new HttpClient();

             

                var opt = new JsonSerializerOptions() { WriteIndented = true };
                string strJson = System.Text.Json.JsonSerializer.Serialize<List<ParametersArpInitialEntity>>(partition, opt);


                HttpResponseMessage responseMessage = client.PostAsync(_configuration["Url"] + "Load/UploadHorizontalARP" + endpoint, new StringContent(strJson, Encoding.UTF8, "application/json")).Result;
               

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        private ARPLoadDetailEntity validaHoraGMT(ARPLoadDetailEntity arpRegistro, List<PaisRelacionGMTEntity> paisGMT, List<CountryEntity> paisGeneral)
        {
            var paisByCode = paisGeneral.FirstOrDefault(x => x.CodigoPais.Trim().ToUpper() == arpRegistro.PAIS.Trim().ToUpper());
            var paisComparacion = paisGMT.FirstOrDefault(e => e.NameCountryCompare == paisByCode.NameCountry);
            try
            {
                var HoraInicioOrigin = DateTime.Parse(arpRegistro.HORA_INICIO);
               // var horaActualizada = HoraInicioOrigin.AddHours(paisComparacion.TimeDifference);
                var horaActualizada = HoraInicioOrigin.AddHours(0);
                arpRegistro.HORA_INICIO = horaActualizada.ToString("HH:mm");

            }
            catch (Exception ex)
            {
                arpRegistro.HORA_INICIO = "0";
            }

            try
            {
                var HoraFinOrigin = DateTime.Parse(arpRegistro.HORA_FIN);
                //var horaFinActualizada = HoraFinOrigin.AddHours(paisComparacion.TimeDifference);
                var horaFinActualizada = HoraFinOrigin.AddHours(0);
                arpRegistro.HORA_FIN = horaFinActualizada.ToString("HH:mm");
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

               


            }
           
            //verificar que si se sobrepone alguna hora y enviar el correo electronico 
            //si tiene la misma hora y fecha y id empleado se actualiza la hora

            //en caso que no se almacena en la base de datos
        }
        private bool TimeRangesOverlap(string existingStartTime, string existingEndTime, string newStartTime, string newEndTime)
        {
            /*DateTime startTimeExisting = DateTime.Parse(existingStartTime);*/
            string[] r = existingStartTime.Split(":");
            DateTime startTimeExisting = DateTime.Parse("00:00:00");
            startTimeExisting = startTimeExisting.AddHours(int.Parse(r[0]));
            startTimeExisting = startTimeExisting.AddMinutes(int.Parse(r[1]));

            r = existingEndTime.Split(":");
            DateTime endTimeExisting = DateTime.Parse("00:00:00");
            endTimeExisting = endTimeExisting.AddHours(int.Parse(r[0]));
            endTimeExisting = endTimeExisting.AddMinutes(int.Parse(r[1]));

            r = newStartTime.Split(":");
            DateTime startTimeNew = DateTime.Parse("00:00:00");
            startTimeNew = startTimeNew.AddHours(int.Parse(r[0]));
            startTimeNew = startTimeNew.AddMinutes(int.Parse(r[1]));

            r = newEndTime.Split(":");
            DateTime endTimeNew = DateTime.Parse("00:00:00");
            endTimeNew = endTimeNew.AddHours(int.Parse(r[0]));
            endTimeNew = endTimeNew.AddMinutes(int.Parse(r[1]));

            return (startTimeNew < endTimeExisting && endTimeNew > startTimeExisting);
        }

        private bool TimeInRange(string timeString, DateTime rangeStart, DateTime rangeEnd)
        {
            string[] r = timeString.Split(":");
            DateTime time = DateTime.Parse("00:00:00");
            time = time.AddHours(int.Parse(r[0]));
            time = time.AddMinutes(int.Parse(r[1]));
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
            await _logCommand.Log(model.idUserEntiyId, "Ejecuta carga OVERTIME TSE", model);

            await updateCargaStatus(model.IdCarga, "Procesando TSE...");
            int counter = 0;

            var semanahorario = new DateTimeOffset();

            CultureInfo cul = CultureInfo.CurrentCulture;
            List<HorarioReturn> fueraH = new List<HorarioReturn>();
            List<FestivosEntity> listFestivos = new();
            List<string> politicaOvertime = new() { "DRAFT" };

            try { 
            
                List<TSELoadEntity> datosTSEExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TSELoadEntity>>(model.Data.ToJsonString());
               
                //Remueve duplicados 
                var datosTSEExcel = datosTSEExcelFull.DistinctBy(m => new { m.NumeroEmpleado, m.StartTime, m.EndTime,m.Status }).ToList();

                //Setting duplicados metrica en TSE
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEOmitidosXDuplicidad = (datosTSEExcelFull.Count() - datosTSEExcel.Count()).ToString());
                await _dataBaseService.SaveAsync();


                try
                {
                    _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                                                 ForEach(x => x.TSEOmitidos = (datosTSEExcelFull.Count - datosTSEExcel.Count).ToString());
                    await _dataBaseService.SaveAsync();

                    //Setting number of register on ARPLoadEntity
                    _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                                  ForEach(x => x.TSECarga = datosTSEExcelFull.Count().ToString());
                    await _dataBaseService.SaveAsync();
                }
                catch(Exception ex)
                {

                }
                

                datosTSEExcel.Where(e => string.IsNullOrEmpty(e.AccountCMRNumber) == true || e.AccountCMRNumber == "N/A").ToList().ForEach(x => x.AccountCMRNumber = "1234");
                
                //Getting list of countries
                var listaCountries = await _consultCountryCommand.List();

                //Getting list of holidays
                listFestivos = _dataBaseService.FestivosEntity.ToList();
               
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();
                var horariosGMT = await _dataBaseService.PaisRelacionGMTEntity.Where(e => e.NameCountrySelected == model.PaisSel).ToListAsync();
                var paisGeneral = await _dataBaseService.CountryEntity.ToListAsync();

                List<ParametersTseInitialEntity> listParametersInitialEntity = new();

                try
                {

               
                foreach (var registrox in datosTSEExcel)
                {
                        var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == registrox.NumeroEmpleado.Substring(registrox.NumeroEmpleado.Length - 3));

                        //var tse = fHomologaTSE(registrox, horariosGMT, paisRegistro!);
                        var tse = fHomologaTSE(registrox, model.PaisSel, paisRegistro!);
                        
                        var parametersTse = new ParametersTseInitialEntity();
                        parametersTse.IdParamTSEInitialId = Guid.NewGuid();
                        parametersTse.EstatusProceso = "";
                        parametersTse.FECHA_REP = tse.StartTime;
                        parametersTse.TOTAL_MINUTOS = getMins(registrox.DurationInHours);
                        parametersTse.totalHoras = registrox.DurationInHours;
                        parametersTse.HorasInicio = 0;
                        parametersTse.HorasFin = 0;
                        parametersTse.EmployeeCode = registrox.NumeroEmpleado;
                        parametersTse.IdCarga = new Guid(model.IdCarga);
                        parametersTse.HoraInicio = "0";
                        parametersTse.HoraFin = "0";
                        parametersTse.OutIme = "N";
                        parametersTse.Semana = 0;
                        parametersTse.HoraInicioHoraio = "0";
                        parametersTse.HoraFinHorario = "0";
                        parametersTse.OverTime = "N";
                        parametersTse.Anio = semanahorario.Year.ToString();
                        parametersTse.Festivo = "N";
                        parametersTse.Estado = "";
                        parametersTse.Reporte = $"{registrox.WorkOrder}";
                        parametersTse.EstatusOrigen = registrox.Status.Trim().ToUpper();
                        parametersTse.Actividad = registrox.WorkOrder;

                        if (politicaOvertime.IndexOf(tse.Status.Trim().ToUpper()) >= 0)
                        {
                            //---------------NO_APLICA_X_OVERTIME----------
                            parametersTse.EstatusProceso = "NO_APLICA_X_OVERTIME";
                            listParametersInitialEntity.Add(parametersTse);
                            continue;
                        }

                        //valida pais
                        if (paisRegistro == null)
                        {
                            //PAIS no valido
                            parametersTse.EstatusProceso = "NO_APLICA_X_PAIS";
                            listParametersInitialEntity.Add(parametersTse);
                            continue;
                        }

                        if (tse.StartTime == null || tse.EndTime == null)
                        {
                            parametersTse.EstatusProceso = "NO_APLICA_X_FALTA_DATOS";
                            listParametersInitialEntity.Add(parametersTse);
                            continue;
                        }


                        var excelStartDateTime = DateTime.ParseExact($"{tse.StartTime.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        var excelEndDateTime = DateTime.ParseExact($"{tse.EndTime.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        var totalDays = Math.Ceiling((excelEndDateTime - excelStartDateTime).TotalDays) + 1;

                        excelStartDateTime = DateTime.ParseExact($"{tse.StartTime.Substring(0, 10)} {tse.StartHours}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        excelEndDateTime = DateTime.ParseExact($"{tse.EndTime.Substring(0, 10)} {tse.EndHours}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                        int overtimeCount = 0;
                        for (int i = 0; i < totalDays; i++) {
                            DateTime startDateTime = (new DateTime(excelStartDateTime.Year, excelStartDateTime.Month, excelStartDateTime.Day, 0, 0, 0)).AddDays(i);
                            DateTime endDateTime;
                            var horaFin = "";
                            if (totalDays > 1 && i < (totalDays - 1))
                            {
                                endDateTime = startDateTime.AddHours(24);
                                horaFin = "23:59";
                            }
                            else
                            {
                                endDateTime = excelEndDateTime;
                                horaFin = endDateTime.ToString("HH:mm");
                            }
                            startDateTime = i == 0 ? startDateTime.AddHours(excelStartDateTime.Hour).AddMinutes(excelStartDateTime.Minute).AddSeconds(excelStartDateTime.Second) : startDateTime;

                            parametersTse = new ParametersTseInitialEntity();
                            parametersTse.IdParamTSEInitialId = Guid.NewGuid();
                            parametersTse.EstatusProceso = "";
                            parametersTse.FECHA_REP = startDateTime.ToString("dd/MM/yyyy 00:00:00");
                            parametersTse.TOTAL_MINUTOS = getMins(registrox.DurationInHours);
                            parametersTse.totalHoras = registrox.DurationInHours;
                            parametersTse.HorasInicio = 0;
                            parametersTse.HorasFin = 0;
                            parametersTse.EmployeeCode = registrox.NumeroEmpleado;
                            parametersTse.IdCarga = new Guid(model.IdCarga);
                            parametersTse.HoraInicio = startDateTime.ToString("HH:mm");
                            parametersTse.HoraFin = horaFin; 
                            parametersTse.OutIme = "N";
                            parametersTse.Semana = 0;
                            parametersTse.HoraInicioHoraio = "0";
                            parametersTse.HoraFinHorario = "0";
                            parametersTse.OverTime = "N";
                            parametersTse.Anio = semanahorario.Year.ToString();
                            parametersTse.Festivo = "N";
                            parametersTse.Estado = "";
                            parametersTse.Reporte = $"{registrox.WorkOrder}-R{overtimeCount}";
                            parametersTse.EstatusOrigen = registrox.Status.Trim().ToUpper();
                            parametersTse.Actividad = registrox.WorkOrder;

                            //valida pais
                            if (paisRegistro == null)
                            {
                                //PAIS no valido
                                parametersTse.EstatusProceso = "NO_APLICA_X_PAIS";
                                listParametersInitialEntity.Add(parametersTse);
                                break;
                            }

                            if (tse.StartTime == null || tse.EndTime == null)
                            {
                                parametersTse.EstatusProceso = "NO_APLICA_X_FALTA_DATOS_INICIO_FIN";
                                listParametersInitialEntity.Add(parametersTse);
                                continue;
                            }

                            semanahorario = DateTimeOffset.ParseExact(startDateTime.ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                            int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                            bool bValidacionHorario = false;

                            var horario = Lsthorario.FindAll(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString() && x.Ano == semanahorario.Year.ToString());

                            int indexHorario = -1;
                            for (int j = 0; j < horario.Count; j++)
                            {
                                if (horario[j].FechaWorking.UtcDateTime.ToString("dd/MM/yyyy 00:00:00") == startDateTime.ToString("dd/MM/yyyy 00:00:00"))
                                {
                                    indexHorario = j;
                                }

                            }

                            var esfestivo = listFestivos.FirstOrDefault(x => x.DiaFestivo.UtcDateTime.ToString("dd/MM/yyyy") == semanahorario.UtcDateTime.ToString("dd/MM/yyyy") && x.CountryId == paisRegistro!.IdCounty);

                            parametersTse.EstatusProceso = "EN_OVERTIME";
                            parametersTse.Festivo = esfestivo != null ? "Y" : "N";
                            parametersTse.Estado = horario == null ? "E204 NO TIENE HORARIO ASIGNADO" : "";
                            parametersTse.Semana = Semana;
                            parametersTse.Anio = semanahorario.Year.ToString();

                            if (parametersTse.Festivo == "Y")
                            {
                                listParametersInitialEntity.Add(parametersTse);
                                continue;
                            }

                            var hasHorarioSemana = false;
                            var hasHorarioDia = false;
                            var previosAndPos = new List<double>();
                            if (horario.Count > 0)
                            {
                                hasHorarioSemana = true;
                                hasHorarioDia = indexHorario >= 0;

                                if (indexHorario >= 0)
                                {
                                    previosAndPos = getPreviasAndPosHorario(tse.StartHours, tse.EndHours, horario[indexHorario].HoraInicio, horario[indexHorario].HoraFin);
                                }
                                /*else
                                {
                                    //NO hay coincidencia en la semana del horario
                                    parametersTse.EstatusProceso = "NO_APLICA_X_HORARIO";
                                    listParametersInitialEntity.Add(parametersTse);
                                    break;
                                }*/
                            }
                            else
                            {
                                previosAndPos.Add(0.0);
                                previosAndPos.Add(0.0);
                            }

                            if (horario.Count > 0)
                            {
                                /*if (indexHorario == -1)
                                {
                                    //NO hay coincidencia en la semana del horario
                                    parametersTse.EstatusProceso = "NO_APLICA_X_HORARIO";
                                    listParametersInitialEntity.Add(parametersTse);
                                    break;
                                }*/

                                if (indexHorario >= 0) {
                                    parametersTse.HoraInicioHoraio = horario[indexHorario].HoraInicio == null ? "0" : horario[indexHorario].HoraInicio;
                                    parametersTse.HoraFinHorario = horario[indexHorario].HoraFin == null ? "0" : horario[indexHorario].HoraFin;
                                    parametersTse.Estado = horario[indexHorario].HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";
                                    //Para TSE, no aplica la politica por overtime

                                    parametersTse.HorasInicio = previosAndPos[0];
                                    parametersTse.HorasFin = previosAndPos[1];
                                }
                                
                            }
                             
                            if (!hasHorarioSemana && !hasHorarioDia) {
                                //NO hay horario
                                parametersTse.HoraInicioHoraio = "0";
                                parametersTse.HoraFinHorario = "0";
                                parametersTse.EstatusProceso = "NO_APLICA_X_HORARIO";
                                parametersTse.Problemas = $"El empleado no cuenta con registro de horarios para la fecha de la actividad";
                                parametersTse.Acciones = $"Solicite a un aprobador actualizar sus horarios de trabajo";
                                listParametersInitialEntity.Add(parametersTse);

                                continue;
                            }

                            if (hasHorarioSemana && !hasHorarioDia) {
                                listParametersInitialEntity.Add(parametersTse);
                                continue;
                            }


                            //Evaluating schedullers
                            //-----------------------------------------------------------------------------------------------

                            var scheduleStartDateTime = DateTime.ParseExact($"{startDateTime.ToString("dd/MM/yyyy")} {horario[indexHorario].HoraInicio}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                            var scheduleEndDateTime = DateTime.ParseExact($"{startDateTime.ToString("dd/MM/yyyy")} {horario[indexHorario].HoraFin}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                            var beforeTime = (endDateTime < scheduleStartDateTime ? endDateTime : scheduleStartDateTime) - startDateTime;
                            var afterTime = endDateTime - scheduleEndDateTime;

                            
                            //listParametersInitialEntity.Add(parametersARP);
                            if (beforeTime.TotalMinutes > 0)
                            {
                                overtimeCount++;
                                //OVERTIME BEFORE SCHEDULE
                                parametersTse.HoraInicio = startDateTime.ToString("HH:mm");
                                parametersTse.HoraFin = (endDateTime < scheduleStartDateTime ? horaFin : scheduleStartDateTime.ToString("HH:mm"));
                                string[] r1 = parametersTse.HoraInicio.Split(":");
                                string[] r2 = parametersTse.HoraFin.Split(":");
                                TimeSpan overtimeTime = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                                parametersTse.TOTAL_MINUTOS = overtimeTime.TotalMinutes.ToString();
                                parametersTse.totalHoras = overtimeTime.TotalHours.ToString();
                                parametersTse.Reporte = $"{registrox.WorkOrder}-R{overtimeCount}";
                                listParametersInitialEntity.Add(parametersTse);

                            }

                            if (afterTime.TotalMinutes > 0)
                            {
                                overtimeCount++;
                                DateTime horaInicio = startDateTime > scheduleEndDateTime ? startDateTime : scheduleEndDateTime;

                                //OVERTIME AFTER SCHEDULE
                                var parametersTse2 = new ParametersTseInitialEntity();
                                parametersTse2.HoraInicio = horaInicio.ToString("HH:mm");
                                parametersTse2.HoraFin = horaFin;
                                string[] r1 = parametersTse2.HoraInicio.Split(":");
                                string[] r2 = parametersTse2.HoraFin.Split(":");
                                TimeSpan overtimeTime = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                                parametersTse2.TOTAL_MINUTOS = overtimeTime.TotalMinutes.ToString();
                                parametersTse2.totalHoras = overtimeTime.TotalHours.ToString();
                                parametersTse2.Reporte = $"{registrox.WorkOrder}-R{overtimeCount}";
                                parametersTse2.Actividad = parametersTse.Actividad;

                                parametersTse2.Anio = parametersTse.Anio;
                                parametersTse2.FECHA_REP = parametersTse.FECHA_REP;
                                parametersTse2.EstatusProceso = parametersTse.EstatusProceso;
                                parametersTse2.Estado = parametersTse.Estado;
                                parametersTse2.EmployeeCode = parametersTse.EmployeeCode;
                                parametersTse2.EstatusOrigen = parametersTse.EstatusOrigen;
                                parametersTse2.Festivo = parametersTse.Festivo;
                                parametersTse2.HoraFinHorario = parametersTse.HoraFinHorario;
                                parametersTse2.HoraInicioHoraio = parametersTse.HoraInicioHoraio;
                                parametersTse2.HorasFin = parametersTse.HorasFin;
                                parametersTse2.HorasInicio = parametersTse.HorasInicio;
                                parametersTse2.IdCarga = parametersTse.IdCarga;
                                parametersTse2.IdParamTSEInitialId = Guid.NewGuid();
                                parametersTse2.OutIme = parametersTse.OutIme;
                                parametersTse2.OverTime = parametersTse.OverTime;
                                parametersTse2.Semana = parametersTse.Semana;

                                listParametersInitialEntity.Add(parametersTse2);
                            }

                            if (overtimeCount <= 0)
                            {
                                //NO hay horario
                                parametersTse.EstatusProceso = "NO_APLICA_X_HORARIO_EMPLEADO";
                                parametersTse.Problemas = $"El registro se encuentra dentro del horario de trabajo del empleado";
                                parametersTse.Acciones = $"Verifique y actualice el reporte en CSP(TSE)";
                                listParametersInitialEntity.Add(parametersTse);
                                continue;
                            }
                            //--------------------------------------------------------------------------------------------------
                            //listParametersInitialEntity.Add(parametersTse);

                        }

                    }

                    


                }
                catch (Exception ex)
                {
                    await updateCargaStatus(model.IdCarga, "Error carga en TSE..." + ex.Message);
                    string error = ex.Message;
                }

                await updateCargaStatus(model.IdCarga, "Procesando " + listParametersInitialEntity.Count() + " de " + listParametersInitialEntity.Count + " registros TSE...");
                _dataBaseService.Context().BulkInsert(listParametersInitialEntity);
                await updateCargaStatus(model.IdCarga, "TSE terminado...");

                return model.IdCarga.ToString();

            }
            catch (Exception ex)
            {
                await updateCargaStatus(model.IdCarga, "Error carga en TSE..." + ex.Message);
                return "00000000-0000-0000-0000-000000000000";

            }
        }


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
        

        private ARPLoadDetailEntity fHomologaARP(ARPLoadDetailEntity item)
        {
            try
            {
                var convert = item;


                DateTimeOffset dateTimeOffset = new DateTimeOffset();

                /*if (convert.FECHA_REP == null)
                {
                    return null;

                }*/
                

                try
                {
                    var dt = new DateTimeOffset();
                   // var paisComparacion = horariosGMT.FirstOrDefault(e => e.NameCountryCompare == paisRegistro.NameCountry);

                    try
                    {
                        if (convert.FECHA_REP.Length == 5)
                        {
                            var julianDate = DateTime.FromOADate((Double.Parse(convert.FECHA_REP) - 2415018.5) + 2400000.5);
                            convert.FECHA_REP = julianDate.Date.ToString("dd/MM/yyyy 00:00:00");
                            convert.HORA_INICIO = TimeSpan.Parse(convert.HORA_INICIO).ToString(@"hh\:mm");
                            convert.HORA_FIN = TimeSpan.Parse(convert.HORA_FIN).ToString(@"hh\:mm");
                            //11/20/1981 12:00:00 AM
                        }
                        //
                    }
                    catch (Exception ex)
                    {

                    }

                    try
                    {
                         
                        if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            //convert.FECHA_REP = dt.ToString("dd/MM/yyyy HH:mm:ss");
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                            convert.HORA_INICIO = TimeSpan.Parse(convert.HORA_INICIO).ToString(@"hh\:mm");
                            convert.HORA_FIN = TimeSpan.Parse(convert.HORA_FIN).ToString(@"hh\:mm");
                        }
                        
                    }
                    catch (Exception exx)
                    {


                    }

                   




                }
                catch (Exception ex)
                {
                    throw;
                }



                return convert;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //private TSELoadEntity fHomologaTSE(TSELoadEntity item, List<PaisRelacionGMTEntity> horariosGMT, CountryModel paisRegistro)
        private TSELoadEntity fHomologaTSE(TSELoadEntity item, string GMTSelect, CountryModel paisRegistro)
        {
            try
            {
                var convert = item;
                DateTimeOffset dateTimeOffset = new DateTimeOffset();

                
                if (convert.EndTime == null)
                {
                    return convert;

                }
                if (convert.StartTime == null)
                {
                    return convert;
                }
            
                


                try
                {
                    var numeroReporte = item.WorkOrder;
                    //var paisComparacion = horariosGMT.FirstOrDefault(e => e.NameCountryCompare == paisRegistro.NameCountry);
                    string horarioRowGMT = convert.ZonaHoraria.Substring(0, 11);
                    int zonf = int.Parse(horarioRowGMT.Replace("(GMT", "").Replace(":00)", ""));
                    int ZonBase = int.Parse(GMTSelect);
                    var zzz = $"{(Math.Sign(ZonBase) < 0 ? "-" : "+")}{Math.Abs(ZonBase).ToString().PadLeft(2, '0')}:00";
                    var dt = DateTimeOffset.ParseExact($"{convert.StartTime} {zzz}", "d/MM/yyyy hh:mm tt zzz", CultureInfo.InvariantCulture);
                    var offset = new TimeSpan(zonf, 0 , 0);
                    dt = dt.ToOffset(offset);
                    convert.StartHours = dt.ToString("HH:mm");
                    convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");

                    dt = DateTimeOffset.ParseExact($"{convert.EndTime} {zzz}", "d/MM/yyyy hh:mm tt zzz", CultureInfo.InvariantCulture);
                    dt = dt.ToOffset(offset);
                    convert.EndHours = dt.ToString("HH:mm");
                    convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");

                }
                catch (Exception ex)
                {
                    throw;
                }



                return convert;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //private STELoadEntity fHomologaSTE(STELoadEntity item, List<PaisRelacionGMTEntity> horariosGMT, CountryModel paisRegistro)
        private STELoadEntity fHomologaSTE(STELoadEntity item, string GMTSelect, string userDbGMT, CountryModel paisRegistro)
        {
            try
            {
                var convert = item;

                if (convert.EndDateTime == null)
                {
                    return convert;

                }
                if (convert.StartDateTime == null)
                {
                    return convert;
                }

                try
                {
                    //var paisComparacion = horariosGMT.FirstOrDefault(e => e.NameCountryCompare == paisRegistro.NameCountry);
                    string horarioRowGMT = userDbGMT.Substring(0, 11);
                    int zonf = int.Parse(horarioRowGMT.Replace("(GMT", "").Replace(":00)", ""));
                    int ZonBase = int.Parse(GMTSelect);
                    var zzz = $"{(Math.Sign(ZonBase) < 0 ? "-" : "+")}{Math.Abs(ZonBase).ToString().PadLeft(2, '0')}:00";
                    var dt = DateTimeOffset.ParseExact($"{convert.StartDateTime} {zzz}", "d/MM/yyyy hh:mm tt zzz", CultureInfo.InvariantCulture);
                    var offset = new TimeSpan(zonf, 0, 0);
                    dt = dt.ToOffset(offset);
                    convert.StartHours = dt.ToString("HH:mm");
                    convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");

                    dt = DateTimeOffset.ParseExact($"{convert.EndDateTime} {zzz}", "d/MM/yyyy hh:mm tt zzz", CultureInfo.InvariantCulture);
                    dt = dt.ToOffset(offset);
                    convert.EndHours = dt.ToString("HH:mm");
                    convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");

                }
                catch (Exception ex)
                {
                    throw;
                }



                return convert;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SummaryLoad> LoadSTE(LoadJsonPais model)
        {

            await _logCommand.Log(model.idUserEntiyId, "Ejecuta carga OVERTIME STE", model);

            await updateCargaStatus(model.IdCarga, "Procesando STE...");
            var semanahorario = new DateTimeOffset();

            CultureInfo cul = CultureInfo.CurrentCulture;
            List<HorarioReturn> fueraH = new List<HorarioReturn>();
            List<FestivosEntity> listFestivos = new();
            List<ParametersSteInitialEntity> listParametersInitialEntity = new();

            try
            {
                //Deserializa obj con carga STE
                List<STELoadEntity> datosSTEExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STELoadEntity>>(model.Data.ToJsonString());

                //Obtiene datos con totalduration>0
                List<STELoadEntity> datosSTEExcel = datosSTEExcelFull!.Where(x => x.TotalDuration != "0").ToList();
                //
                // datosSTEExcel.Where(e => string.IsNullOrEmpty(e.AccountCMRNumber) == true).ToList().ForEach(x => x.AccountCMRNumber = "1234");


                //Setting number of register on ARPLoadEntity
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                              ForEach(x => x.STECarga = datosSTEExcel.Count().ToString());
                await _dataBaseService.SaveAsync();


                //Remueve duplicados 
                datosSTEExcel = datosSTEExcel.DistinctBy(m => new { m.SessionEmployeeSerialNumber, m.FechaRegistro, m.StartDateTime, m.EndDateTime }).ToList();

                //Setting duplicados metrica en STE
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEOmitidosXDuplicidad = (datosSTEExcelFull.Count() - datosSTEExcel.Count()).ToString());
                await _dataBaseService.SaveAsync();



                //Obtiene listado de paises
                var listaCountries = await _consultCountryCommand.List();

                //Obtiene listado de festivos
                listFestivos = _dataBaseService.FestivosEntity.ToList();

                //Obtiene listado de horarios disponibles
                var Lsthorario = _dataBaseService.workinghoursEntity.Include("UserEntity").ToList();

                var horariosGMT = await _dataBaseService.PaisRelacionGMTEntity.Where(e => e.NameCountrySelected == model.PaisSel).ToListAsync();
                var paisGeneral = await _dataBaseService.CountryEntity.ToListAsync();



                //Remueve duplicados 
                datosSTEExcel = datosSTEExcel.DistinctBy(m => new { m.SessionEmployeeSerialNumber, m.StartDateTime, m.EndDateTime }).ToList();
                DateTime currentDateTime = DateTime.Now;
                await updateCargaStatus(model.IdCarga, "Procesando STE...");
                List<string> employeeCodes = datosSTEExcel.Select(x => x.SessionEmployeeSerialNumber).ToList();
                List<UserZonaHoraria> userZonasHorarias = _dataBaseService.UserZonaHoraria.Where(x => employeeCodes.Contains(x.EmployeeCode)).ToList();
                foreach (var registrox in datosSTEExcel)
                {
                    var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == registrox.SessionEmployeeSerialNumber.Substring(registrox.SessionEmployeeSerialNumber.Length - 3));
                    var UserDB = userZonasHorarias.FirstOrDefault(op => op.EmployeeCode == registrox.SessionEmployeeSerialNumber);
                    var zonaHorariaU = UserDB != null ? UserDB.ZonaHorariaU : "(GMT+00:00) hora del meridiano de Greenwich (GMT)";

                    //var ste = fHomologaSTE(registrox, horariosGMT, paisRegistro!);
                    var ste = fHomologaSTE(registrox, model.PaisSel, zonaHorariaU, paisRegistro!);

                    var parametersSte = new ParametersSteInitialEntity();
                    parametersSte.IdParamSTEInitialId = Guid.NewGuid();
                    parametersSte.EstatusProceso = "";
                    parametersSte.FECHA_REP = registrox.StartDateTime;
                    parametersSte.TOTAL_MINUTOS = getMins(registrox.TotalDuration);
                    parametersSte.totalHoras = registrox.TotalDuration;
                    parametersSte.HorasInicio = 0;
                    parametersSte.HorasFin = 0;
                    parametersSte.EmployeeCode = registrox.SessionEmployeeSerialNumber;
                    parametersSte.IdCarga = new Guid(model.IdCarga);
                    parametersSte.HoraInicio = "0";
                    parametersSte.HoraFin = "0"; 
                    parametersSte.OutIme = "N";
                    parametersSte.Semana = 0;
                    parametersSte.HoraInicioHoraio = "0";
                    parametersSte.HoraFinHorario = "0";
                    parametersSte.OverTime = "N";
                    parametersSte.Anio = semanahorario.Year.ToString();
                    parametersSte.Festivo = "N";
                    parametersSte.Estado = "";
                    parametersSte.Reporte = $"{registrox.NumeroCaso}";
                    /*parametersSte.EstatusOrigen = "STE"; //STE no maneja estatus*/
                    parametersSte.EstatusOrigen = "EXTRACTED";
                    parametersSte.Actividad = registrox.NumeroCaso;

                    if (UserDB == null)
                    {
                        parametersSte.EstatusProceso = "NO_APLICA_X_USUARIO_SIN_ZONA_HORARIA";
                        listParametersInitialEntity.Add(parametersSte);
                        continue;
                    }

                    //valida pais
                    if (paisRegistro == null)
                    {
                        //PAIS no valido
                        parametersSte.EstatusProceso = "NO_APLICA_X_PAIS";
                        listParametersInitialEntity.Add(parametersSte);
                        continue;
                    }

                    if (ste.EndDateTime == null || ste.StartDateTime == null)
                    {
                        parametersSte.EstatusProceso = "NO_APLICA_X_FALTA_DATOS";
                        listParametersInitialEntity.Add(parametersSte);
                        continue;

                    }

                    var excelStartDateTime = DateTime.ParseExact($"{ste.StartDateTime.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    var excelEndDateTime = DateTime.ParseExact($"{ste.EndDateTime.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    var totalDays = Math.Ceiling((excelEndDateTime - excelStartDateTime).TotalDays) + 1;

                    excelStartDateTime = DateTime.ParseExact($"{ste.StartDateTime.Substring(0, 10)} {ste.StartHours}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    excelEndDateTime = DateTime.ParseExact($"{ste.EndDateTime.Substring(0, 10)} {ste.EndHours}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    
                    int overtimeCount = 0;
                    for (int i = 0; i < totalDays; i++) {
                        DateTime startDateTime = (new DateTime(excelStartDateTime.Year, excelStartDateTime.Month, excelStartDateTime.Day, 0, 0, 0)).AddDays(i);
                        DateTime endDateTime;
                        var horaFin = "";
                        if (totalDays > 1 && i < (totalDays - 1)) {
                            endDateTime = startDateTime.AddHours(24);
                            horaFin = "23:59";
                        } else {
                            endDateTime = excelEndDateTime;
                            horaFin = endDateTime.ToString("HH:mm");
                        }
                        startDateTime = i == 0 ? startDateTime.AddHours(excelStartDateTime.Hour).AddMinutes(excelStartDateTime.Minute).AddSeconds(excelStartDateTime.Second) : startDateTime;

                        parametersSte = new ParametersSteInitialEntity();
                        parametersSte.IdParamSTEInitialId = Guid.NewGuid();
                        parametersSte.EstatusProceso = "";
                        parametersSte.FECHA_REP = startDateTime.ToString("dd/MM/yyyy 00:00:00");
                        parametersSte.TOTAL_MINUTOS = getMins(registrox.TotalDuration);
                        parametersSte.totalHoras = registrox.TotalDuration;
                        parametersSte.HorasInicio = 0;
                        parametersSte.HorasFin = 0;
                        parametersSte.EmployeeCode = registrox.SessionEmployeeSerialNumber;
                        parametersSte.IdCarga = new Guid(model.IdCarga);
                        parametersSte.HoraInicio = startDateTime.ToString("HH:mm");
                        parametersSte.HoraFin = horaFin;
                        parametersSte.OutIme = "N";
                        parametersSte.Semana = 0;
                        parametersSte.HoraInicioHoraio = "0";
                        parametersSte.HoraFinHorario = "0";
                        parametersSte.OverTime = "N";
                        parametersSte.Anio = semanahorario.Year.ToString();
                        parametersSte.Festivo = "N";
                        parametersSte.Estado = "";
                        parametersSte.Reporte = $"{registrox.NumeroCaso}-R{overtimeCount}";
                        /*parametersSte.EstatusOrigen = "STE"; //STE no maneja estatus*/
                        parametersSte.EstatusOrigen = "EXTRACTED";
                        parametersSte.Actividad = registrox.NumeroCaso;


                        //valida pais
                        if (paisRegistro == null)
                        {
                            //PAIS no valido
                            parametersSte.EstatusProceso = "NO_APLICA_X_PAIS";
                            listParametersInitialEntity.Add(parametersSte);
                            continue;
                        }

                        
                        if (ste.EndDateTime == null || ste.StartDateTime == null)
                        {
                            parametersSte.EstatusProceso = "NO_APLICA_X_FALTA_DATOS_INICIO_FIN";
                            listParametersInitialEntity.Add(parametersSte);
                            continue;

                        }

                        semanahorario = DateTimeOffset.ParseExact(startDateTime.ToString("dd/MM/yyyy 00:00:00"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture); 


                        int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                        bool bValidacionHorario = false;
                        var horario = Lsthorario.FindAll(x => x.UserEntity.EmployeeCode == ste.SessionEmployeeSerialNumber && x.week == Semana.ToString());// && x.FechaWorking== semanahorario.DateTime);

                        int indexHorario = -1;

                        for (int j = 0; j < horario.Count; j++)
                        {
                            if (horario[j].FechaWorking.UtcDateTime.ToString("dd/MM/yyyy 00:00:00") == startDateTime.ToString("dd/MM/yyyy 00:00:00"))
                            {
                                indexHorario = j;
                            }

                        }

                        var esfestivo = listFestivos.FirstOrDefault(x => x.DiaFestivo.UtcDateTime.ToString("dd/MM/yyyy") == semanahorario.UtcDateTime.ToString("dd/MM/yyyy") && x.CountryId == paisRegistro!.IdCounty);

                        parametersSte.EstatusProceso = "EN_OVERTIME";
                        parametersSte.Anio = semanahorario.Year.ToString();
                        parametersSte.Festivo = esfestivo != null ? "Y" : "N";
                        parametersSte.Semana = Semana;

                        if (parametersSte.Festivo == "Y") {
                            listParametersInitialEntity.Add(parametersSte);
                            continue;
                        }


                        var hasHorarioSemana = false;
                        var hasHorarioDia = false;
                        var previosAndPos = new List<double>();
                        if (horario.Count > 0)
                        {
                            hasHorarioSemana = true;
                            hasHorarioDia = indexHorario >= 0;

                            if (indexHorario >= 0)
                            {
                                previosAndPos = getPreviasAndPosHorario(ste.StartHours, ste.EndHours, horario[indexHorario].HoraInicio, horario[indexHorario].HoraFin);
                            }
                            /*else
                            {
                                //NO hay coincidencia en la semana del horario
                                parametersSte.EstatusProceso = "NO_APLICA_X_HORARIO";
                                listParametersInitialEntity.Add(parametersSte);
                                continue;
                            }*/

                            bValidacionHorario = true;
                        }
                        else
                        {
                            previosAndPos.Add(0.0);
                            previosAndPos.Add(0.0);
                            bValidacionHorario = false;
                        }

                        if (horario.Count() > 0)
                        {
                            /*if (indexHorario == -1)
                            {
                                //NO hay coincidencia en la semana del horario
                                parametersSte.EstatusProceso = "NO_APLICA_X_HORARIO";
                                listParametersInitialEntity.Add(parametersSte);
                                continue;
                            }*/

                            if (indexHorario >= 0) {
                                parametersSte.HoraInicioHoraio = horario[indexHorario].HoraInicio == null ? "0" : horario[indexHorario].HoraInicio;
                                parametersSte.HoraFinHorario = horario[indexHorario].HoraFin == null ? "0" : horario[indexHorario].HoraFin;
                                /*parametersSte.Estado = horario[indexHorario].HoraInicio == null ? "E204 NO TIENE HORARIO ASIGNADO" : "E205 PROCESO REALIZADO CORRECTAMENTE";*/

                                //Para STE, no aplica la politica por overtime

                                parametersSte.HorasInicio = previosAndPos[0];

                                //agregar validacion para horasFin
                                parametersSte.HorasFin = previosAndPos[1];
                            }

                        }

                        if (!hasHorarioSemana && !hasHorarioDia) {
                            //NO hay horario
                            parametersSte.EstatusProceso = "NO_APLICA_X_HORARIO";
                            parametersSte.Problemas = $"El empleado no cuenta con registro de horarios para la fecha de la actividad";
                            parametersSte.Acciones = $"Solicite a un aprobador actualizar sus horarios de trabajo";
                            listParametersInitialEntity.Add(parametersSte);
                            continue;
                        }

                        if (hasHorarioSemana && !hasHorarioDia) {
                            listParametersInitialEntity.Add(parametersSte);
                            continue;
                        }


                        //Evaluating schedullers
                        //-----------------------------------------------------------------------------------------------

                        var scheduleStartDateTime = DateTime.ParseExact($"{startDateTime.ToString("dd/MM/yyyy")} {horario[indexHorario].HoraInicio}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        var scheduleEndDateTime = DateTime.ParseExact($"{startDateTime.ToString("dd/MM/yyyy")} {horario[indexHorario].HoraFin}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        var beforeTime = (endDateTime < scheduleStartDateTime ? endDateTime : scheduleStartDateTime) - startDateTime;
                        var afterTime = endDateTime - scheduleEndDateTime;

                        var totMinOrig = parametersSte.TOTAL_MINUTOS;
                        var totHorasOrig = parametersSte.totalHoras;

                        if (beforeTime.TotalMinutes > 0)
                        {
                            overtimeCount++;
                            //OVERTIME BEFORE SCHEDULE
                            parametersSte.HoraInicio = startDateTime.ToString("HH:mm");
                            parametersSte.HoraFin = (endDateTime < scheduleStartDateTime ? horaFin : scheduleStartDateTime.ToString("HH:mm"));
                            string[] r1 = parametersSte.HoraInicio.Split(":");
                            string[] r2 = parametersSte.HoraFin.Split(":");
                            TimeSpan overtimeTime = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                            parametersSte.TOTAL_MINUTOS = overtimeTime.TotalMinutes.ToString();
                            parametersSte.totalHoras = overtimeTime.TotalHours.ToString();
                            parametersSte.Reporte = $"{registrox.NumeroCaso}-R{overtimeCount}";
                            /*parametersSte.TOTAL_MINUTOS = repo.rFinOK.ToString() == "23.59" ? bOvertime == false ? "1": (int.Parse(totMinOrig) - 1).ToString(): parametersSte.TOTAL_MINUTOS;
                            parametersSte.totalHoras = repo.rFinOK.ToString() == "23.59" ? bOvertime == false ? (1/60).ToString() : totHorasOrig : totHorasOrig;*/
                            listParametersInitialEntity.Add(parametersSte);

                        }

                        if (afterTime.TotalMinutes > 0)
                        {
                            overtimeCount++;
                            DateTime horaInicio = startDateTime > scheduleEndDateTime ? startDateTime : scheduleEndDateTime;
                            
                            //OVERTIME AFTER SCHEDULE
                            var parametersSte2 = new ParametersSteInitialEntity();
                            parametersSte2.HoraInicio = horaInicio.ToString("HH:mm");
                            parametersSte2.HoraFin = horaFin;
                            string[] r1 = parametersSte2.HoraInicio.Split(":");
                            string[] r2 = parametersSte2.HoraFin.Split(":");
                            TimeSpan overtimeTime = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                            parametersSte2.TOTAL_MINUTOS = overtimeTime.TotalMinutes.ToString();
                            parametersSte2.totalHoras = overtimeTime.TotalHours.ToString();
                            parametersSte2.Reporte = $"{registrox.NumeroCaso}-R{overtimeCount}";
                            parametersSte2.Actividad = parametersSte.Actividad;

                            parametersSte2.Anio = parametersSte.Anio;
                            parametersSte2.FECHA_REP = parametersSte.FECHA_REP;
                            parametersSte2.EstatusProceso = parametersSte.EstatusProceso;
                            parametersSte2.Estado = parametersSte.Estado;
                            parametersSte2.EmployeeCode = parametersSte.EmployeeCode;
                            parametersSte2.EstatusOrigen = parametersSte.EstatusOrigen;
                            parametersSte2.Festivo = parametersSte.Festivo;
                            parametersSte2.HoraFinHorario = parametersSte.HoraFinHorario;
                            parametersSte2.HoraInicioHoraio = parametersSte.HoraInicioHoraio;
                            parametersSte2.HorasFin = parametersSte.HorasFin;
                            parametersSte2.HorasInicio = parametersSte.HorasInicio;
                            parametersSte2.IdCarga = parametersSte.IdCarga;
                            parametersSte2.IdParamSTEInitialId = Guid.NewGuid();
                            parametersSte2.OutIme = parametersSte.OutIme;
                            parametersSte2.OverTime = parametersSte.OverTime;
                            parametersSte2.Semana = parametersSte.Semana;
                            listParametersInitialEntity.Add(parametersSte2);

                        }

                        if (overtimeCount <= 0)
                        {
                            //NO hay horario
                            parametersSte.EstatusProceso = "NO_APLICA_X_HORARIO_EMPLEADO";
                            parametersSte.Problemas = $"El registro se encuentra dentro del horario de trabajo del empleado";
                            parametersSte.Acciones = $"Verifique y actualice el reporte en CSP(STE)";
                            listParametersInitialEntity.Add(parametersSte);
                            continue;
                        }
                        //--------------------------------------------------------------------------------------------------
                        //listParametersInitialEntity.Add(parametersSte);
                    }

                }

                await updateCargaStatus(model.IdCarga, "Procesando " + listParametersInitialEntity.Count() + " de " + listParametersInitialEntity.Count + " registros STE...");
                _dataBaseService.Context().BulkInsert(listParametersInitialEntity);
                await updateCargaStatus(model.IdCarga, "STE terminado...");




                // VALIDACION DE OVERTIMES ENTRE TIPOS (ORIGENES) DE LA CARGA, GENERANDO OVERLAPING
                var RowGralARPTSE = _dataBaseService.ParametersArpInitialEntity.FromSqlRaw($"select a.* from \"ParametersArpInitialEntity\" a, \"ParametersTseInitialEntity\" b where ((a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{model.IdCarga}') and a.\"EmployeeCode\" = b.\"EmployeeCode\" and (b.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdCarga\" = '{model.IdCarga}')) and to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') = to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') and ((((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp and (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp and (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)))").ToList();
                var RowGralARPSTE = _dataBaseService.ParametersArpInitialEntity.FromSqlRaw($"select a.* from \"ParametersArpInitialEntity\" a, \"ParametersSteInitialEntity\" b where ((a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{model.IdCarga}') and a.\"EmployeeCode\" = b.\"EmployeeCode\" and (b.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdCarga\" = '{model.IdCarga}')) and to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') = to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') and ((((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp and (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp and (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)))").ToList();
                var RowGralTSEARP = _dataBaseService.ParametersTseInitialEntity.FromSqlRaw($"select a.* from \"ParametersTseInitialEntity\" a, \"ParametersArpInitialEntity\" b where ((a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{model.IdCarga}') and a.\"EmployeeCode\" = b.\"EmployeeCode\" and (b.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdCarga\" = '{model.IdCarga}')) and to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') = to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') and ((((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp and (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp and (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)))").ToList();
                var RowGralTSESTE = _dataBaseService.ParametersTseInitialEntity.FromSqlRaw($"select a.* from \"ParametersTseInitialEntity\" a, \"ParametersSteInitialEntity\" b where ((a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{model.IdCarga}') and a.\"EmployeeCode\" = b.\"EmployeeCode\" and (b.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdCarga\" = '{model.IdCarga}')) and to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') = to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') and ((((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp and (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp and (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)))").ToList();
                var RowGralSTEARP = _dataBaseService.ParametersSteInitialEntity.FromSqlRaw($"select a.* from \"ParametersSteInitialEntity\" a, \"ParametersArpInitialEntity\" b where ((a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{model.IdCarga}') and a.\"EmployeeCode\" = b.\"EmployeeCode\" and (b.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdCarga\" = '{model.IdCarga}')) and to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') = to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') and ((((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp and (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp and (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)))").ToList();
                var RowGralSTETSE = _dataBaseService.ParametersSteInitialEntity.FromSqlRaw($"select a.* from \"ParametersSteInitialEntity\" a, \"ParametersTseInitialEntity\" b where ((a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{model.IdCarga}') and a.\"EmployeeCode\" = b.\"EmployeeCode\" and (b.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdCarga\" = '{model.IdCarga}')) and to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') = to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') and ((((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp <= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp and (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp)) or (((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraInicio\",':', 1)::int, mins => split_part(b.\"HoraInicio\",':', 2)::int))::timestamp and (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp > (to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraInicio\",':', 1)::int, mins => split_part(a.\"HoraInicio\",':', 2)::int))::timestamp) and ((to_timestamp(substring(a.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(a.\"HoraFin\",':', 1)::int, mins => split_part(a.\"HoraFin\",':', 2)::int))::timestamp >= (to_timestamp(substring(b.\"FECHA_REP\",0,11)||' 00:00', 'DD/MM/YYYY HH24:MI') + make_interval(hours => split_part(b.\"HoraFin\",':', 1)::int, mins => split_part(b.\"HoraFin\",':', 2)::int))::timestamp)))").ToList();

                RowGralARPTSE.ToList().ForEach(x => { x.EstatusProceso = "NO_APLICA_X_OVERLAPING"; x.Problemas = $"El registro esta sobrepuesto con otros registros de CSP(TSE) en el mismo rango de tiempo"; x.Acciones = $"Verifique y actualice el reporte en CSP(TSE) o ARP"; });
                RowGralTSEARP.ToList().ForEach(x => { x.EstatusProceso = "NO_APLICA_X_OVERLAPING"; x.Problemas = $"El registro esta sobrepuesto con otros registros de ARP en el mismo rango de tiempo"; x.Acciones = $"Verifique y actualice el reporte en ARP o CSP(TSE)"; });
                RowGralARPSTE.ToList().ForEach(x => { x.EstatusProceso = "NO_APLICA_X_OVERLAPING"; x.Problemas = $"El registro esta sobrepuesto con otros registros de CSP(STE) en el mismo rango de tiempo"; x.Acciones = $"Verifique y actualice el reporte en CSP(STE) o ARP"; });
                RowGralSTEARP.ToList().ForEach(x => { x.EstatusProceso = "NO_APLICA_X_OVERLAPING"; x.Problemas = $"El registro esta sobrepuesto con otros registros de ARP en el mismo rando de tiempo"; x.Acciones = $"Verifique y actualice el reporte en ARP o CS(STE)"; });
                RowGralTSESTE.ToList().ForEach(x => { x.EstatusProceso = "NO_APLICA_X_OVERLAPING"; x.Problemas = $"El registro esta sobrepuesto con otros registros de CSP(STE) en el mismo rango de tiempo"; x.Acciones = $"Verifique y actualice el reporte en CSP(STE) o CSP(TSE)"; });
                RowGralSTETSE.ToList().ForEach(x => { x.EstatusProceso = "NO_APLICA_X_OVERLAPING"; x.Problemas = $"El registro esta sobrepuesto con otros registros de CSP(TSE) en el mismo rango de tiempo"; x.Acciones = $"Verifique y actualice el reporte en CSP(TSE) o (STE)"; });
                /*RowGralARPTSESTE.ToList().ForEach(x => { x.EstatusProceso = "NO_APLICA_X_OVERLAPING"; x.Problemas = $"El registro esta sobrepuesto con otros registros de ARP en el mismo rando de tiempo"; x.Acciones = $"Verifique y actualice el reporte en ARP o PORTAL TLS"; });*/
                /*RowGralARPTSE.ToList().ForEach(x => x.EstatusProceso = "EN_OVERTIME");*/

                await _dataBaseService.SaveAsync();





                //------------VALIDACION ENTRE OVERTIMES DEL MISMO TIPO---------------------------------------------------------------------------------------------------------------------------

                var rowARPGral2 = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();
                var rowSTEGral2 = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();
                var rowTSEGral2 = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();



                //======================================================================================================================

                //Integrar datos de las 3 cargas
               /* var agrupados = integrados(rowARPGral2, rowSTEGral2, rowTSEGral2);

                foreach (var itemsEmployee in agrupados)
                {
                    //for each employee
                    foreach (var empl in itemsEmployee)
                    {
                        var rowARPGralEmpl = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga) && e.EmployeeCode== empl.EmployeeCode ).ToList();
                        var rowSTEGralEmpl = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga) && e.EmployeeCode == empl.EmployeeCode).ToList();
                        var rowTSEGralEmpl = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga) && e.EmployeeCode == empl.EmployeeCode).ToList();
                        //==========================================================================================================

                        var listParametros = new List<Parametrosaux>();

                        foreach (var item in rowARPGralEmpl)
                        {

                            item.HoraInicio = addZerotime(item.HoraInicio);
                            item.HoraFin = addZerotime(item.HoraFin);

                            var startDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            startDate = startDate.AddHours(Int32.Parse(item.HoraInicio.Split(":")[0]));
                            startDate = startDate.AddMinutes(Int32.Parse(item.HoraInicio.Split(":")[1]));

                            var endDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            endDate = endDate.AddHours(Int32.Parse(item.HoraFin.Split(":")[0]));
                            endDate = endDate.AddMinutes(Int32.Parse(item.HoraFin.Split(":")[1]));

                            if (listParametros.Count == 0)
                            {
                                var pAux = new Parametrosaux();
                                pAux.FECHA_REP = item.FECHA_REP;
                                pAux.HoraInicio = item.HoraInicio;
                                pAux.HoraFin = item.HoraFin;

                                listParametros.Add(pAux);

                            }
                            else
                            {
                                foreach (var reporteEvaluado in listParametros)
                                {

                                    var startDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                                    startDatex = startDatex.AddHours(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[0]));
                                    startDatex = startDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[1]));

                                    var endDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                                    endDatex = endDatex.AddHours(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[0]));
                                    endDatex = endDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[1]));

                                    if (OverlappingDates(startDate, endDate, startDatex, endDatex))
                                    {
                                        item.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                                        break;
                                    }
                                    else
                                    {
                                        var pAux = new Parametrosaux();
                                        pAux.FECHA_REP = item.FECHA_REP;
                                        pAux.HoraInicio = item.HoraInicio;
                                        pAux.HoraFin = item.HoraFin;

                                        listParametros.Add(pAux);
                                        break;
                                    }
                                }
                            }


                        }

                        await _dataBaseService.SaveAsync();


                        foreach (var item in rowSTEGralEmpl)
                        {

                            item.HoraInicio = addZerotime(item.HoraInicio);
                            item.HoraFin = addZerotime(item.HoraFin);

                            var startDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            startDate = startDate.AddHours(Int32.Parse(item.HoraInicio.Split(":")[0]));
                            startDate = startDate.AddMinutes(Int32.Parse(item.HoraInicio.Split(":")[1]));

                            var endDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            endDate = endDate.AddHours(Int32.Parse(item.HoraFin.Split(":")[0]));
                            endDate = endDate.AddMinutes(Int32.Parse(item.HoraFin.Split(":")[1]));

                            if (listParametros.Count == 0)
                            {
                                var pAux = new Parametrosaux();
                                pAux.FECHA_REP = item.FECHA_REP;
                                pAux.HoraInicio = item.HoraInicio;
                                pAux.HoraFin = item.HoraFin;

                                listParametros.Add(pAux);

                            }
                            else
                            {
                                foreach (var reporteEvaluado in listParametros)
                                {

                                    var startDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                                    startDatex = startDatex.AddHours(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[0]));
                                    startDatex = startDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[1]));

                                    var endDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                                    endDatex = endDatex.AddHours(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[0]));
                                    endDatex = endDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[1]));

                                    if (OverlappingDates(startDate, endDate, startDatex, endDatex))
                                    {
                                        item.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                                        break;
                                    }
                                    else
                                    {
                                        var pAux = new Parametrosaux();
                                        pAux.FECHA_REP = item.FECHA_REP;
                                        pAux.HoraInicio = item.HoraInicio;
                                        pAux.HoraFin = item.HoraFin;

                                        listParametros.Add(pAux);
                                        break;
                                    }
                                }
                            }


                        }

                        await _dataBaseService.SaveAsync();

                        //testing
                        foreach (var item in rowTSEGralEmpl)
                        {

                            item.HoraInicio = addZerotime(item.HoraInicio);
                            item.HoraFin = addZerotime(item.HoraFin);

                            var startDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            startDate = startDate.AddHours(Int32.Parse(item.HoraInicio.Split(":")[0]));
                            startDate = startDate.AddMinutes(Int32.Parse(item.HoraInicio.Split(":")[1]));

                            var endDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            endDate = endDate.AddHours(Int32.Parse(item.HoraFin.Split(":")[0]));
                            endDate = endDate.AddMinutes(Int32.Parse(item.HoraFin.Split(":")[1]));

                            if (listParametros.Count == 0)
                            {
                                var pAux = new Parametrosaux();
                                pAux.FECHA_REP = item.FECHA_REP;
                                pAux.HoraInicio = item.HoraInicio;
                                pAux.HoraFin = item.HoraFin;

                                listParametros.Add(pAux);

                            }
                            else
                            {
                                foreach (var reporteEvaluado in listParametros)
                                {

                                    var startDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                                    startDatex = startDatex.AddHours(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[0]));
                                    startDatex = startDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[1]));

                                    var endDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                                    endDatex = endDatex.AddHours(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[0]));
                                    endDatex = endDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[1]));

                                    if (OverlappingDates(startDate, endDate, startDatex, endDatex))
                                    {
                                        item.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                                        break;
                                    }
                                    else
                                    {
                                        var pAux = new Parametrosaux();
                                        pAux.FECHA_REP = item.FECHA_REP;
                                        pAux.HoraInicio = item.HoraInicio;
                                        pAux.HoraFin = item.HoraFin;

                                        listParametros.Add(pAux);
                                        break;
                                    }
                                }
                            }


                        }

                        await _dataBaseService.SaveAsync();
                        //==========================================================================================================

                    }
                }*/


















                //=======================================================================================================

                //crea lista homolgada
                
                var listParametros = new List<Parametrosaux>();

                foreach (var item in rowARPGral2)
                {

                    item.HoraInicio = addZerotime(item.HoraInicio);
                    item.HoraFin = addZerotime(item.HoraFin);

                    var startDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                    startDate = startDate.AddHours(Int32.Parse(item.HoraInicio.Split(":")[0]));
                    startDate = startDate.AddMinutes(Int32.Parse(item.HoraInicio.Split(":")[1]));

                    var endDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                    endDate = endDate.AddHours(Int32.Parse(item.HoraFin.Split(":")[0]));
                    endDate = endDate.AddMinutes(Int32.Parse(item.HoraFin.Split(":")[1]));

                    if (listParametros.Count == 0)
                    {
                        var pAux = new Parametrosaux();
                        pAux.FECHA_REP = item.FECHA_REP;
                        pAux.HoraInicio = item.HoraInicio;
                        pAux.HoraFin = item.HoraFin;
                        pAux.EmployeeCode = item.EmployeeCode;

                        listParametros.Add(pAux);

                    }
                    else
                    {
                        foreach (var reporteEvaluado in listParametros)
                        {

                            var startDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            startDatex = startDatex.AddHours(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[0]));
                            startDatex = startDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[1]));

                            var endDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            endDatex = endDatex.AddHours(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[0]));
                            endDatex = endDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[1]));

                            if (item.EmployeeCode == reporteEvaluado.EmployeeCode && OverlappingDates(startDate, endDate, startDatex, endDatex))
                            {
                                item.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                                item.Problemas = $"El registro esta sobrepuesto con otros registros de ARP en el mismo rango de tiempo";
                                item.Acciones = $"Verifique y actualice el reporte en ARP";
                                break;
                            }
                            else
                            {
                                var pAux = new Parametrosaux();
                                pAux.FECHA_REP = item.FECHA_REP;
                                pAux.HoraInicio = item.HoraInicio;
                                pAux.HoraFin = item.HoraFin;
                                pAux.EmployeeCode = item.EmployeeCode;

                                listParametros.Add(pAux);
                                break;
                            }
                        }
                    }


                }

                await _dataBaseService.SaveAsync();


                foreach (var item in rowSTEGral2)
                {

                    item.HoraInicio = addZerotime(item.HoraInicio);
                    item.HoraFin = addZerotime(item.HoraFin);

                    var startDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                    startDate = startDate.AddHours(Int32.Parse(item.HoraInicio.Split(":")[0]));
                    startDate = startDate.AddMinutes(Int32.Parse(item.HoraInicio.Split(":")[1]));

                    var endDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                    endDate = endDate.AddHours(Int32.Parse(item.HoraFin.Split(":")[0]));
                    endDate = endDate.AddMinutes(Int32.Parse(item.HoraFin.Split(":")[1]));

                    if (listParametros.Count == 0)
                    {
                        var pAux = new Parametrosaux();
                        pAux.FECHA_REP = item.FECHA_REP;
                        pAux.HoraInicio = item.HoraInicio;
                        pAux.HoraFin = item.HoraFin;
                        pAux.EmployeeCode = item.EmployeeCode;

                        listParametros.Add(pAux);

                    }
                    else
                    {
                        foreach (var reporteEvaluado in listParametros)
                        {

                            var startDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            startDatex = startDatex.AddHours(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[0]));
                            startDatex = startDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[1]));

                            var endDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            endDatex = endDatex.AddHours(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[0]));
                            endDatex = endDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[1]));

                            if (item.EmployeeCode == reporteEvaluado.EmployeeCode && OverlappingDates(startDate, endDate, startDatex, endDatex))
                            {
                                item.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                                item.Problemas = $"El registro esta sobrepuesto con otros registros de CSP(STE) en el mismo rando de tiempo";
                                item.Acciones = $"Verifique y actualice el reporte en CSP(STE)";
                                break;
                            }
                            else
                            {
                                var pAux = new Parametrosaux();
                                pAux.FECHA_REP = item.FECHA_REP;
                                pAux.HoraInicio = item.HoraInicio;
                                pAux.HoraFin = item.HoraFin;
                                pAux.EmployeeCode = item.EmployeeCode;

                                listParametros.Add(pAux);
                                break;
                            }
                        }
                    }


                }

                await _dataBaseService.SaveAsync();

                //testing
                foreach (var item in rowTSEGral2)
                {

                    item.HoraInicio = addZerotime(item.HoraInicio);
                    item.HoraFin = addZerotime(item.HoraFin);

                    var startDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                    startDate = startDate.AddHours(Int32.Parse(item.HoraInicio.Split(":")[0]));
                    startDate = startDate.AddMinutes(Int32.Parse(item.HoraInicio.Split(":")[1]));

                    var endDate = DateTime.ParseExact(item.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                    endDate = endDate.AddHours(Int32.Parse(item.HoraFin.Split(":")[0]));
                    endDate = endDate.AddMinutes(Int32.Parse(item.HoraFin.Split(":")[1]));

                    if (listParametros.Count == 0)
                    {
                        var pAux = new Parametrosaux();
                        pAux.FECHA_REP = item.FECHA_REP;
                        pAux.HoraInicio = item.HoraInicio;
                        pAux.HoraFin = item.HoraFin;
                        pAux.EmployeeCode = item.EmployeeCode;

                        listParametros.Add(pAux);

                    }
                    else
                    {
                        foreach (var reporteEvaluado in listParametros)
                        {

                            var startDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            startDatex = startDatex.AddHours(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[0]));
                            startDatex = startDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraInicio.Split(":")[1]));

                            var endDatex = DateTime.ParseExact(reporteEvaluado.FECHA_REP, "dd/MM/yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                            endDatex = endDatex.AddHours(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[0]));
                            endDatex = endDatex.AddMinutes(Int32.Parse(reporteEvaluado.HoraFin.Split(":")[1]));

                            if (item.EmployeeCode == reporteEvaluado.EmployeeCode && OverlappingDates(startDate, endDate, startDatex, endDatex))
                            {
                                item.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                                item.Problemas = $"El registro esta sobrepuesto con otros registros de CSP(TSE) en el mismo rando de tiempo";
                                item.Acciones = $"Verifique y actualice el reporte en CSP(TSE)";
                                break;
                            }
                            else
                            {
                                var pAux = new Parametrosaux();
                                pAux.FECHA_REP = item.FECHA_REP;
                                pAux.HoraInicio = item.HoraInicio;
                                pAux.HoraFin = item.HoraFin;
                                pAux.EmployeeCode = item.EmployeeCode;

                                listParametros.Add(pAux);
                                break;
                            }
                        }
                    }


                }

                await _dataBaseService.SaveAsync();
                


                //----------------------------------------------------------------------------------------------------------------------------------------






                //getting metrics
                int arpNoAplicaXHorario = _dataBaseService.ParametersArpInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_HORARIO" || e.EstatusProceso == "NO_APLICA_X_HORARIO_EMPLEADO") && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXOverttime = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXOverLaping = _dataBaseService.ParametersArpInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_OVERLAPING" || e.EstatusProceso == "NO_APLICA_X_OVERLAPING_INTERNO") && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpEnProceso = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXPais = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_PAIS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXFaltaDatoshoras = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_FALTA_DATOS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();


                var cargaRegistro = _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).FirstOrDefault();


                int tseNoAplicaXHorario = _dataBaseService.ParametersTseInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_HORARIO" || e.EstatusProceso == "NO_APLICA_X_HORARIO_EMPLEADO") && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXOverttime = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXOverLaping = _dataBaseService.ParametersTseInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_OVERLAPING" || e.EstatusProceso == "NO_APLICA_X_OVERLAPING_INTERNO") && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseEnProceso = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXPais = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_PAIS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXFaltaDatoshoras = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_FALTA_DATOS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();

                int steNoAplicaXHorario = _dataBaseService.ParametersSteInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_HORARIO" || e.EstatusProceso == "NO_APLICA_X_HORARIO_EMPLEADO") && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXOverttime = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXOverLaping = _dataBaseService.ParametersSteInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_OVERLAPING" || e.EstatusProceso == "NO_APLICA_X_OVERLAPING_INTERNO") && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steEnProceso = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXPais = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_PAIS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXFaltaDatoshoras = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_FALTA_DATOS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                string steOmitidos = "0";


                //updatting metrics
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXHorario = arpNoAplicaXHorario.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXOvertime = arpNoAplicaXOverttime.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXOverlaping = arpNoAplicaXOverLaping.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXProceso = arpEnProceso.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPXDatosNovalidos = (arpNoAplicaXPais + arpNoAplicaXFaltaDatoshoras).ToString());

                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXHorario = tseNoAplicaXHorario.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXOvertime = tseNoAplicaXOverttime.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXOverlaping = tseNoAplicaXOverLaping.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXProceso = tseEnProceso.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXDatosNovalidos = (tseNoAplicaXPais + tseNoAplicaXFaltaDatoshoras).ToString());

                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXHorario = steNoAplicaXHorario.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEEXOvertime = steNoAplicaXOverttime.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXOverlaping = steNoAplicaXOverLaping.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXProceso = steEnProceso.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXDatosNovalidos = (steNoAplicaXPais + steNoAplicaXFaltaDatoshoras).ToString());

                await _dataBaseService.SaveAsync();



                //finishing load process
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                              ForEach(x => x.Estado = 2);
                await _dataBaseService.SaveAsync();


                await updateCargaStatus(model.IdCarga, "Procesando completo OK...");

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

                summary.NO_APLICA_X_HORARIO_TSE = tseNoAplicaXHorario.ToString();
                summary.NO_APLICA_X_OVERTIME_TSE = tseNoAplicaXOverttime.ToString();
                summary.NO_APLICA_X_OVERLAPING_TSE = tseNoAplicaXOverLaping.ToString();
                summary.EN_PROCESO_TSE = tseEnProceso.ToString();

                summary.NO_APLICA_X_HORARIO_STE = steNoAplicaXHorario.ToString();
                summary.NO_APLICA_X_OVERTIME_STE = steNoAplicaXOverttime.ToString();
                summary.NO_APLICA_X_OVERLAPING_STE = steNoAplicaXOverLaping.ToString();
                summary.EN_PROCESO_STE = steEnProceso.ToString();
                summary.IdCarga = model.IdCarga;

                summary.ARPOmitidosXDuplicidad = cargaRegistro.ARPOmitidosXDuplicidad;
                summary.TSEOmitidosXDuplicidad = cargaRegistro.TSEOmitidosXDuplicidad;
                summary.STEOmitidosXDuplicidad = cargaRegistro.STEOmitidosXDuplicidad;

                summary.ARPXDatosNovalidos = cargaRegistro.ARPXDatosNovalidos;
                summary.TSEXDatosNovalidos = cargaRegistro.TSEXDatosNovalidos;
                summary.STEXDatosNovalidos = cargaRegistro.STEXDatosNovalidos;

                return summary;


            }
            catch (Exception ex)
            {
                await updateCargaStatus(model.IdCarga, "Error carg en STE..." + ex.Message);
                //finishing load process
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                              ForEach(x => x.Estado = 3);
                await _dataBaseService.SaveAsync();


                SummaryLoad summary = new SummaryLoad();
                summary.Mensaje = "Carga en error, verifique logs";
                summary.NO_APLICA_X_HORARIO_ARP = "0";
                summary.NO_APLICA_X_OVERTIME_ARP = "0";
                summary.NO_APLICA_X_OVERLAPING_ARP = "0";
                summary.EN_PROCESO_ARP = "0";

                summary.NO_APLICA_X_HORARIO_TSE = "0";
                summary.NO_APLICA_X_OVERTIME_TSE = "0";
                summary.NO_APLICA_X_OVERLAPING_TSE = "0";
                summary.EN_PROCESO_TSE = "0";

                summary.NO_APLICA_X_HORARIO_STE = "0";
                summary.NO_APLICA_X_OVERTIME_STE = "0";
                summary.NO_APLICA_X_OVERLAPING_STE = "0";
                summary.EN_PROCESO_STE = "0";

                summary.ARP_CARGA = "0";
                summary.TSE_CARGA = "0";
                summary.STE_CARGA = "0";

                summary.ARPOmitidosXDuplicidad = "0";
                summary.TSEOmitidosXDuplicidad = "0";
                summary.STEOmitidosXDuplicidad = "0";

                summary.IdCarga = model.IdCarga;

                return summary;
            }
        }

        private List<List<ParametersArpInitialEntity>> integrados(List<ParametersArpInitialEntity> rowARPGral2, List<ParametersSteInitialEntity> rowSTEGral2, List<ParametersTseInitialEntity> rowTSEGral2)
        {
            var listIntegrados = new List<ParametersArpInitialEntity>();
            foreach (var item in rowARPGral2)
            {
                listIntegrados.Add(item);
            }

            foreach (var item in rowTSEGral2)
            {
                var itemTse = new ParametersArpInitialEntity();
                itemTse.IdParametersInitialEntity = item.IdParamTSEInitialId;
                itemTse.OverTime = item.OverTime;
                itemTse.OutIme = item.OutIme;
                itemTse.Anio = item.Anio;
                itemTse.HoraInicio = item.HoraInicio;
                itemTse.HoraInicioHoraio = item.HoraInicioHoraio;
                itemTse.HoraFinHorario = item.HoraFinHorario;
                itemTse.totalHoras = item.totalHoras;
                itemTse.HorasFin = item.HorasFin;
                itemTse.HoraFin = item.HoraFin;
                itemTse.EmployeeCode = item.EmployeeCode;
                itemTse.Estado = item.Estado;
                itemTse.EstatusOrigen = item.EstatusOrigen;
                itemTse.EstatusProceso = item.EstatusProceso;
                itemTse.Reporte = item.Reporte;
                itemTse.FECHA_REP = item.FECHA_REP;
                itemTse.Festivo = item.Festivo;
                itemTse.IdCarga = item.IdCarga;
                itemTse.Semana = item.Semana;
                itemTse.TOTAL_MINUTOS = item.TOTAL_MINUTOS;


                listIntegrados.Add(itemTse);
            }

            foreach (var item in rowSTEGral2)
            {
                var itemSte = new ParametersArpInitialEntity();
                itemSte.IdParametersInitialEntity = item.IdParamSTEInitialId;
                itemSte.OverTime = item.OverTime;
                itemSte.OutIme = item.OutIme;
                itemSte.Anio = item.Anio;
                itemSte.HoraInicio = item.HoraInicio;
                itemSte.HoraInicioHoraio = item.HoraInicioHoraio;
                itemSte.HoraFinHorario = item.HoraFinHorario;
                itemSte.totalHoras = item.totalHoras;
                itemSte.HorasFin = item.HorasFin;
                itemSte.HoraFin = item.HoraFin;
                itemSte.EmployeeCode = item.EmployeeCode;
                itemSte.Estado = item.Estado;
                itemSte.EstatusOrigen = item.EstatusOrigen;
                itemSte.EstatusProceso = item.EstatusProceso;
                itemSte.Reporte = item.Reporte;
                itemSte.FECHA_REP = item.FECHA_REP;
                itemSte.Festivo = item.Festivo;
                itemSte.IdCarga = item.IdCarga;
                itemSte.Semana = item.Semana;
                itemSte.TOTAL_MINUTOS = item.TOTAL_MINUTOS;

                listIntegrados.Add(itemSte);
            }



            var lstGroupedByEmployeecode = listIntegrados.GroupBy(x => new { x.EmployeeCode })
                 .Select(grp => grp.ToList())
                 .ToList();

            return lstGroupedByEmployeecode;
        }

        private string addZerotime(string time)
        {
            if (time.Length==4)
            {
                return "0" + time;
            }
            else return time;
        }

        public bool OverlappingDates(DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2)
        {
            return startDate1 < endDate2 && startDate2 < endDate1;
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

        public async Task<bool> NotificacionesProceso1(string idCarga, string idUserEntiyId)
        {
            await _logCommand.Log(idUserEntiyId, "Acepta carga OVERTIME", idCarga);

            var usersTLS = _dataBaseService.UserEntity.ToList();

            var ARPGral = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso != "EN_OVERTIME" && e.IdCarga == new Guid(idCarga)).ToList();
            var STEGral = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso != "EN_OVERTIME" && e.IdCarga == new Guid(idCarga)).ToList();
            var TSEGral = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso != "EN_OVERTIME" && e.IdCarga == new Guid(idCarga)).ToList();




            foreach (var evento in ARPGral)
            {
                var userData = usersTLS.FirstOrDefault(e => e.EmployeeCode == evento.EmployeeCode);
                if (userData == null)
                {
                    //no se pudo enviar el correo, por q no hay datos registrados en el sistema

                    //notificar al admin
                    //*p0rtaltlsx*
                    _emailCommand.SendEmail(new EmailModel { To = "portaltlsx@gmail.com", Plantilla = "11" });
                    Thread.Sleep(300);
                    continue;
                }

                try
                {
                    //TODO Definir plantilla de correo
                    switch (evento.EstatusProceso)
                    {
                        case "NO_APLICA_X_HORARIO": _emailCommand.SendEmail(new EmailModel { To = userData!.Email, Plantilla = "8" }); break;
                        case "NO_APLICA_X_OVERTIME": _emailCommand.SendEmail(new EmailModel { To = userData!.Email, Plantilla = "9" }); break;
                        case "NO_APLICA_X_OVERLAPING": _emailCommand.SendEmail(new EmailModel { To = userData!.Email, Plantilla = "10" }); break;
                        case "NO_APLICA_X_FALTA_DATOS_INICIO_FIN": _emailCommand.SendEmail(new EmailModel { To = userData!.Email, Plantilla = "12" }); break;
                        default: break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            foreach (var eventoSTE in STEGral)
            {
                var userDataSte = usersTLS.FirstOrDefault(e => e.EmployeeCode == eventoSTE.EmployeeCode);
                if (userDataSte == null)
                {
                    //no se pudo enviar el correo, por q no hay datos registrados en el sistema

                    //notificar al admin
                    _emailCommand.SendEmail(new EmailModel { To = "portaltlsx@gmail.com", Plantilla = "11" });
                    Thread.Sleep(300);
                    continue;
                }

                try
                {
                    //TODO Definir plantilla de correo
                    switch (eventoSTE.EstatusProceso)
                    {
                        case "NO_APLICA_X_HORARIO": _emailCommand.SendEmail(new EmailModel { To = userDataSte!.Email, Plantilla = "8" }); break;
                        case "NO_APLICA_X_OVERTIME": _emailCommand.SendEmail(new EmailModel { To = userDataSte!.Email, Plantilla = "9" }); break;
                        case "NO_APLICA_X_OVERLAPING": _emailCommand.SendEmail(new EmailModel { To = userDataSte!.Email, Plantilla = "10" }); break;
                        case "NO_APLICA_X_FALTA_DATOS_INICIO_FIN": _emailCommand.SendEmail(new EmailModel { To = userDataSte!.Email, Plantilla = "12" }); break;
                        default: break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            foreach (var eventoTSE in TSEGral)
            {
                var userDataTse = usersTLS.FirstOrDefault(e => e.EmployeeCode == eventoTSE.EmployeeCode);
                if (userDataTse == null)
                {
                    //no se pudo enviar el correo, por q no hay datos registrados en el sistema

                    //notificar al admin
                    _emailCommand.SendEmail(new EmailModel { To = "portaltlsx@gmail.com", Plantilla = "11" });
                    Thread.Sleep(300);
                    continue;
                }

                try
                {
                    //TODO Definir plantilla de correo
                    switch (eventoTSE.EstatusProceso)
                    {
                        case "NO_APLICA_X_HORARIO": _emailCommand.SendEmail(new EmailModel { To = userDataTse!.Email, Plantilla = "8" }); break;
                        case "NO_APLICA_X_OVERTIME": _emailCommand.SendEmail(new EmailModel { To = userDataTse!.Email, Plantilla = "9" }); break;
                        case "NO_APLICA_X_OVERLAPING": _emailCommand.SendEmail(new EmailModel { To = userDataTse!.Email, Plantilla = "10" }); break;
                        case "NO_APLICA_X_FALTA_DATOS_INICIO_FIN": _emailCommand.SendEmail(new EmailModel { To = userDataTse!.Email, Plantilla = "12" }); break;
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

        public async Task<SummaryPortalDB> ValidaLimitesExcepcionesOverlapping(string idCarga)
        {
            SummaryPortalDB summary = new SummaryPortalDB();
            try
            {
                // Si el usuario no existe NO_APLICA_X_USUARIO_INEXISTENTE
                _dataBaseService.ParametersArpInitialEntity.FromSqlRaw($"select a.* from \"ParametersArpInitialEntity\" a left join \"UserEntity\" b on b.\"EmployeeCode\" = a.\"EmployeeCode\" where a.\"IdCarga\" = '{idCarga}' and a.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdUser\" is null").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_USUARIO_INEXISTENTE");
                _dataBaseService.ParametersTseInitialEntity.FromSqlRaw($"select a.* from \"ParametersTseInitialEntity\" a left join \"UserEntity\" b on b.\"EmployeeCode\" = a.\"EmployeeCode\" where a.\"IdCarga\" = '{idCarga}' and a.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdUser\" is null").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_USUARIO_INEXISTENTE");
                _dataBaseService.ParametersSteInitialEntity.FromSqlRaw($"select a.* from \"ParametersSteInitialEntity\" a left join \"UserEntity\" b on b.\"EmployeeCode\" = a.\"EmployeeCode\" where a.\"IdCarga\" = '{idCarga}' and a.\"EstatusProceso\" = 'EN_OVERTIME' and b.\"IdUser\" is null").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_USUARIO_INEXISTENTE");
                await _dataBaseService.SaveAsync();

                var rowARPParameter = _dataBaseService.ParametersArpInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowTSEParameter = _dataBaseService.ParametersTseInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowSTEParameter = _dataBaseService.ParametersSteInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();

                var limitesCountryARP = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == Guid.Parse("908465f1-4848-4c86-9e30-471982c01a2d"));
                var HorasLimiteDia = limitesCountryARP.TargetTimeDay;

                var listaCountries = _dataBaseService.CountryEntity.ToList();
                var listExeptios = _dataBaseService.UsersExceptions.ToList();
                var listHorusReport = _dataBaseService.HorusReportEntity.ToList();
                var UserLst = _dataBaseService.UserEntity.ToList();

                var CoEmple = "";
                var HoursTotEMp = 0.0;

                //Integrar datos de las 3 cargas
                var listIntegrados = new List<ParametersArpInitialEntity>();
                foreach (var item in rowARPParameter)
                {
                    listIntegrados.Add(item);
                }

                foreach (var item in rowTSEParameter)
                {
                    var itemTse = new ParametersArpInitialEntity();
                    itemTse.IdParametersInitialEntity = item.IdParamTSEInitialId;
                    itemTse.OverTime = item.OverTime;
                    itemTse.OutIme = item.OutIme;
                    itemTse.Anio = item.Anio;
                    itemTse.HoraInicio = item.HoraInicio;
                    itemTse.HoraInicioHoraio = item.HoraInicioHoraio;
                    itemTse.HoraFinHorario = item.HoraFinHorario;
                    itemTse.totalHoras = item.totalHoras;
                    itemTse.HorasFin = item.HorasFin;
                    itemTse.HoraFin = item.HoraFin;
                    itemTse.EmployeeCode = item.EmployeeCode;
                    itemTse.Estado = item.Estado;
                    itemTse.EstatusOrigen = item.EstatusOrigen;
                    itemTse.EstatusProceso = item.EstatusProceso;
                    itemTse.Reporte = item.Reporte;
                    itemTse.FECHA_REP = item.FECHA_REP;
                    itemTse.Festivo = item.Festivo;
                    itemTse.IdCarga = item.IdCarga;
                    itemTse.Semana = item.Semana;
                    itemTse.TOTAL_MINUTOS = item.TOTAL_MINUTOS;


                    listIntegrados.Add(itemTse);
                }

                foreach (var item in rowSTEParameter)
                {
                    var itemSte = new ParametersArpInitialEntity();
                    itemSte.IdParametersInitialEntity = item.IdParamSTEInitialId;
                    itemSte.OverTime = item.OverTime;
                    itemSte.OutIme = item.OutIme;
                    itemSte.Anio = item.Anio;
                    itemSte.HoraInicio = item.HoraInicio;
                    itemSte.HoraInicioHoraio = item.HoraInicioHoraio;
                    itemSte.HoraFinHorario = item.HoraFinHorario;
                    itemSte.totalHoras = item.totalHoras;
                    itemSte.HorasFin = item.HorasFin;
                    itemSte.HoraFin = item.HoraFin;
                    itemSte.EmployeeCode = item.EmployeeCode;
                    itemSte.Estado = item.Estado;
                    itemSte.EstatusOrigen = item.EstatusOrigen;
                    itemSte.EstatusProceso = item.EstatusProceso;
                    itemSte.Reporte = item.Reporte;
                    itemSte.FECHA_REP = item.FECHA_REP;
                    itemSte.Festivo = item.Festivo;
                    itemSte.IdCarga = item.IdCarga;
                    itemSte.Semana = item.Semana;
                    itemSte.TOTAL_MINUTOS = item.TOTAL_MINUTOS;

                    listIntegrados.Add(itemSte);
                }



                var lstGroupedByEmployeecode = listIntegrados.GroupBy(x => new { x.EmployeeCode })
                     .Select(grp => grp.ToList())
                     .ToList();

                double horasAcumuladasEmployee = 0.0;
                foreach (var itemsEmployee in lstGroupedByEmployeecode)
                {
                    horasAcumuladasEmployee = 0.0;
                    foreach (var item in itemsEmployee)
                    {
                        //get worked hours 
                        string[] r1 = item.HoraInicio.Split(":");
                        string[] r2 = item.HoraFin.Split(":");
                        TimeSpan tsReportado = (new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0));
                        //get employee ref from this employee
                        var UserRow = UserLst.FirstOrDefault(op => op.EmployeeCode == item.EmployeeCode);
                        //get user exceptions
                        //var exceptionUser = listExeptios.FirstOrDefault(x => x.UserId == UserRow.IdUser && x.StartDate.UtcDateTime.ToString("MM/dd/yyyy") == DateTimeOffset.ParseExact(item.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"));
                        //var exceptedHoursByEmployee = exceptionUser == null ? 0 : exceptionUser.horas;


                        var exceptionUserOr = listExeptios.Where(x => x.UserId == UserRow.IdUser && x.StartDate.UtcDateTime.ToString("dd/MM/yyyy") == DateTimeOffset.ParseExact(item.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") && x.ReportType.Trim().ToUpper().Equals(("OVERTIME").Trim().ToUpper())).ToList();
                        var exceptionUser = exceptionUserOr.Sum(op => op.horas);
                        var exceptedHoursByEmployee = exceptionUser == 0 ? 0 : exceptionUser;
                        
                        //get employee hours from PortalDB
                        var HorasDetectedInPortalDB = listHorusReport.Where(co => co.StrStartDate == item.FECHA_REP && co.UserEntityId == UserRow.IdUser && (co.EstatusFinal!= "RECHAZADO" && co.EstatusFinal!="DESCARTADO")).ToList();
                        //get acummulated hours by this employee
                        var HorasGroupedByEmployeeInPortalDB = HorasDetectedInPortalDB.Select(x => double.Parse(x.CountHours)).Sum();
                        /*if (HorasLimiteDia != 0 && (tsReportado.TotalHours + HorasGroupedByEmployeeInPortalDB + horasAcumuladasEmployee) > (HorasLimiteDia + exceptedHoursByEmployee))
                        {
                            item.EstatusProceso = "NO_APLICA_X_LIMITE_HORAS";
                            //=====================================================================
                            _dataBaseService.ParametersArpInitialEntity.Where(e => e.IdParametersInitialEntity == item.IdParametersInitialEntity).ToList().
                                         ForEach(x => x.EstatusProceso = "NO_APLICA_X_LIMITE_HORAS");

                            _dataBaseService.ParametersTseInitialEntity.Where(e => e.IdParamTSEInitialId == item.IdParametersInitialEntity).ToList().
                                         ForEach(x => x.EstatusProceso = "NO_APLICA_X_LIMITE_HORAS");

                            _dataBaseService.ParametersSteInitialEntity.Where(e => e.IdParamSTEInitialId == item.IdParametersInitialEntity).ToList().
                                         ForEach(x => x.EstatusProceso = "NO_APLICA_X_LIMITE_HORAS");

                            await _dataBaseService.SaveAsync();
                            //=====================================================================
                        }
                        else
                        {*/
                            horasAcumuladasEmployee += tsReportado.TotalHours;
                        /*}*/





                    }
                }


                //=======================================================================================================================

                // Verificar si el overtime ya existe y fue eliminado por el usuario estandar, si es el caso entonces el overtime no aplica NO_APLICA_X_ELIMINACION_USUARIO
                _dataBaseService.ParametersArpInitialEntity.FromSqlRaw($"select a.* from \"ParametersArpInitialEntity\" a left join \"HorusReportEntity\" b on split_part(b.\"StrReport\", '-', 1) = split_part(a.\"Reporte\", '-', 1) left join \"UserEntity\" u on u.\"IdUser\" = b.\"UserEntityId\" where a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{idCarga}' and u.\"EmployeeCode\" = a.\"EmployeeCode\" and a.\"FECHA_REP\" = b.\"StrStartDate\" and a.\"HoraInicio\" = b.\"StartTime\" and a.\"HoraFin\" = b.\"EndTime\" and b.\"EstatusFinal\" = 'RECHAZADO' and b.\"DetalleEstatusFinal\" = 'RECHAZADO POR EL EMPLEADO'").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_ELIMINACION_USUARIO");
                _dataBaseService.ParametersTseInitialEntity.FromSqlRaw($"select a.* from \"ParametersTseInitialEntity\" a left join \"HorusReportEntity\" b on split_part(b.\"StrReport\", '-', 1) = split_part(a.\"Reporte\", '-', 1) left join \"UserEntity\" u on u.\"IdUser\" = b.\"UserEntityId\" where a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{idCarga}' and u.\"EmployeeCode\" = a.\"EmployeeCode\" and a.\"FECHA_REP\" = b.\"StrStartDate\" and a.\"HoraInicio\" = b.\"StartTime\" and a.\"HoraFin\" = b.\"EndTime\" and b.\"EstatusFinal\" = 'RECHAZADO' and b.\"DetalleEstatusFinal\" = 'RECHAZADO POR EL EMPLEADO'").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_ELIMINACION_USUARIO");
                _dataBaseService.ParametersSteInitialEntity.FromSqlRaw($"select a.* from \"ParametersSteInitialEntity\" a left join \"HorusReportEntity\" b on split_part(b.\"StrReport\", '-', 1) = split_part(a.\"Reporte\", '-', 1) left join \"UserEntity\" u on u.\"IdUser\" = b.\"UserEntityId\" where a.\"EstatusProceso\" = 'EN_OVERTIME' and a.\"IdCarga\" = '{idCarga}' and u.\"EmployeeCode\" = a.\"EmployeeCode\" and a.\"FECHA_REP\" = b.\"StrStartDate\" and a.\"HoraInicio\" = b.\"StartTime\" and a.\"HoraFin\" = b.\"EndTime\" and b.\"EstatusFinal\" = 'RECHAZADO' and b.\"DetalleEstatusFinal\" = 'RECHAZADO POR EL EMPLEADO'").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_ELIMINACION_USUARIO");

                //ARP
                // si el overtime ya existe se coloca como NO_APLICA_X_OVERLAPING_INTERNO
                //_dataBaseService.ParametersArpInitialEntity.FromSqlRaw($"select d.* from \"HorusReportEntity\" c left join \"ParametersArpInitialEntity\" d on split_part(d.\"Reporte\", '-', 1) = split_part(c.\"StrReport\", '-', 1) where d.\"IdCarga\" = '{idCarga}' and d.\"EstatusProceso\" = 'EN_OVERTIME' and d.\"FECHA_REP\" = c.\"StrStartDate\" and d.\"HoraInicio\" = c.\"StartTime\" and d.\"HoraFin\" = c.\"EndTime\" and c.\"EstatusFinal\" != 'RECHAZADO' and c.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO'").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO");
                
                // si el reporte(overtime) del mismo numero de reporte y fechas y horas no existe se descarta.
                _dataBaseService.HorusReportEntity.FromSqlRaw($"select a.* from \"HorusReportEntity\" a left join \"UserEntity\" u on u.\"IdUser\" = a.\"UserEntityId\" left join \"ParametersArpInitialEntity\" b on split_part(b.\"Reporte\", '-', 1) = split_part(a.\"StrReport\", '-', 1) where a.\"IdHorusReport\" not in (select c.\"IdHorusReport\" from \"HorusReportEntity\" c left join \"UserEntity\" t on t.\"IdUser\" = c.\"UserEntityId\" left join \"ParametersArpInitialEntity\" d on split_part(d.\"Reporte\", '-', 1) = split_part(c.\"StrReport\", '-', 1) where d.\"IdCarga\" = '{idCarga}' and d.\"EstatusProceso\" = 'EN_OVERTIME' and t.\"EmployeeCode\" = d.\"EmployeeCode\" and d.\"FECHA_REP\" = c.\"StrStartDate\" and d.\"HoraInicio\" = c.\"StartTime\" and d.\"HoraFin\" = c.\"EndTime\" and ((c.\"EstatusFinal\" != 'RECHAZADO' and c.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO') and c.\"EstatusFinal\" != 'DESCARTADO') group by c.\"IdHorusReport\") and b.\"IdCarga\" = '{idCarga}' and ((a.\"EstatusFinal\" != 'RECHAZADO' and a.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO') and a.\"EstatusFinal\" != 'DESCARTADO') and u.\"EmployeeCode\" = b.\"EmployeeCode\" group by a.\"IdHorusReport\"").ToList().ForEach(x => discardReport(x));
                //FIN ARP

                //TSE
                // si el overtime ya existe se coloca como NO_APLICA_X_OVERLAPING_INTERNO
                //_dataBaseService.ParametersTseInitialEntity.FromSqlRaw($"select d.* from \"HorusReportEntity\" c left join \"ParametersTseInitialEntity\" d on split_part(d.\"Reporte\", '-', 1) = split_part(c.\"StrReport\", '-', 1) where d.\"IdCarga\" = '{idCarga}' and d.\"EstatusProceso\" = 'EN_OVERTIME' and d.\"FECHA_REP\" = c.\"StrStartDate\" and d.\"HoraInicio\" = c.\"StartTime\" and d.\"HoraFin\" = c.\"EndTime\" and c.\"EstatusFinal\" != 'RECHAZADO' and c.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO'").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO");

                // si el reporte(overtime) del mismo numero de reporte y fechas y horas no existe se descarta.
                _dataBaseService.HorusReportEntity.FromSqlRaw($"select a.* from \"HorusReportEntity\" a left join \"UserEntity\" u on u.\"IdUser\" = a.\"UserEntityId\" left join \"ParametersTseInitialEntity\" b on split_part(b.\"Reporte\", '-', 1) = split_part(a.\"StrReport\", '-', 1) where a.\"IdHorusReport\" not in (select c.\"IdHorusReport\" from \"HorusReportEntity\" c left join \"UserEntity\" t on t.\"IdUser\" = c.\"UserEntityId\" left join \"ParametersTseInitialEntity\" d on split_part(d.\"Reporte\", '-', 1) = split_part(c.\"StrReport\", '-', 1) where d.\"IdCarga\" = '{idCarga}' and d.\"EstatusProceso\" = 'EN_OVERTIME' and t.\"EmployeeCode\" = d.\"EmployeeCode\" and d.\"FECHA_REP\" = c.\"StrStartDate\" and d.\"HoraInicio\" = c.\"StartTime\" and d.\"HoraFin\" = c.\"EndTime\" and ((c.\"EstatusFinal\" != 'RECHAZADO' and c.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO') and c.\"EstatusFinal\" != 'DESCARTADO') group by c.\"IdHorusReport\") and b.\"IdCarga\" = '{idCarga}' and ((a.\"EstatusFinal\" != 'RECHAZADO' and a.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO') and a.\"EstatusFinal\" != 'DESCARTADO') and u.\"EmployeeCode\" = b.\"EmployeeCode\" group by a.\"IdHorusReport\"").ToList().ForEach(x => discardReport(x));
                //FIN TSE

                //STE
                // si el overtime ya existe se coloca como NO_APLICA_X_OVERLAPING_INTERNO
                //_dataBaseService.ParametersSteInitialEntity.FromSqlRaw($"select d.* from \"HorusReportEntity\" c left join \"ParametersSteInitialEntity\" d on split_part(d.\"Reporte\", '-', 1) = split_part(c.\"StrReport\", '-', 1) where d.\"IdCarga\" = '{idCarga}' and d.\"EstatusProceso\" = 'EN_OVERTIME' and d.\"FECHA_REP\" = c.\"StrStartDate\" and d.\"HoraInicio\" = c.\"StartTime\" and d.\"HoraFin\" = c.\"EndTime\" and c.\"EstatusFinal\" != 'RECHAZADO' and c.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO'").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO");

                // si el reporte(overtime) del mismo numero de reporte y fechas y horas no existe se descarta.
                _dataBaseService.HorusReportEntity.FromSqlRaw($"select a.* from \"HorusReportEntity\" a left join \"UserEntity\" u on u.\"IdUser\" = a.\"UserEntityId\" left join \"ParametersSteInitialEntity\" b on split_part(b.\"Reporte\", '-', 1) = split_part(a.\"StrReport\", '-', 1) where a.\"IdHorusReport\" not in (select c.\"IdHorusReport\" from \"HorusReportEntity\" c left join \"UserEntity\" t on t.\"IdUser\" = c.\"UserEntityId\" left join \"ParametersSteInitialEntity\" d on split_part(d.\"Reporte\", '-', 1) = split_part(c.\"StrReport\", '-', 1) where d.\"IdCarga\" = '{idCarga}' and d.\"EstatusProceso\" = 'EN_OVERTIME' and t.\"EmployeeCode\" = d.\"EmployeeCode\" and d.\"FECHA_REP\" = c.\"StrStartDate\" and d.\"HoraInicio\" = c.\"StartTime\" and d.\"HoraFin\" = c.\"EndTime\" and ((c.\"EstatusFinal\" != 'RECHAZADO' and c.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO') and c.\"EstatusFinal\" != 'DESCARTADO') group by c.\"IdHorusReport\") and b.\"IdCarga\" = '{idCarga}' and ((a.\"EstatusFinal\" != 'RECHAZADO' and a.\"DetalleEstatusFinal\" != 'RECHAZADO POR EL EMPLEADO') and a.\"EstatusFinal\" != 'DESCARTADO') and u.\"EmployeeCode\" = b.\"EmployeeCode\" group by a.\"IdHorusReport\"").ToList().ForEach(x => discardReport(x));
                //FIN STE

                await _dataBaseService.SaveAsync();



                var rowARPParameterFinal = _dataBaseService.ParametersArpInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowTSEParameterFinal = _dataBaseService.ParametersTseInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowSTEParameterFinal = _dataBaseService.ParametersSteInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();

                var datax = _dataBaseService.HorusReportEntity.ToList();

                //NO_APLICA_X_OVERLAPING
                //=======================================================================================================================
                PreaprobadosARP(UserLst, rowARPParameterFinal);
                PreaprobadosTSE(UserLst, rowTSEParameterFinal);
                PreaprobadosSTE(UserLst, rowSTEParameterFinal);

                await _dataBaseService.SaveAsync();
                //=======================================================================================================================

                //Verifica OVERLAPING INTERNO de todos los overtimes entrantes y les pone como NO_APLICA_X_OVERLAPING_INTERNO
                _dataBaseService.ParametersArpInitialEntity.FromSqlRaw($"select a.* from \"ParametersArpInitialEntity\" a, \"HorusReportEntity\" h left join \"UserEntity\" u on u.\"IdUser\" = h.\"UserEntityId\" where a.\"IdCarga\" = '{idCarga}' and a.\"EstatusProceso\" = 'EN_OVERTIME' and u.\"EmployeeCode\" = a.\"EmployeeCode\" and a.\"FECHA_REP\" = h.\"StrStartDate\" and (a.\"HoraInicio\"::time, a.\"HoraFin\"::time) overlaps (h.\"StartTime\"::time, h.\"EndTime\"::time) and (h.\"EstatusFinal\" != 'RECHAZADO' and h.\"EstatusFinal\" != 'DESCARTADO')").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO");
                _dataBaseService.ParametersTseInitialEntity.FromSqlRaw($"select a.* from \"ParametersTseInitialEntity\" a, \"HorusReportEntity\" h left join \"UserEntity\" u on u.\"IdUser\" = h.\"UserEntityId\" where a.\"IdCarga\" = '{idCarga}' and a.\"EstatusProceso\" = 'EN_OVERTIME' and u.\"EmployeeCode\" = a.\"EmployeeCode\" and a.\"FECHA_REP\" = h.\"StrStartDate\" and (a.\"HoraInicio\"::time, a.\"HoraFin\"::time) overlaps (h.\"StartTime\"::time, h.\"EndTime\"::time) and (h.\"EstatusFinal\" != 'RECHAZADO' and h.\"EstatusFinal\" != 'DESCARTADO')").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO");
                _dataBaseService.ParametersSteInitialEntity.FromSqlRaw($"select a.* from \"ParametersSteInitialEntity\" a, \"HorusReportEntity\" h left join \"UserEntity\" u on u.\"IdUser\" = h.\"UserEntityId\" where a.\"IdCarga\" = '{idCarga}' and a.\"EstatusProceso\" = 'EN_OVERTIME' and u.\"EmployeeCode\" = a.\"EmployeeCode\" and a.\"FECHA_REP\" = h.\"StrStartDate\" and (a.\"HoraInicio\"::time, a.\"HoraFin\"::time) overlaps (h.\"StartTime\"::time, h.\"EndTime\"::time) and (h.\"EstatusFinal\" != 'RECHAZADO' and h.\"EstatusFinal\" != 'DESCARTADO')").ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO");
                await _dataBaseService.SaveAsync();
                //Fin de Verifica OVERLAPING INTERNO


                //Metricas finales
                //=======================================================================================================================
                var rowARPParameterGral = _dataBaseService.ParametersArpInitialEntity.AsNoTracking().Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowTSEParameterGral = _dataBaseService.ParametersTseInitialEntity.AsNoTracking().Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowSTEParameterGral = _dataBaseService.ParametersSteInitialEntity.AsNoTracking().Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                int arpNoAplicaXOverLaping = _dataBaseService.ParametersArpInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_OVERLAPING" || e.EstatusProceso == "NO_APLICA_X_OVERLAPING_INTERNO") && e.IdCarga == new Guid(idCarga)).ToList().Count();
                int tseNoAplicaXOverLaping = _dataBaseService.ParametersTseInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_OVERLAPING" || e.EstatusProceso == "NO_APLICA_X_OVERLAPING_INTERNO") && e.IdCarga == new Guid(idCarga)).ToList().Count();
                int steNoAplicaXOverLaping = _dataBaseService.ParametersSteInitialEntity.Where(e => (e.EstatusProceso == "NO_APLICA_X_OVERLAPING" || e.EstatusProceso == "NO_APLICA_X_OVERLAPING_INTERNO") && e.IdCarga == new Guid(idCarga)).ToList().Count();

                int arpNoAplicaLimites = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_LIMITE_HORAS" && e.IdCarga == new Guid(idCarga)).ToList().Count();
                int tseNoAplicaLimites = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_LIMITE_HORAS" && e.IdCarga == new Guid(idCarga)).ToList().Count();
                int steNoAplicaXLimites = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_LIMITE_HORAS" && e.IdCarga == new Guid(idCarga)).ToList().Count();

                summary.Mensaje = "Carga procesada";
                summary.REGISTROS_PORTALDB = (rowARPParameterGral.Count() + rowSTEParameterGral.Count() + rowTSEParameterGral.Count()).ToString();
                summary.NO_APLICA_X_OVERLAPING_ARP = arpNoAplicaXOverLaping.ToString();
                summary.NO_APLICA_X_OVERLAPING_TSE = tseNoAplicaXOverLaping.ToString();
                summary.NO_APLICA_X_OVERLAPING_STE = steNoAplicaXOverLaping.ToString();

                summary.NO_APLICA_X_LIMITE_HORAS_ARP = arpNoAplicaLimites.ToString();
                summary.NO_APLICA_X_LIMITE_HORAS_TSE = tseNoAplicaLimites.ToString();
                summary.NO_APLICA_X_LIMITE_HORAS_STE = steNoAplicaXLimites.ToString();


                List<HorusReportEntity> rowsHorusNew = new();
                HorusReportEntity rowAdd = new();
                List<Domain.Entities.AssignmentReport.AssignmentReport> rowAssignments = new();
                Domain.Entities.AssignmentReport.AssignmentReport rowAddAssig = new();

                var Maxen = 0;
                try
                {
                    Maxen = _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
                }
                catch (Exception)
                {
                    Maxen = 0;
                }

                //=======================================================================================================
                //Inserting to PortalDB
                //=======================================================================================================
                foreach (var itemARPNew in rowARPParameterGral)
                {
                    Maxen++;
                    DateTime fechaHoraOriginal = DateTime.ParseExact(itemARPNew.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);
                    string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");
                    var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemARPNew.EmployeeCode);


                    //Caso aprobacion directa por sistema
                    /*if (userRow.RoleEntity.NameRole== "Usuario Aprobador N2" || userRow.RoleEntity.NameRole == "Administrador" || userRow.RoleEntity.NameRole == "Super Administrador")
                    {
                        //Generating HORUSREPORT
                        rowAdd = new()
                        {
                            IdHorusReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            StrStartDate = nuevaFechaHoraFormato,
                            StartTime = itemARPNew.HoraInicio,
                            EndTime = itemARPNew.HoraFin,
                            ClientEntityId = Guid.Parse("71f6bb04-e301-4b60-afe8-3bb7c2895a69"),//   processed by overtime clientEntity,
                            strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            CountHours = itemARPNew.totalHoras,
                            StrReport = itemARPNew.Reporte,
                            ARPLoadingId = idCarga,
                            Acitivity = 1,//overtime
                            NumberReport = Maxen,
                            DateApprovalSystem = DateTime.Now,
                            Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2,
                            EstatusOrigen = itemARPNew.EstatusOrigen,
                            EstatusFinal = "APROBADO",
                            DetalleEstatusFinal = "",
                            Origen = "ARP",
                            Semana = getWeek(nuevaFechaHoraFormato)

                        };
                        rowsHorusNew.Add(rowAdd);

                        rowAddAssig = new()
                        {
                            IdAssignmentReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            HorusReportEntityId = rowAdd.IdHorusReport,
                            State = 1,
                            Description = "(Aprobado por sistema)",
                            strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            Resultado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2,
                            Nivel = 2
                        };
                        rowAssignments.Add(rowAddAssig);
                    }
                    else
                    {*/
                    //Generating HORUSREPORT
                    string[] r1 = itemARPNew.HoraInicio.Split(":");
                    string[] r2 = itemARPNew.HoraFin.Split(":");
                    var countHours = ((new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0))).TotalHours;

                    rowAdd = new()
                        {
                            IdHorusReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            StrStartDate = nuevaFechaHoraFormato,
                            StartTime = itemARPNew.HoraInicio,
                            EndTime = itemARPNew.HoraFin,
                            ClientEntityId = Guid.Parse("71f6bb04-e301-4b60-afe8-3bb7c2895a69"),//   processed by overtime clientEntity,
                            strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            CountHours = countHours.ToString(),
                            StrReport = itemARPNew.Reporte,
                            ARPLoadingId = idCarga,
                            Acitivity = 1,//overtime
                            NumberReport = Maxen,
                            DateApprovalSystem = DateTime.Now,
                            Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente,
                            EstatusOrigen = itemARPNew.EstatusOrigen,
                            EstatusFinal = "ENPROGRESO",
                            DetalleEstatusFinal = "",
                            Origen = "ARP",
                            Semana = getWeek(nuevaFechaHoraFormato)

                        };
                        rowsHorusNew.Add(rowAdd);

                        rowAddAssig = new()
                        {
                            IdAssignmentReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            HorusReportEntityId = rowAdd.IdHorusReport,
                            State = 0,
                            Description = "PROCESO_OVERTIME",
                            strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            Resultado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente,
                            Nivel = 0
                        };
                        rowAssignments.Add(rowAddAssig);
                   // }
                   
                }

                //TSE
                //------------------------------------------------------------------------

                foreach (var itemTSENew in rowTSEParameterGral)
                {
                    Maxen++;
                    DateTime fechaHoraOriginal = DateTime.ParseExact(itemTSENew.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("yyyy-MM-dd 00:00:00");
                    string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");
                    var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemTSENew.EmployeeCode);


                    //Caso aprobacion directa por sistema
                    /*if (userRow.RoleEntity.NameRole == "Usuario Aprobador N2" || userRow.RoleEntity.NameRole == "Administrador" || userRow.RoleEntity.NameRole == "Super Administrador")
                    {
                        //Generating HORUSREPORT
                        rowAdd = new()
                        {
                            IdHorusReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            StrStartDate = nuevaFechaHoraFormato,
                            StartTime = itemTSENew.HoraInicio,
                            EndTime = itemTSENew.HoraFin,
                            ClientEntityId = Guid.Parse("71f6bb04-e301-4b60-afe8-3bb7c2895a69"),//   processed by overtime clientEntity,
                            strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            CountHours = itemTSENew.totalHoras,
                            StrReport = itemTSENew.Reporte,
                            ARPLoadingId = idCarga,
                            Acitivity = 1,//overtime
                            NumberReport = Maxen,
                            DateApprovalSystem = DateTime.Now,
                            Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2,
                            EstatusOrigen = itemTSENew.EstatusOrigen,
                            EstatusFinal = "APROBADO",
                            DetalleEstatusFinal = "",
                            Origen = "TSE",
                            Semana = getWeek(nuevaFechaHoraFormato)

                        };
                        rowsHorusNew.Add(rowAdd);


                        rowAddAssig = new()
                        {
                            IdAssignmentReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            HorusReportEntityId = rowAdd.IdHorusReport,
                            State = 1,
                            Description = "(Aprobado por sistema)",
                            strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            Resultado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2,
                            Nivel = 2
                        };
                        rowAssignments.Add(rowAddAssig);
                    }
                    else { */
                    string[] r1 = itemTSENew.HoraInicio.Split(":");
                    string[] r2 = itemTSENew.HoraFin.Split(":");
                    var countHours = ((new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0))).TotalHours;
                    rowAdd = new()
                        {
                            IdHorusReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            StrStartDate = nuevaFechaHoraFormato,
                            StartTime = itemTSENew.HoraInicio,
                            EndTime = itemTSENew.HoraFin,
                            ClientEntityId = Guid.Parse("71f6bb04-e301-4b60-afe8-3bb7c2895a69"),
                            strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            CountHours = countHours.ToString(),
                            StrReport = itemTSENew.Reporte,
                            ARPLoadingId = idCarga,
                            Acitivity = 1,//overtime
                            NumberReport = Maxen,
                            DateApprovalSystem = DateTime.Now,
                            Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente,
                            EstatusOrigen = itemTSENew.EstatusOrigen,
                            EstatusFinal = "ENPROGRESO",
                            DetalleEstatusFinal = "",
                            Origen = "TSE",
                            Semana = getWeek(nuevaFechaHoraFormato)
                        };
                        rowsHorusNew.Add(rowAdd);

                        rowAddAssig = new()
                        {
                            IdAssignmentReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            HorusReportEntityId = rowAdd.IdHorusReport,
                            State = 0,
                            Description = "PROCESO_OVERTIME",
                            strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            Resultado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente,
                            Nivel = 0
                        };
                        rowAssignments.Add(rowAddAssig);
                   // }
                }
                //STE
                //------------------------------------------------------------------------
                foreach (var itemSTENew in rowSTEParameterGral)
                {
                    Maxen++;
                    DateTime fechaHoraOriginal = DateTime.ParseExact(itemSTENew.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    // string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("yyyy-MM-dd 00:00:00");
                    string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");
                    var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemSTENew.EmployeeCode);


                    //Caso aprobacion directa por sistema
                    /*if (userRow.RoleEntity.NameRole == "Usuario Aprobador N2" || userRow.RoleEntity.NameRole == "Administrador" || userRow.RoleEntity.NameRole == "Super Administrador")
                    {
                        //Generating HORUSREPORT
                        rowAdd = new()
                        {
                            IdHorusReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            StrStartDate = nuevaFechaHoraFormato,
                            StartTime = itemSTENew.HoraInicio,
                            EndTime = itemSTENew.HoraFin,
                            ClientEntityId = Guid.Parse("71f6bb04-e301-4b60-afe8-3bb7c2895a69"),//   processed by overtime clientEntity,
                            strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            CountHours = itemSTENew.totalHoras,
                            StrReport = itemSTENew.Reporte,
                            ARPLoadingId = idCarga,
                            Acitivity = 1,//overtime
                            NumberReport = Maxen,
                            DateApprovalSystem = DateTime.Now,
                            Estado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2,
                            EstatusOrigen = itemSTENew.EstatusOrigen,
                            EstatusFinal = "APROBADO",
                            DetalleEstatusFinal = "",
                            Origen = "STE",
                            Semana = getWeek(nuevaFechaHoraFormato)

                        };
                        rowsHorusNew.Add(rowAdd);

                        rowAddAssig = new()
                        {
                            IdAssignmentReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            HorusReportEntityId = rowAdd.IdHorusReport,
                            State = 1,
                            Description = "(Aprobado por sistema)",
                            strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            Resultado = (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2,
                            Nivel = 2
                        };
                        rowAssignments.Add(rowAddAssig);
                    }
                    else
                    {*/
                    string[] r1 = itemSTENew.HoraInicio.Split(":");
                    string[] r2 = itemSTENew.HoraFin.Split(":");
                    var countHours = ((new TimeSpan(int.Parse(r2[0]), int.Parse(r2[1]), 0)) - (new TimeSpan(int.Parse(r1[0]), int.Parse(r1[1]), 0))).TotalHours;
                    rowAdd = new()
                        {
                            IdHorusReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            StrStartDate = nuevaFechaHoraFormato,
                            StartTime = itemSTENew.HoraInicio,
                            EndTime = itemSTENew.HoraFin,
                            ClientEntityId = Guid.Parse("71f6bb04-e301-4b60-afe8-3bb7c2895a69"),
                            strCreationDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            CountHours = countHours.ToString(),
                            StrReport = itemSTENew.Reporte,
                            ARPLoadingId = idCarga,
                            Acitivity = 1,//overtime
                            NumberReport = Maxen,
                            DateApprovalSystem = DateTime.Now,
                            Estado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente,
                            EstatusOrigen = itemSTENew.EstatusOrigen,
                            EstatusFinal = "ENPROGRESO",
                            DetalleEstatusFinal = "",
                            Origen = "STE",
                        Semana = getWeek(nuevaFechaHoraFormato)
                        };
                        rowsHorusNew.Add(rowAdd);
                     
                        rowAddAssig = new()
                        {
                            IdAssignmentReport = Guid.NewGuid(),
                            UserEntityId = userRow.IdUser,
                            HorusReportEntityId = rowAdd.IdHorusReport,
                            State = 0,
                            Description = "PROCESO_OVERTIME",
                            strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            Resultado = (byte)Enums.Enums.AprobacionPortalDB.Pendiente,
                            Nivel = 0
                        };
                        rowAssignments.Add(rowAddAssig);
                    //}
                }


                _dataBaseService.HorusReportEntity.AddRange(rowsHorusNew);
                await _dataBaseService.SaveAsync();


                _dataBaseService.assignmentReports.AddRangeAsync(rowAssignments);
                await _dataBaseService.SaveAsync();


                //finishing load process
                await updateCargaStatus(idCarga, "carga terminada...");

            }
            catch (Exception ex)
            {

                string error = ex.Message;
                //finishing load process
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(idCarga)).ToList().
                                              ForEach(x => x.Estado = 3);
                await _dataBaseService.SaveAsync();
            }

            return summary;
        }

        private void discardReport(HorusReportEntity report) {
            report.Estado = (byte)Enums.Enums.AprobacionPortalDB.Descartado;
            report.EstatusFinal = "DESCARTADO";
            report.DetalleEstatusFinal = "";

            var currentAssignment = _dataBaseService.assignmentReports.Where(x => x.HorusReportEntityId == report.IdHorusReport && x.State == 0).AsEnumerable().OrderByDescending(x => DateTime.ParseExact(x.strFechaAtencion, "dd/MM/yyyy HH:mm", null)).FirstOrDefault();
            if (currentAssignment != null) { currentAssignment.State = 1; }

            //Crea asignacion como historial descartado ya que la tabla de asignaciones la estan utilizando como historial (revisar!!!)
            var assignment = new CreateAssignmentReportModel();
            assignment.IdAssignmentReport = Guid.NewGuid();
            assignment.UserEntityId = Guid.Parse("53765c41-411f-4add-9034-7debaf04f276"); // usuario sistema
            assignment.HorusReportEntityId = report.IdHorusReport;
            assignment.State = 0;
            assignment.Resultado = (byte)Enums.Enums.AprobacionPortalDB.Descartado;
            assignment.Description = "";
            assignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var assigmentEntity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignment);
            _dataBaseService.assignmentReports.AddAsync(assigmentEntity);
        }

        public async Task<List<InconsistenceModel>> GetInconsistences(string? idCarga = null, string? employeeCode = null)
        {
            List<InconsistenceModel> inconsistences = new();

            ARPLoadEntity? arpLoad = idCarga != null? _dataBaseService.ARPLoadEntity.Find(Guid.Parse(idCarga)) : _dataBaseService.ARPLoadEntity.Where(x => x.Estado == 2).OrderByDescending(x => x.FechaCreacion).FirstOrDefault(); // Estado == 2 quiere decir estatus terminada
            
            if (arpLoad != null)
            {
                List<ParametersArpInitialEntity> arpInitialParameters = _dataBaseService.ParametersArpInitialEntity.AsNoTracking().Where(op => op.IdCarga == arpLoad.IdArpLoad && (employeeCode != null ? op.EmployeeCode == employeeCode : true) && (op.EstatusProceso == "NO_APLICA_X_OVERLAPING" || op.EstatusProceso == "NO_APLICA_X_HORARIO")).ToList();
                List<ParametersTseInitialEntity> tseInitialParameters = _dataBaseService.ParametersTseInitialEntity.AsNoTracking().Where(op => op.IdCarga == arpLoad.IdArpLoad && (employeeCode != null ? op.EmployeeCode == employeeCode : true) && (op.EstatusProceso == "NO_APLICA_X_OVERLAPING" || op.EstatusProceso == "NO_APLICA_X_HORARIO")).ToList();
                List<ParametersSteInitialEntity> steInitialParameters = _dataBaseService.ParametersSteInitialEntity.AsNoTracking().Where(op => op.IdCarga == arpLoad.IdArpLoad && (employeeCode != null ? op.EmployeeCode == employeeCode : true) && (op.EstatusProceso == "NO_APLICA_X_OVERLAPING" || op.EstatusProceso == "NO_APLICA_X_HORARIO")).ToList();

                DateTime loadDateTime = arpLoad.FechaCreacion;
                for (var i = 0; i < arpInitialParameters.Count(); i++)
                {
                    var parameter = arpInitialParameters[i];
                    UserEntity? userEntity = _dataBaseService.UserEntity.AsNoTracking().Include("CountryEntity").Where(x => x.EmployeeCode == parameter.EmployeeCode).FirstOrDefault();
                    UserManagerEntity? userManagerEntity = _dataBaseService.UserManagerEntity.AsNoTracking().Where(x => x.EmployeeCode == parameter.EmployeeCode).FirstOrDefault();

                    var startDateTime = DateTime.ParseExact($"{parameter.FECHA_REP.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    string[] r1 = parameter.HoraInicio.Split(":");
                    string[] r2 = parameter.HoraFin.Split(":");
                    startDateTime = startDateTime.AddHours(int.Parse(r1[0])).AddMinutes(int.Parse(r1[1]));
                    var endDateTime = DateTime.ParseExact($"{parameter.FECHA_REP.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    r1 = parameter.HoraInicio.Split(":");
                    r2 = parameter.HoraFin.Split(":");
                    endDateTime = endDateTime.AddHours(int.Parse(r2[0])).AddMinutes(int.Parse(r2[1]));

                    var codigoPais = userEntity != null ? userEntity.CountryEntity.CodigoPais : "";
                    var empCode = userEntity != null ? userEntity.EmployeeCode : "";
                    var employeeName = userEntity != null ? $"{userEntity.NameUser} {userEntity.surnameUser}" : "";
                    var employeeEmail = userEntity != null ? userEntity.Email : "";
                    var managerName = userManagerEntity != null ? userManagerEntity.ManagerName : "";
                    var managerEmail = userManagerEntity != null ? userManagerEntity.ManagerEmail : "";
                    var problemas = parameter.Problemas != null ? parameter.Problemas : "";
                    var acciones = parameter.Acciones != null ? parameter.Acciones : "";
                    string issue = "";
                    switch (parameter.EstatusProceso)
                    {
                        case "NO_APLICA_X_HORARIO": { issue = "Error de horario"; break; }
                        case "NO_APLICA_X_OVERLAPING": { issue = "Overlaping"; break; }
                    }


                    InconsistenceModel inconsistence = new();
                    inconsistence.codigoPais = codigoPais;
                    inconsistence.employeeCode = empCode;
                    inconsistence.employeeName = employeeName;
                    inconsistence.employeeEmail = employeeEmail;
                    inconsistence.managerName = managerName;
                    inconsistence.managerEmail = managerEmail;
                    inconsistence.creationDate = loadDateTime.ToString("M/d/yyyy");
                    inconsistence.startDateTime = startDateTime.ToString("M/d/yyyy HH:mm tt");
                    inconsistence.endDateTime = endDateTime.ToString("M/d/yyyy HH:mm tt");
                    inconsistence.report = parameter.Reporte;
                    inconsistence.activity = parameter.Actividad;
                    inconsistence.totalHours = parameter.totalHoras;
                    inconsistence.tool = "ARP";
                    inconsistence.status = issue;
                    inconsistence.problems = problemas;
                    inconsistence.actions = acciones;

                    inconsistences.Add(inconsistence);

                }

                for (var i = 0; i < tseInitialParameters.Count(); i++)
                {
                    var parameter = tseInitialParameters[i];
                    UserEntity? userEntity = _dataBaseService.UserEntity.AsNoTracking().Include("CountryEntity").Where(x => x.EmployeeCode == parameter.EmployeeCode).FirstOrDefault();
                    UserManagerEntity? userManagerEntity = _dataBaseService.UserManagerEntity.AsNoTracking().Where(x => x.EmployeeCode == parameter.EmployeeCode).FirstOrDefault();

                    var startDateTime = DateTime.ParseExact($"{parameter.FECHA_REP.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    string[] r1 = parameter.HoraInicio.Split(":");
                    string[] r2 = parameter.HoraFin.Split(":");
                    startDateTime = startDateTime.AddHours(int.Parse(r1[0])).AddMinutes(int.Parse(r1[1]));
                    var endDateTime = DateTime.ParseExact($"{parameter.FECHA_REP.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    r1 = parameter.HoraInicio.Split(":");
                    r2 = parameter.HoraFin.Split(":");
                    endDateTime = endDateTime.AddHours(int.Parse(r2[0])).AddMinutes(int.Parse(r2[1]));

                    var codigoPais = userEntity != null ? userEntity.CountryEntity.CodigoPais : "";
                    var empCode = userEntity != null ? userEntity.EmployeeCode : "";
                    var employeeName = userEntity != null ? $"{userEntity.NameUser} {userEntity.surnameUser}" : "";
                    var employeeEmail = userEntity != null ? userEntity.Email : "";
                    var managerName = userManagerEntity != null ? userManagerEntity.ManagerName : "";
                    var managerEmail = userManagerEntity != null ? userManagerEntity.ManagerEmail : "";
                    var problemas = parameter.Problemas != null ? parameter.Problemas : "";
                    var acciones = parameter.Acciones != null ? parameter.Acciones : "";
                    string issue = "";
                    switch (parameter.EstatusProceso)
                    {
                        case "NO_APLICA_X_HORARIO": { issue = "Error de horario"; break; }
                        case "NO_APLICA_X_OVERLAPING": { issue = "Overlaping"; break; }
                    }


                    InconsistenceModel inconsistence = new();
                    inconsistence.codigoPais = codigoPais;
                    inconsistence.employeeCode = empCode;
                    inconsistence.employeeName = employeeName;
                    inconsistence.employeeEmail = employeeEmail;
                    inconsistence.managerName = managerName;
                    inconsistence.managerEmail = managerEmail;
                    inconsistence.creationDate = loadDateTime.ToString("M/d/yyyy");
                    inconsistence.startDateTime = startDateTime.ToString("M/d/yyyy HH:mm tt");
                    inconsistence.endDateTime = endDateTime.ToString("M/d/yyyy HH:mm tt");
                    inconsistence.report = parameter.Reporte;
                    inconsistence.activity = parameter.Actividad;
                    inconsistence.totalHours = parameter.totalHoras;
                    inconsistence.tool = "ARP";
                    inconsistence.status = issue;
                    inconsistence.problems = problemas;
                    inconsistence.actions = acciones;

                    inconsistences.Add(inconsistence);

                }

                for (var i = 0; i < steInitialParameters.Count(); i++)
                {
                    var parameter = steInitialParameters[i];
                    UserEntity? userEntity = _dataBaseService.UserEntity.AsNoTracking().Include("CountryEntity").Where(x => x.EmployeeCode == parameter.EmployeeCode).FirstOrDefault();
                    UserManagerEntity? userManagerEntity = _dataBaseService.UserManagerEntity.AsNoTracking().Where(x => x.EmployeeCode == parameter.EmployeeCode).FirstOrDefault();

                    var startDateTime = DateTime.ParseExact($"{parameter.FECHA_REP.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    string[] r1 = parameter.HoraInicio.Split(":");
                    string[] r2 = parameter.HoraFin.Split(":");
                    startDateTime = startDateTime.AddHours(int.Parse(r1[0])).AddMinutes(int.Parse(r1[1]));
                    var endDateTime = DateTime.ParseExact($"{parameter.FECHA_REP.Substring(0, 10)} 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    r1 = parameter.HoraInicio.Split(":");
                    r2 = parameter.HoraFin.Split(":");
                    endDateTime = endDateTime.AddHours(int.Parse(r2[0])).AddMinutes(int.Parse(r2[1]));

                    var codigoPais = userEntity != null ? userEntity.CountryEntity.CodigoPais : "";
                    var empCode = userEntity != null ? userEntity.EmployeeCode : "";
                    var employeeName = userEntity != null ? $"{userEntity.NameUser} {userEntity.surnameUser}" : "";
                    var employeeEmail = userEntity != null ? userEntity.Email : "";
                    var managerName = userManagerEntity != null ? userManagerEntity.ManagerName : "";
                    var managerEmail = userManagerEntity != null ? userManagerEntity.ManagerEmail : "";
                    var problemas = parameter.Problemas != null ? parameter.Problemas : "";
                    var acciones = parameter.Acciones != null ? parameter.Acciones : "";
                    string issue = "";
                    switch (parameter.EstatusProceso)
                    {
                        case "NO_APLICA_X_HORARIO": { issue = "Error de horario"; break; }
                        case "NO_APLICA_X_OVERLAPING": { issue = "Overlaping"; break; }
                    }


                    InconsistenceModel inconsistence = new();
                    inconsistence.codigoPais = codigoPais;
                    inconsistence.employeeCode = empCode;
                    inconsistence.employeeName = employeeName;
                    inconsistence.employeeEmail = employeeEmail;
                    inconsistence.managerName = managerName;
                    inconsistence.managerEmail = managerEmail;
                    inconsistence.creationDate = loadDateTime.ToString("M/d/yyyy");
                    inconsistence.startDateTime = startDateTime.ToString("M/d/yyyy HH:mm tt");
                    inconsistence.endDateTime = endDateTime.ToString("M/d/yyyy HH:mm tt");
                    inconsistence.report = parameter.Reporte;
                    inconsistence.activity = parameter.Actividad;
                    inconsistence.totalHours = parameter.totalHoras;
                    inconsistence.tool = "ARP";
                    inconsistence.status = issue;
                    inconsistence.problems = problemas;
                    inconsistence.actions = acciones;

                    inconsistences.Add(inconsistence);

                }

            }

            return inconsistences;

        }

        public async Task<FileStreamResult> GenerateInconsistencesFile(string? idCarga, string? employeeCode = null) {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Hoja1");

            worksheet.Column("A").Width = 30;
            worksheet.Column("B").Width = 35;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 30;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 30;
            worksheet.Column("G").Width = 30;
            worksheet.Column("H").Width = 30;
            worksheet.Column("I").Width = 30;
            worksheet.Column("J").Width = 30;
            worksheet.Column("K").Width = 30;
            worksheet.Column("L").Width = 30;
            worksheet.Column("M").Width = 30;
            worksheet.Column("N").Width = 30;
            worksheet.Column("O").Width = 50;
            worksheet.Cell("A1").Value = "PAIS DE EMPLEADO";
            worksheet.Cell("B1").Value = "CODIGO DE EMPLEADO";
            worksheet.Cell("C1").Value = "NOMBRE DEL EMPLEADO";
            worksheet.Cell("D1").Value = "CORREO DEL EMPLEADO";
            worksheet.Cell("E1").Value = "NOMBRE DEL GERENTE";
            worksheet.Cell("F1").Value = "CORREO DEL GERENTE";
            worksheet.Cell("G1").Value = "FECHA DE GENERADO EL INFORME";
            worksheet.Cell("H1").Value = "FECHA Y HORA INICIO DEL REPORTE";
            worksheet.Cell("I1").Value = "FECHA Y HORA FIN DEL REPORTE";
            worksheet.Cell("J1").Value = "NUMERO DE CASO SI ES ARP TSE O STE";
            worksheet.Cell("K1").Value = "ACTIVIDAD DEL REPORTE";
            worksheet.Cell("L1").Value = "TOTAL HORAS";
            worksheet.Cell("M1").Value = "HERRAMIENTA TOOL DE DONDE SE GENERO";
            worksheet.Cell("N1").Value = "ESTADO PORTAL TLS OVERLAPING HORARIOS";
            worksheet.Cell("O1").Value = "COMENTARIOS O DETALLE DEL ERROR";

            var initialRow = 2;
            var currentRow = initialRow;
            var inconsistences = await GetInconsistences(idCarga, employeeCode);
            for (var i = 0; i < inconsistences.Count(); i++) {
                var inconsistence = inconsistences[i];

                worksheet.Cell(currentRow, 1).Value = inconsistence.codigoPais;
                worksheet.Cell(currentRow, 2).Value = inconsistence.employeeCode;
                worksheet.Cell(currentRow, 3).Value = inconsistence.employeeName;
                worksheet.Cell(currentRow, 4).Value = inconsistence.employeeEmail;
                worksheet.Cell(currentRow, 5).Value = inconsistence.managerName;
                worksheet.Cell(currentRow, 6).Value = inconsistence.managerEmail;
                worksheet.Cell(currentRow, 7).Value = inconsistence.creationDate;
                worksheet.Cell(currentRow, 8).Value = inconsistence.startDateTime;
                worksheet.Cell(currentRow, 9).Value = inconsistence.endDateTime;
                worksheet.Cell(currentRow, 10).Value = inconsistence.report;
                worksheet.Cell(currentRow, 11).Value = inconsistence.activity;
                worksheet.Cell(currentRow, 12).Value = inconsistence.totalHours;
                worksheet.Cell(currentRow, 13).Value = inconsistence.tool;
                worksheet.Cell(currentRow, 14).Value = inconsistence.status;
                var rt = worksheet.Cell(currentRow, 15).CreateRichText();
                var rs = rt.AddText("Problema:");
                rs.SetFontColor(XLColor.Blue);
                rt.AddText(inconsistence.problems);
                rt.AddNewLine();
                rs = rt.AddText("Acciones:");
                rs.SetFontColor(XLColor.Red);
                rt.AddText(inconsistence.actions);
                worksheet.Cell(currentRow, 15).Style.Alignment.WrapText = true;

                currentRow++;
            }

            var table = worksheet.Range($"A1:O{(currentRow - 1)}").CreateTable("Table");
            table.Theme = XLTableTheme.TableStyleLight2;

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "Plantilla de Inconsistencias.xlsx" };

        }

        private void addRowInNotificationFile<T>(IXLWorksheet worksheet, T parameter) { 
            
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

        private void PreaprobadosARP(List<UserEntity> UserLst, List<ParametersArpInitialEntity> rowParameterFinal)
        {
            DateTime dateTime = DateTime.Now;
            List<string> employeeCodes = new List<string>();

            for (var i = 0; i < rowParameterFinal.Count(); i++)
            {
                var p = rowParameterFinal[i];
                employeeCodes.Add(p.EmployeeCode);
                var reportDateTime = DateTime.ParseExact(p.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (reportDateTime < dateTime) dateTime = reportDateTime;
            }

            System.String employeeCodesIn = $"'{System.String.Join("','", employeeCodes.ToArray())}'";
            var horusReports = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h INNER JOIN \"UserEntity\" u on u.\"IdUser\" = h.\"UserEntityId\" WHERE u.\"EmployeeCode\" IN ({employeeCodesIn}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND (h.\"EstatusFinal\" != 'RECHAZADO' and h.\"EstatusFinal\" != 'DESCARTADO')")
                .Include(x => x.UserEntity)
                .AsEnumerable()
                .OrderByDescending(x => DateTime.ParseExact(x.strCreationDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
                .ToList();

            foreach (var itemARPp in rowParameterFinal)
            {
                //------------------------------------------------------------------------------------------------------------------
                var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemARPp.EmployeeCode);

                string[] r1 = itemARPp.HoraInicio.Split(":");
                string[] r2 = itemARPp.HoraFin.Split(":");
                var startTime = DateTime.Parse("00:00:00").AddHours(int.Parse(r1[0])).AddMinutes(int.Parse(r1[1]));
                var endTime = DateTime.Parse("00:00:00").AddHours(int.Parse(r2[0])).AddMinutes(int.Parse(r2[1]));

                DateTime fechaHoraOriginal = DateTime.ParseExact(itemARPp.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");

                //Escenario coincidencia 100% 
                //=================================================================
                var _horusCoincidencia = horusReports
                    .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.StartTime == itemARPp.HoraInicio && h.EndTime == itemARPp.HoraFin && h.UserEntityId == userRow.IdUser && (h.EstatusFinal != "RECHAZADO" && h.EstatusFinal != "DESCARTADO"))
                    .Where(h => h.StrReport.Split("-")[0] == itemARPp.Reporte.Split("-")[0])
                    .FirstOrDefault();

                if (_horusCoincidencia != null)
                {
                    if (_horusCoincidencia.EstatusOrigen != "EXTRACTED")
                    {
                        _horusCoincidencia.DetalleEstatusFinal = $"Actualización de ESTATUS ORIGEN {_horusCoincidencia.EstatusOrigen} a {itemARPp.EstatusOrigen}";
                        _horusCoincidencia.EstatusOrigen = itemARPp.EstatusOrigen; 

                        if (itemARPp.EstatusOrigen == "EXTRACTED") {
                            if (_horusCoincidencia.EstatusFinal == "PREAPROBADO") {
                                _horusCoincidencia.EstatusFinal = "APROBADO";
                            }
                        }
                    }

                    itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
            }
        }

        private void EscenarioCoincidenciaParcialARP(ParametersArpInitialEntity? itemARPp, UserEntity? userRow, DateTime startTime, DateTime endTime, string nuevaFechaHoraFormato, List<HorusReportEntity> _horusCoincidencia)
        {
            //------------------------------------------------------------------------------------------------------------------

            //Escenario coincidencia Parcial
            //=================================================================
            var _horusCoincidenciaParcial = _dataBaseService.HorusReportEntity
                .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == userRow.IdUser && (h.EstatusFinal != "RECHAZADO" && h.EstatusFinal != "DESCARTADO"))
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemARPp.HoraInicio, itemARPp.HoraFin) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                TimeInRange(h.EndTime, startTime, endTime)))
                .Where(h => h.StrReport.Split("-")[0] == itemARPp.Reporte.Split("-")[0])
                .ToList();




            if (_horusCoincidenciaParcial.Count > 0)
            {

                if (_horusCoincidenciaParcial[0].EstatusOrigen == "EXTRACTED" && itemARPp.EstatusOrigen == "EXTRACTED" && _horusCoincidenciaParcial[0].StartTime == itemARPp.HoraInicio && _horusCoincidenciaParcial[0].EndTime == itemARPp.HoraFin)
                {
                    itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemARPp.EstatusOrigen == "EXTRACTED" && (_horusCoincidenciaParcial[0].StartTime != itemARPp.HoraInicio || _horusCoincidenciaParcial[0].EndTime != itemARPp.HoraFin))
                {
                    itemARPp.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED y Horas Diferentes";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemARPp.EstatusOrigen == "EXTRACTED" && (_horusCoincidenciaParcial[0].StartTime == itemARPp.HoraInicio || _horusCoincidenciaParcial[0].EndTime == itemARPp.HoraFin))
                {
                    itemARPp.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED y Horas Iguales";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemARPp.EstatusOrigen == "FINAL" || itemARPp.EstatusOrigen == "SUBMITTED") && (_horusCoincidenciaParcial[0].StartTime != itemARPp.HoraInicio || _horusCoincidenciaParcial[0].EndTime != itemARPp.HoraFin))
                {
                    itemARPp.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a (FINAL/SUBMITTED) y Horas Diferentes";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemARPp.EstatusOrigen == "FINAL" || itemARPp.EstatusOrigen == "SUBMITTED") && (_horusCoincidenciaParcial[0].StartTime == itemARPp.HoraInicio || _horusCoincidenciaParcial[0].EndTime == itemARPp.HoraFin))
                {
                    itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
            }
            //------------------------------------------------------------------------------------------------------------------
        }

        private void EscenarioCoincidenciaTotalARP(ParametersArpInitialEntity? itemARPp, List<HorusReportEntity> _horusCoincidencia)
        {
            if (_horusCoincidencia[0].EstatusOrigen == "EXTRACTED" && itemARPp.EstatusOrigen == "EXTRACTED")
            {
                itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
            }
            else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemARPp.EstatusOrigen == "EXTRACTED")
            {
                itemARPp.EstatusProceso = "EN_OVERTIME";
                _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED";
                discardReport(_horusCoincidencia[0]);
            }
            else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemARPp.EstatusOrigen == "FINAL" || itemARPp.EstatusOrigen == "SUBMITTED"))
            {
                itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
            }
        }

        private void PreaprobadosTSE(List<UserEntity> UserLst, List<ParametersTseInitialEntity> rowTSEParameterFinal)
        {
            DateTime dateTime = DateTime.Now;
            List<string> employeeCodes = new List<string>();

            for (var i = 0; i < rowTSEParameterFinal.Count(); i++)
            {
                var p = rowTSEParameterFinal[i];
                employeeCodes.Add(p.EmployeeCode);
                var reportDateTime = DateTime.ParseExact(p.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (reportDateTime < dateTime) dateTime = reportDateTime;
            }

            System.String employeeCodesIn = $"'{System.String.Join("','", employeeCodes.ToArray())}'";
            var horusReports = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h INNER JOIN \"UserEntity\" u on u.\"IdUser\" = h.\"UserEntityId\" WHERE u.\"EmployeeCode\" IN ({employeeCodesIn}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND (h.\"EstatusFinal\" != 'RECHAZADO' and h.\"EstatusFinal\" != 'DESCARTADO')")
                .Include(x => x.UserEntity)
                .AsEnumerable()
                .OrderByDescending(x => DateTime.ParseExact(x.strCreationDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
                .ToList();

            foreach (var itemTSE in rowTSEParameterFinal)
            {
                //------------------------------------------------------------------------------------------------------------------
                var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemTSE.EmployeeCode);


                string[] r1 = itemTSE.HoraInicio.Split(":");
                string[] r2 = itemTSE.HoraFin.Split(":");
                var startTime = DateTime.Parse("00:00:00").AddHours(int.Parse(r1[0])).AddMinutes(int.Parse(r1[1]));
                var endTime = DateTime.Parse("00:00:00").AddHours(int.Parse(r2[0])).AddMinutes(int.Parse(r2[1]));

                DateTime fechaHoraOriginal = DateTime.ParseExact(itemTSE.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");

                //Escenario coincidencia 100%
                //=================================================================
                var _horusCoincidencia = horusReports
                    .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.StartTime == itemTSE.HoraInicio && h.EndTime == itemTSE.HoraFin && h.UserEntityId == userRow.IdUser && (h.EstatusFinal != "RECHAZADO" && h.EstatusFinal != "DESCARTADO"))
                    .Where(h => h.StrReport.Split("-")[0] == itemTSE.Reporte.Split("-")[0])
                    .FirstOrDefault();

                if (_horusCoincidencia != null)
                {
                    if (_horusCoincidencia.EstatusOrigen != "EXTRACTED")
                    {
                        _horusCoincidencia.DetalleEstatusFinal = $"Actualización de ESTATUS ORIGEN {_horusCoincidencia.EstatusOrigen} a {itemTSE.EstatusOrigen}";
                        _horusCoincidencia.EstatusOrigen = itemTSE.EstatusOrigen;

                        if (itemTSE.EstatusOrigen == "EXTRACTED")
                        {
                            if (_horusCoincidencia.EstatusFinal == "PREAPROBADO")
                            {
                                _horusCoincidencia.EstatusFinal = "APROBADO";
                            }
                        }
                    }

                    itemTSE.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
            }
        }        

        private void EscenarioCoincidenciaParcialTSE(ParametersTseInitialEntity? itemTSE, UserEntity? userRow, DateTime startTime, DateTime endTime, string nuevaFechaHoraFormato, List<HorusReportEntity> _horusCoincidencia)
        {
            //------------------------------------------------------------------------------------------------------------------

            //Escenario coincidencia Parcial
            //=================================================================
            var _horusCoincidenciaParcial = _dataBaseService.HorusReportEntity
                .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == userRow.IdUser && (h.EstatusFinal != "RECHAZADO" && h.EstatusFinal != "DESCARTADO"))
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemTSE.HoraInicio, itemTSE.HoraFin) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                TimeInRange(h.EndTime, startTime, endTime)))
                .Where(h => h.StrReport.Split("-")[0] == itemTSE.Reporte.Split("-")[0])
                .ToList();




            if (_horusCoincidenciaParcial.Count > 0)
            {

                if (_horusCoincidenciaParcial[0].EstatusOrigen == "EXTRACTED" && itemTSE.EstatusOrigen == "EXTRACTED" && _horusCoincidenciaParcial[0].StartTime == itemTSE.HoraInicio && _horusCoincidenciaParcial[0].EndTime == itemTSE.HoraFin)
                {
                    itemTSE.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemTSE.EstatusOrigen == "EXTRACTED" && (_horusCoincidenciaParcial[0].StartTime != itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime != itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED y Horas Diferentes";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemTSE.EstatusOrigen == "EXTRACTED" && (_horusCoincidenciaParcial[0].StartTime == itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime == itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED y Horas Iguales";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemTSE.EstatusOrigen == "FINAL" || itemTSE.EstatusOrigen == "SUBMITTED") && (_horusCoincidenciaParcial[0].StartTime != itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime != itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a (FINAL/SUBMITTED) y Horas Diferentes";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemTSE.EstatusOrigen == "FINAL" || itemTSE.EstatusOrigen == "SUBMITTED") && (_horusCoincidenciaParcial[0].StartTime == itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime == itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
            }
            //------------------------------------------------------------------------------------------------------------------
        }

        private void EscenarioCoincidenciaTotalTSE(ParametersTseInitialEntity? itemARPp, List<HorusReportEntity> _horusCoincidencia)
        {
            if (_horusCoincidencia[0].EstatusOrigen == "EXTRACTED" && itemARPp.EstatusOrigen == "EXTRACTED")
            {
                itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
            }
            else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemARPp.EstatusOrigen == "EXTRACTED")
            {
                itemARPp.EstatusProceso = "EN_OVERTIME";
                _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED";
                discardReport(_horusCoincidencia[0]);
            }
            else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemARPp.EstatusOrigen == "FINAL" || itemARPp.EstatusOrigen == "SUBMITTED"))
            {
                itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
            }
        }

        private void PreaprobadosSTE(List<UserEntity> UserLst, List<ParametersSteInitialEntity> rowTSEParameterFinal)
        {
            DateTime dateTime = DateTime.Now;
            List<string> employeeCodes = new List<string>();

            for (var i = 0; i < rowTSEParameterFinal.Count(); i++)
            {
                var p = rowTSEParameterFinal[i];
                employeeCodes.Add(p.EmployeeCode);
                var reportDateTime = DateTime.ParseExact(p.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (reportDateTime < dateTime) dateTime = reportDateTime;
            }

            System.String employeeCodesIn = $"'{System.String.Join("','", employeeCodes.ToArray())}'";
            var horusReports = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h INNER JOIN \"UserEntity\" u on u.\"IdUser\" = h.\"UserEntityId\" WHERE u.\"EmployeeCode\" IN ({employeeCodesIn}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI') AND (h.\"EstatusFinal\" != 'RECHAZADO' and h.\"EstatusFinal\" != 'DESCARTADO')")
                .Include(x => x.UserEntity)
                .AsEnumerable()
                .OrderByDescending(x => DateTime.ParseExact(x.strCreationDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
                .ToList();

            foreach (var itemSTE in rowTSEParameterFinal)
            {
                //------------------------------------------------------------------------------------------------------------------
                var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemSTE.EmployeeCode);

                string[] r1 = itemSTE.HoraInicio.Split(":");
                string[] r2 = itemSTE.HoraFin.Split(":");
                var startTime = DateTime.Parse("00:00:00").AddHours(int.Parse(r1[0])).AddMinutes(int.Parse(r1[1]));
                var endTime = DateTime.Parse("00:00:00").AddHours(int.Parse(r2[0])).AddMinutes(int.Parse(r2[1]));

                DateTime fechaHoraOriginal = DateTime.ParseExact(itemSTE.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");

                //Escenario coincidencia 100%
                //================================================================= 
                var _horusCoincidencia = horusReports
                    .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.StartTime == itemSTE.HoraInicio && h.EndTime == itemSTE.HoraFin && h.UserEntityId == userRow.IdUser && (h.EstatusFinal != "RECHAZADO" && h.EstatusFinal != "DESCARTADO"))
                    .Where(h => h.StrReport.Split("-")[0] == itemSTE.Reporte.Split("-")[0])
                    .FirstOrDefault();

                if (_horusCoincidencia != null)
                {
                    if (_horusCoincidencia.EstatusOrigen != "STE")
                    {
                        _horusCoincidencia.DetalleEstatusFinal = $"Actualización de ESTATUS ORIGEN {_horusCoincidencia.EstatusOrigen} a {itemSTE.EstatusOrigen}";
                        _horusCoincidencia.EstatusOrigen = itemSTE.EstatusOrigen;

                        if (itemSTE.EstatusOrigen == "STE")
                        {
                            if (_horusCoincidencia.EstatusFinal == "PREAPROBADO")
                            {
                                _horusCoincidencia.EstatusFinal = "APROBADO";
                            }
                        }
                    }

                    itemSTE.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
            }
        }

        private void EscenarioCoincidenciaParcialSTE(ParametersSteInitialEntity? itemTSE, UserEntity? userRow, DateTime startTime, DateTime endTime, string nuevaFechaHoraFormato, List<HorusReportEntity> _horusCoincidencia)
        {
            //------------------------------------------------------------------------------------------------------------------

            //Escenario coincidencia Parcial
            //=================================================================
            var _horusCoincidenciaParcial = _dataBaseService.HorusReportEntity
                .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == userRow.IdUser && (h.EstatusFinal != "RECHAZADO" && h.EstatusFinal != "DESCARTADO"))
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemTSE.HoraInicio, itemTSE.HoraFin) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                TimeInRange(h.EndTime, startTime, endTime)))
                .Where(h => h.StrReport.Split("-")[0] == itemTSE.Reporte.Split("-")[0])
                .ToList();




            if (_horusCoincidenciaParcial.Count > 0)
            {

                if (_horusCoincidenciaParcial[0].EstatusOrigen == "EXTRACTED" && itemTSE.EstatusOrigen == "EXTRACTED" && _horusCoincidenciaParcial[0].StartTime == itemTSE.HoraInicio && _horusCoincidenciaParcial[0].EndTime == itemTSE.HoraFin)
                {
                    itemTSE.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemTSE.EstatusOrigen == "EXTRACTED" && (_horusCoincidenciaParcial[0].StartTime != itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime != itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED y Horas Diferentes";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemTSE.EstatusOrigen == "EXTRACTED" && (_horusCoincidenciaParcial[0].StartTime == itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime == itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED y Horas Iguales";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemTSE.EstatusOrigen == "FINAL" || itemTSE.EstatusOrigen == "SUBMITTED") && (_horusCoincidenciaParcial[0].StartTime != itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime != itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "EN_OVERTIME";
                    _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a (FINAL/SUBMITTED) y Horas Diferentes";
                    discardReport(_horusCoincidencia[0]);
                }
                else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemTSE.EstatusOrigen == "FINAL" || itemTSE.EstatusOrigen == "SUBMITTED") && (_horusCoincidenciaParcial[0].StartTime == itemTSE.HoraInicio || _horusCoincidenciaParcial[0].EndTime == itemTSE.HoraFin))
                {
                    itemTSE.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
                }
            }
            //------------------------------------------------------------------------------------------------------------------
        }

        private void EscenarioCoincidenciaTotalSTE(ParametersSteInitialEntity? itemARPp, List<HorusReportEntity> _horusCoincidencia)
        {
            if (_horusCoincidencia[0].EstatusOrigen == "EXTRACTED" && itemARPp.EstatusOrigen == "EXTRACTED")
            {
                itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
            }
            else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && itemARPp.EstatusOrigen == "EXTRACTED")
            {
                itemARPp.EstatusProceso = "EN_OVERTIME";
                _horusCoincidencia[0].DetalleEstatusFinal = "Actualización de ESTATUS (FINAL/SUBMITTED) a EXTRACTED";
                discardReport(_horusCoincidencia[0]);
            }
            else if ((_horusCoincidencia[0].EstatusOrigen == "FINAL" || _horusCoincidencia[0].EstatusOrigen == "SUBMITTED") && (itemARPp.EstatusOrigen == "FINAL" || itemARPp.EstatusOrigen == "SUBMITTED"))
            {
                itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING_INTERNO";
            }
        }


        private async Task updateCargaStatus(string idCarga,string message)
        {
            _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(idCarga)).ToList().
                                          ForEach(x => x.EstadoCarga = message);
            await _dataBaseService.SaveAsync();
        }

        public async Task<string> LoadUserGMT(LoadJsonUserGMT model)
        {
            try
            {
                _dataBaseService.UserZonaHoraria.ExecuteDelete();
                await _dataBaseService.SaveAsync();

                List<UserZonaHoraria> listUserZonaHoraria = new();

                //Serializa la carga completa del excel
                List<UserZonaHoraria> datosUSZONExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserZonaHoraria>>(model.Data.ToJsonString());

                //Remueve duplicados 
                var datosUSZONExcel = datosUSZONExcelFull.DistinctBy(m => new { m.EmployeeCode, m.ZonaHorariaU}).ToList();

                //Aplica validaciones del PROCESO TLS para overtime
                foreach (var entity in datosUSZONExcel)
                {
                    var oUserZonaH = new UserZonaHoraria() { IdUserZone = new Guid(),EmployeeCode=entity.EmployeeCode,ZonaHorariaU=entity.ZonaHorariaU };
                    listUserZonaHoraria.Add(oUserZonaH);
                }
                _dataBaseService.UserZonaHoraria.AddRange(listUserZonaHoraria);
                await _dataBaseService.SaveAsync();

                return "Success";

            }
            catch (Exception ex)
            {
                return "Fail";
            }

        }
    }

    public class ReportOvertime
    {
        public decimal rInicio { get; set; }
        public decimal rFin { get; set; }

        public decimal hInicio { get; set; }
        public decimal hFin { get; set; }
        public List<HoraRegistro> horarioref { get; set; }

        public decimal rInicioOK { get; set; }
        public decimal rFinOK { get; set; }
        public decimal rInicio2OK { get; set; }
        public decimal rFin2OK { get; set; }


        public void generaHorarioDefinicion()
        {
            horarioref = new List<HoraRegistro>();

            if(this.rFin < this.rInicio)
            {
                //Escenario con 2 dias involucrados
                this.rFin = (decimal)24.00 + this.rFin;

                for (decimal i = this.rInicio; i <= this.rFin; i += (decimal)0.01)
                {
                    if (i.ToString().EndsWith(".60"))
                    {
                        i += (decimal)0.39;
                        continue;
                    }

                    var _tipo = "OVERTIME1";

                    if (i >= this.hInicio && i <= this.hFin) _tipo = "HORARIO";
                    else
                    {

                        if (i <= (decimal)23.59)
                        {
                            _tipo = "OVERTIME1";
                        }
                        else
                        {
                            _tipo = "OVERTIME2";
                        }

                    }


                    horarioref.Add(new HoraRegistro()
                    {
                        hora = i>(decimal)23.59? i-24:i,
                        tipo = _tipo
                    });
                }



            }
            else
            {
                //Escenario con 1 dia involucrados
                for (decimal i = this.rInicio; i <= this.rFin; i += (decimal)0.01)
                {
                    if (i.ToString().EndsWith(".60"))
                    {
                        i += (decimal)0.39;
                        continue;
                    }

                    var _tipo = "OVERTIME1";

                    if (i >= this.hInicio && i <= this.hFin) _tipo = "HORARIO";
                    else
                    {

                        if (i <= this.hInicio)
                        {
                            _tipo = "OVERTIME1";
                        }
                        else
                        {
                            _tipo = "OVERTIME2";
                        }

                    }


                    horarioref.Add(new HoraRegistro()
                    {
                        hora = i,
                        tipo = _tipo
                    });
                }
            }


            

        }

        public void detectaFirstAndLast()
        {
            try
            {
            
            this.rInicioOK = this.horarioref.FirstOrDefault(s => s.tipo == "OVERTIME1").hora;
            this.rFinOK = this.horarioref.LastOrDefault(s => s.tipo == "OVERTIME1").hora;
            }catch(Exception ex)
            {
                this.rInicioOK =400;
                this.rFinOK = 400;
            }

            try
            {
                this.rInicio2OK = this.horarioref.FirstOrDefault(s => s.tipo == "OVERTIME2").hora;
                this.rFin2OK = this.horarioref.LastOrDefault(s => s.tipo == "OVERTIME2").hora;
            }
            catch (Exception)
            {

                this.rInicio2OK = 400;
                this.rFin2OK = 400;
            }

           
        }
    }

    public class HoraRegistro
    {
        public decimal hora { get; set; }
        public string tipo { get; set; }
        
    }

    public interface IRange
    {
        DateTime StartRango { get; }
        DateTime EndRango { get; }
        bool WithOverlapping(DateTime value);
        bool WithOverlapping(IRange range);
    }

    public class DateRange : IRange
    {
        public DateTime StartRango { get; set; }
        public DateTime EndRango { get; set; }

        

        public DateRange(DateTime start, DateTime end)
        {
            StartRango = start;
            EndRango = end;
        }

        public bool WithOverlapping(DateTime value)
        {
            if ((StartRango <= value) && (value <= EndRango))return true;
            else return false;

           
        }

        public bool WithOverlapping(IRange range)
        {
            return (StartRango <= range.StartRango) && (range.EndRango <= EndRango);
        }

        
    }

    public class Parametrosaux
    {
        public string FECHA_REP { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string EmployeeCode { get; set; }
    }


}
