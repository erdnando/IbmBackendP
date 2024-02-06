﻿using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.PortalDB.Commands;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.User.Commands.GetManager;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public LoadHoursReport(IDataBaseService dataBaseService, IMapper mapper, IConsultCountryCommand consultCountryCommand, IEmailCommand emailCommand, IConfiguration configuration)
        {
            _dataBaseService = dataBaseService;
            _configuration = configuration;
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


            int arpEnProceso = _dataBaseService.ParametersArpInitialEntity.Where(e => e.IdCarga == new Guid(idCarga)).ToList().Count();
            int tseEnProceso = _dataBaseService.ParametersTseInitialEntity.Where(e => e.IdCarga == new Guid(idCarga)).ToList().Count();
            int steEnProceso = _dataBaseService.ParametersSteInitialEntity.Where(e => e.IdCarga == new Guid(idCarga)).ToList().Count();
            var cargaRef = _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(idCarga)).FirstOrDefault();

            var aRPCarga = Int32.Parse(cargaRef!.ARPCarga == "" ? "0" : cargaRef.ARPCarga);
            var tSECarga = Int32.Parse(cargaRef!.TSECarga == "" ? "0" : cargaRef.TSECarga);
            var sTECarga = Int32.Parse(cargaRef!.STECarga == "" ? "0" : cargaRef.STECarga);

            contadores.arp = aRPCarga != 0 ? (Int32)Math.Ceiling((double)arpEnProceso * 100 / aRPCarga) : 0;
            contadores.tse = tSECarga != 0 ? (Int32)Math.Ceiling((double)tseEnProceso * 100 / tSECarga) : 0;
            contadores.ste = sTECarga != 0 ? (Int32)Math.Ceiling((double)steEnProceso * 100 / sTECarga) : 0;
            contadores.total = (contadores.arp + contadores.tse + contadores.ste) / 3;
            contadores.estadoCarga = cargaRef.Estado;
            return contadores;
        }


        public async Task<string> GeneraCarga()
        {
            try
            {
                
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
                    TSEXDatosNovalidos = "0"

                };

                 _dataBaseService.ARPLoadEntity.AddAsync(aRPLoadEntity);
                await _dataBaseService.SaveAsync();
                return aRPLoadEntity.IdArpLoad.ToString();
            }catch(Exception ex)
            {
                return "-1";
            }
        }

        public async Task<string> LoadARP(LoadJsonPais model)
        {

            try { 

                Int64 counter = 0;
                List<ParametersArpInitialEntity> listParametersInitialEntity = new();


                //Serializa la carga completa del excel
                List<ARPLoadDetailEntity> datosARPExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ARPLoadDetailEntity>>(model.Data.ToJsonString());
               
                
              

                //Remueve duplicados 
                var datosARPExcel = datosARPExcelFull.DistinctBy(m => new { m.ID_EMPLEADO, m.FECHA_REP, m.HORA_INICIO,m.HORA_FIN }).ToList();

                //Setting duplicados metrica en ARP
                 _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.ARPOmitidosXDuplicidad = (datosARPExcelFull.Count()- datosARPExcel.Count()).ToString() );
                 await _dataBaseService.SaveAsync();


                //Actualiza metricas en la carga acerca de cuantos registros se procesaran
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == Guid.Parse(model.IdCarga)).ToList().
                                              ForEach(x => x.ARPCarga = datosARPExcel.Count.ToString());

                 _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == Guid.Parse(model.IdCarga)).ToList().
                                              ForEach(x => x.ARPOmitidos = (datosARPExcel.Count - datosARPExcel.Count).ToString() );

                await _dataBaseService.SaveAsync();

                List<string> politicaOvertime = new() { "Vacations", "Absence", "Holiday", "Stand By" };

                var semanahorario = new DateTimeOffset();

                CultureInfo cul = CultureInfo.CurrentCulture;
                List<HorarioReturn> fueraH = new List<HorarioReturn>();

                var listaCountries = await _consultCountryCommand.List();
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
                    var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == entity.ID_EMPLEADO.Substring(entity.ID_EMPLEADO.Length - 3));


                    if (paisRegistro == null)
                    {
                        var arpx2 = fHomologaARP(entity!);
                        //PAIS no valido
                        ParametersArpInitialEntity parametersARPInitialEntity = new ParametersArpInitialEntity();

                        parametersARPInitialEntity.IdParametersInitialEntity = Guid.NewGuid();
                        parametersARPInitialEntity.EstatusProceso = "NO_APLICA_X_PAIS";
                        parametersARPInitialEntity.FECHA_REP = arpx2.FECHA_REP;
                        parametersARPInitialEntity.TOTAL_MINUTOS = arpx2.TOTAL_MINUTOS;
                        parametersARPInitialEntity.totalHoras = getHoras(arpx2.TOTAL_MINUTOS);
                        parametersARPInitialEntity.HorasInicio = 0;
                        parametersARPInitialEntity.HorasFin = 0;
                        parametersARPInitialEntity.EmployeeCode = arpx2.ID_EMPLEADO;
                        parametersARPInitialEntity.IdCarga = new Guid(model.IdCarga);
                        parametersARPInitialEntity.HoraInicio = "0";
                        parametersARPInitialEntity.HoraFin = "0";
                        parametersARPInitialEntity.OutIme = "N";
                        parametersARPInitialEntity.Semana = 0;
                        parametersARPInitialEntity.HoraInicioHoraio = "0";
                        parametersARPInitialEntity.HoraFinHorario = "0";
                        parametersARPInitialEntity.OverTime = "N";
                        parametersARPInitialEntity.Anio = semanahorario.Year.ToString();
                        parametersARPInitialEntity.Festivo = "N";
                        parametersARPInitialEntity.Estado = "";
                        parametersARPInitialEntity.Reporte = arpx2.DOC_NUM;

                        listParametersInitialEntity.Add(parametersARPInitialEntity);
                        
                        continue;
                    }
                    
                    var arpx = fHomologaARP(entity);

                   
                    if (arpx == null)
                    {
                        ParametersArpInitialEntity parametersARPInitialEntity = new ParametersArpInitialEntity();

                        parametersARPInitialEntity.IdParametersInitialEntity = Guid.NewGuid();
                        parametersARPInitialEntity.EstatusProceso = "NO_APLICA_X_FALTA_DATOS";
                        parametersARPInitialEntity.FECHA_REP = entity.FECHA_REP;
                        parametersARPInitialEntity.TOTAL_MINUTOS = entity.TOTAL_MINUTOS;
                        parametersARPInitialEntity.totalHoras = getHoras(entity.TOTAL_MINUTOS);
                        parametersARPInitialEntity.HorasInicio = 0;
                        parametersARPInitialEntity.HorasFin = 0;
                        parametersARPInitialEntity.EmployeeCode = entity.ID_EMPLEADO;
                        parametersARPInitialEntity.IdCarga = new Guid(model.IdCarga);

                        
                        parametersARPInitialEntity.HoraInicio = "0";
                        parametersARPInitialEntity.HoraFin = "0";
                        
                        parametersARPInitialEntity.OutIme = "N";
                        parametersARPInitialEntity.Semana = 0;
                        parametersARPInitialEntity.HoraInicioHoraio = "0";
                        parametersARPInitialEntity.HoraFinHorario = "0";
                        parametersARPInitialEntity.OverTime = "N";
                        parametersARPInitialEntity.Anio = semanahorario.Year.ToString();
                        parametersARPInitialEntity.Festivo = "N";
                        parametersARPInitialEntity.Estado = "";
                        parametersARPInitialEntity.Reporte = entity.DOC_NUM;

                        listParametersInitialEntity.Add(parametersARPInitialEntity);
                        continue;

                    }


                    var arp = validaHoraGMT(arpx, horariosGMT, paisGeneral);


                    arp.ARPLoadEntityId = new Guid(model.IdCarga);


                    try
                    {
                        semanahorario = DateTimeOffset.ParseExact(arp.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch(Exception ex)
                    {
                        ParametersArpInitialEntity parametersARPInitialEntity = new ParametersArpInitialEntity();

                        parametersARPInitialEntity.IdParametersInitialEntity = Guid.NewGuid();
                        parametersARPInitialEntity.EstatusProceso = "NO_APLICA_X_FORMATO_FECHA";
                        parametersARPInitialEntity.FECHA_REP = entity.FECHA_REP;
                        parametersARPInitialEntity.TOTAL_MINUTOS = entity.TOTAL_MINUTOS;
                        parametersARPInitialEntity.totalHoras = getHoras(entity.TOTAL_MINUTOS);
                        parametersARPInitialEntity.HorasInicio = 0;
                        parametersARPInitialEntity.HorasFin = 0;
                        parametersARPInitialEntity.EmployeeCode = entity.ID_EMPLEADO;
                        parametersARPInitialEntity.IdCarga = new Guid(model.IdCarga);


                        parametersARPInitialEntity.HoraInicio = "0";
                        parametersARPInitialEntity.HoraFin = "0";

                        parametersARPInitialEntity.OutIme = "N";
                        parametersARPInitialEntity.Semana = 0;
                        parametersARPInitialEntity.HoraInicioHoraio = "0";
                        parametersARPInitialEntity.HoraFinHorario = "0";
                        parametersARPInitialEntity.OverTime = "N";
                        parametersARPInitialEntity.Anio = semanahorario.Year.ToString();
                        parametersARPInitialEntity.Festivo = "N";
                        parametersARPInitialEntity.Estado = "";
                        parametersARPInitialEntity.Reporte = entity.DOC_NUM;

                        listParametersInitialEntity.Add(parametersARPInitialEntity);
                        continue;
                    }
                    

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
                    parametersInitialEntity.Reporte = arp.DOC_NUM;





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
                        parametersInitialEntity.Reporte = arp.DOC_NUM;
                        parametersInitialEntity.EstatusProceso = "NO_APLICA_X_HORARIO";
                        listParametersInitialEntity.Add(parametersInitialEntity);
                        continue;
                    }


                    listParametersInitialEntity.Add(parametersInitialEntity);
                }

               
               int PAGE_SIZE = listParametersInitialEntity.Count()/10;
                List<List<ParametersArpInitialEntity>> partitions = listParametersInitialEntity.partition(PAGE_SIZE);

                for (int i = 0; i < partitions.Count(); i++)
                {
                    var endpoint= i % 2 == 0 ? "1" : "2";
                    try
                    {
                        LoadGeneric(partitions[i], endpoint);
                    }
                    catch(Exception ex)
                    {
                        var error = ex.Message;
                    }
                    
                }

              

                // _dataBaseService.ParametersArpInitialEntity.AddRange(listParametersInitialEntity);
                // await _dataBaseService.SaveAsync();


                return model.IdCarga.ToString();





            }
            catch (Exception ex){
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
            

            try { 
            
                List<TSELoadEntity> datosTSEExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TSELoadEntity>>(model.Data.ToJsonString());
                //datosTSEExcel.Where(e => e.Status.Trim() == "EXTRACTED").ToList().ForEach(x => x.Status = "Extracted");
               // datosTSEExcel.Where(e => e.Status.Trim() == "SUBMITTED").ToList().ForEach(x => x.Status = "Submitted");

                //List<TSELoadEntity> datosTSEExcel = datosTSEExcelFull!.Where(x => x.Status != "Extractedx" && x.Status != "Finalx" && x.Status != "Submittedx").ToList();


                //Remueve duplicados 
                var datosTSEExcel = datosTSEExcelFull.DistinctBy(m => new { m.NumeroEmpleado, m.StartTime, m.EndTime }).ToList();

                //Setting duplicados metrica en TSE
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEOmitidosXDuplicidad = (datosTSEExcelFull.Count() - datosTSEExcel.Count()).ToString());
                await _dataBaseService.SaveAsync();


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

                try
                {

               
                foreach (var registrox in datosTSEExcel)
                {

                        var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == registrox.NumeroEmpleado.Substring(registrox.NumeroEmpleado.Length - 3));
                        //valida pais
                        if (paisRegistro == null)
                        {
                            //PAIS no valido
                            ParametersTseInitialEntity parametersTseInitialEntityaux = new ParametersTseInitialEntity();

                            parametersTseInitialEntityaux.IdParamTSEInitialId = Guid.NewGuid();
                            parametersTseInitialEntityaux.EstatusProceso = "NO_APLICA_X_PAIS";
                            parametersTseInitialEntityaux.FECHA_REP = registrox.StartTime;
                            parametersTseInitialEntityaux.TOTAL_MINUTOS = getMins(registrox.DurationInHours);
                            parametersTseInitialEntityaux.totalHoras = registrox.DurationInHours;
                            parametersTseInitialEntityaux.HorasInicio = 0;
                            parametersTseInitialEntityaux.HorasFin = 0;
                            parametersTseInitialEntityaux.EmployeeCode = registrox.NumeroEmpleado;
                            parametersTseInitialEntityaux.IdCarga = new Guid(model.IdCarga);
                            parametersTseInitialEntityaux.HoraInicio = "0";
                            parametersTseInitialEntityaux.HoraFin = "0";

                            parametersTseInitialEntityaux.OutIme = "N";
                            parametersTseInitialEntityaux.Semana = 0;
                            parametersTseInitialEntityaux.HoraInicioHoraio = "0";
                            parametersTseInitialEntityaux.HoraFinHorario = "0";
                            parametersTseInitialEntityaux.OverTime = "N";
                            parametersTseInitialEntityaux.Anio = semanahorario.Year.ToString();
                            parametersTseInitialEntityaux.Festivo = "N";
                            parametersTseInitialEntityaux.Estado = "";
                            parametersTseInitialEntityaux.Reporte = registrox.WorkOrder;

                            listParametersInitialEntity.Add(parametersTseInitialEntityaux);
                            continue;

                            
                        }



                        var tse = fHomologaTSE(registrox, horariosGMT, paisRegistro!);
                        if (tse == null)
                        {
                            ParametersTseInitialEntity parametersTseInitialEntityaux = new ParametersTseInitialEntity();

                            parametersTseInitialEntityaux.IdParamTSEInitialId = Guid.NewGuid();
                            parametersTseInitialEntityaux.EstatusProceso = "NO_APLICA_X_FALTA_DATOS_INICIO_FIN";
                            parametersTseInitialEntityaux.FECHA_REP = registrox.StartTime;
                            parametersTseInitialEntityaux.TOTAL_MINUTOS = getMins(registrox.DurationInHours);
                            parametersTseInitialEntityaux.totalHoras = registrox.DurationInHours;
                            parametersTseInitialEntityaux.HorasInicio = 0;
                            parametersTseInitialEntityaux.HorasFin = 0;
                            parametersTseInitialEntityaux.EmployeeCode = registrox.NumeroEmpleado;
                            parametersTseInitialEntityaux.IdCarga = new Guid(model.IdCarga);
                            parametersTseInitialEntityaux.HoraInicio = "0";
                            parametersTseInitialEntityaux.HoraFin = "0";

                            parametersTseInitialEntityaux.OutIme = "N";
                            parametersTseInitialEntityaux.Semana = 0;
                            parametersTseInitialEntityaux.HoraInicioHoraio = "0";
                            parametersTseInitialEntityaux.HoraFinHorario = "0";
                            parametersTseInitialEntityaux.OverTime = "N";
                            parametersTseInitialEntityaux.Anio = semanahorario.Year.ToString();
                            parametersTseInitialEntityaux.Festivo = "N";
                            parametersTseInitialEntityaux.Estado ="E204 NO TIENE HORARIO ASIGNADO";
                            parametersTseInitialEntityaux.Reporte = registrox.WorkOrder;

                            listParametersInitialEntity.Add(parametersTseInitialEntityaux);
                            continue;
                           
                        }
                    
                   // var paisRegistro = listaCountries.FirstOrDefault(e=>e.CodigoPais== registro.NumeroEmpleado.Substring(registro.NumeroEmpleado.Length - 3));

                   // var tse = validaHoraTSEGMT(registro, horariosGMT, paisRegistro);

                        semanahorario = DateTimeOffset.ParseExact(tse.StartTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                       

                        int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                    bool bValidacionHorario = false;

                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString());// && x.FechaWorking== semanahorario.DateTime);
                    //var horario = _dataBaseService.workinghoursEntity.FirstOrDefault(x => x.UserEntity.EmployeeCode == tse.NumeroEmpleado && x.week == Semana.ToString() && x.FechaWorking == semanahorario);
                    var esfestivo = listFestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario && x.CountryId== paisRegistro!.IdCounty);

                    

                   


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
                    parametersTseInitialEntity.HoraInicio = tse.StartHours;// DateTimeOffset.ParseExact(tse.StartTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay.ToString();
                                                                              //DateTimeOffset.Parse(tse.StartTime).TimeOfDay.ToString();
                    parametersTseInitialEntity.HoraFin = tse.EndHours;// DateTimeOffset.ParseExact(tse.EndTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay.ToString();
                        //DateTimeOffset.Parse(tse.EndTime).TimeOfDay.ToString();
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
                    parametersTseInitialEntity.Reporte = tse.WorkOrder;



                        var previosAndPos = new List<double>();
                    if (horario != null)
                    {
                        previosAndPos = getPreviasAndPosHorario(tse.StartHours, tse.EndHours, horario.HoraInicio, horario.HoraFin);
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
                        parametersTseInitialEntity.Reporte = tse.WorkOrder;

                        }
                    else
                    {
                        //NO hay horario
                        parametersTseInitialEntity.HoraInicioHoraio = "0";
                        parametersTseInitialEntity.HoraFinHorario = "0";
                        parametersTseInitialEntity.EstatusProceso = "NO_APLICA_X_HORARIO";
                        parametersTseInitialEntity.Reporte = tse.WorkOrder;
                        listParametersInitialEntity.Add(parametersTseInitialEntity);
                        
                        continue;
                    }
                   

                  



                    listParametersInitialEntity.Add(parametersTseInitialEntity);

                }

                    


                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                //await _dataBaseService.ParametersTseInitialEntity.AddRangeAsync(listParametersInitialEntity);
                //await _dataBaseService.SaveAsync();


                int PAGE_SIZE = listParametersInitialEntity.Count() / 10;
                List<List<ParametersTseInitialEntity>> partitions = listParametersInitialEntity.partition(PAGE_SIZE);

                for (int i = 0; i < partitions.Count(); i++)
                {
                    var endpoint = i % 2 == 0 ? "1" : "2";
                    try
                    {
                        LoadGenericTse(partitions[i], endpoint);
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;
                    }

                }




                return model.IdCarga.ToString();

            }
            catch (Exception ex)
            {
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

                if (convert.FECHA_REP == null)
                {
                    return null;

                }
                

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
                            //11/20/1981 12:00:00 AM
                        }
                        //
                    }
                    catch (Exception ex)
                    {

                    }

                    try
                    {

                        if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            //convert.FECHA_REP = dt.ToString("dd/MM/yyyy HH:mm:ss");
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "M/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "MM/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.FECHA_REP, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.FECHA_REP = dt.ToString("dd/MM/yyyy 00:00:00");
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


        private TSELoadEntity fHomologaTSE(TSELoadEntity item, List<PaisRelacionGMTEntity> horariosGMT, CountryModel paisRegistro)
        {
            try
            {
                var convert = item;
                DateTimeOffset dateTimeOffset = new DateTimeOffset();

                
                if (convert.EndTime == null)
                {
                    return null;

                }
                if (convert.StartTime == null)
                {
                    return null;
                }
            
                


                try
                {
                    var paisComparacion = horariosGMT.FirstOrDefault(e => e.NameCountryCompare == paisRegistro.NameCountry);

                    var dt = new DateTimeOffset();
                    if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {

                       dt= dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }



                    if(DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }







                    try
                    {
                       

                        if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                          //  convert.StartTime = dt.ToString("dd/MM/yyyy HH:mm:ss");
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }

                    }
                    catch (Exception exx)
                    {
                       

                    }



                    try
                    {


                        if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndTime = dt.ToString("dd/MM/yyyy 00:00:00");
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

        private STELoadEntity fHomologaSTE(STELoadEntity item, List<PaisRelacionGMTEntity> horariosGMT, CountryModel paisRegistro)
        {
            try
            {
                var convert = item;


                DateTimeOffset dateTimeOffset = new DateTimeOffset();


                if (convert.EndDateTime == null)
                {
                    return null;

                }
                if (convert.StartDateTime == null)
                {
                    return null;
                }




                try
                {
                    var paisComparacion = horariosGMT.FirstOrDefault(e => e.NameCountryCompare == paisRegistro.NameCountry);
                    var dt = new DateTimeOffset();
                    if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.StartHours = dt.ToString("HH:mm");
                    }



                    if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }
                    else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dt = dt.AddHours(paisComparacion!.TimeDifference);
                        convert.EndHours = dt.ToString("HH:mm");
                    }







                    try
                    {


                        if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                           // convert.StartDateTime = dt.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");

                        }
                        else if (DateTimeOffset.TryParseExact(convert.StartDateTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.StartDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }

                    }
                    catch (Exception exx)
                    {


                    }



                    try
                    {


                        if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                            //convert.EndDateTime = dt.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yyyy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/MM/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "d/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "dd/M/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
                        }
                        else if (DateTimeOffset.TryParseExact(convert.EndDateTime, "M/d/yy h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            convert.EndDateTime = dt.ToString("dd/MM/yyyy 00:00:00");
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




        public async Task<SummaryLoad> LoadSTE(LoadJsonPais model)
        {
         

         

            try
            {
                List<STELoadEntity> datosSTEExcelFull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STELoadEntity>>(model.Data.ToJsonString());


                List<STELoadEntity> datosSTEExcel = datosSTEExcelFull!.Where(x => x.TotalDuration!="0").ToList();
                datosSTEExcel.Where(e => string.IsNullOrEmpty(e.AccountCMRNumber) == true).ToList().ForEach(x => x.AccountCMRNumber = "1234");


                //Setting number of register on STE
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().
                                              ForEach(x => x.STECarga = datosSTEExcel.Count().ToString());
                await _dataBaseService.SaveAsync();


                //Remueve duplicados 
                datosSTEExcel = datosSTEExcel.DistinctBy(m => new { m.SessionEmployeeSerialNumber, m.FechaRegistro, m.StartDateTime, m.EndDateTime }).ToList();

                //Setting duplicados metrica en STE
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEOmitidosXDuplicidad = (datosSTEExcelFull.Count() - datosSTEExcel.Count()).ToString());
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

                //Remueve duplicados 
                datosSTEExcel = datosSTEExcel.DistinctBy(m => new { m.SessionEmployeeSerialNumber, m.StartDateTime, m.EndDateTime }).ToList();



                foreach (var registrox in datosSTEExcel)
                {
                    var paisRegistro = listaCountries.FirstOrDefault(e => e.CodigoPais == registrox.SessionEmployeeSerialNumber.Substring(registrox.SessionEmployeeSerialNumber.Length - 3));

                    //valida pais
                    if (paisRegistro == null)
                    {
                        //PAIS no valido
                        ParametersSteInitialEntity parametersTseInitialEntityaux = new ParametersSteInitialEntity();

                        parametersTseInitialEntityaux.IdParamSTEInitialId = Guid.NewGuid();
                        parametersTseInitialEntityaux.EstatusProceso = "NO_APLICA_X_PAIS";
                        parametersTseInitialEntityaux.FECHA_REP = registrox.StartDateTime;
                        parametersTseInitialEntityaux.TOTAL_MINUTOS = getMins(registrox.TotalDuration);
                        parametersTseInitialEntityaux.totalHoras = registrox.TotalDuration;
                        parametersTseInitialEntityaux.HorasInicio = 0;
                        parametersTseInitialEntityaux.HorasFin = 0;
                        parametersTseInitialEntityaux.EmployeeCode = registrox.SessionEmployeeSerialNumber;
                        parametersTseInitialEntityaux.IdCarga = new Guid(model.IdCarga);
                        parametersTseInitialEntityaux.HoraInicio = "0";
                        parametersTseInitialEntityaux.HoraFin = "0";

                        parametersTseInitialEntityaux.OutIme = "N";
                        parametersTseInitialEntityaux.Semana = 0;
                        parametersTseInitialEntityaux.HoraInicioHoraio = "0";
                        parametersTseInitialEntityaux.HoraFinHorario = "0";
                        parametersTseInitialEntityaux.OverTime = "N";
                        parametersTseInitialEntityaux.Anio = semanahorario.Year.ToString();
                        parametersTseInitialEntityaux.Festivo = "N";
                        parametersTseInitialEntityaux.Estado = "";
                        parametersTseInitialEntityaux.Reporte = registrox.NumeroCaso;

                        listParametersInitialEntity.Add(parametersTseInitialEntityaux);
                        continue;
                    }




                        var ste = fHomologaSTE(registrox, horariosGMT, paisRegistro!);

                    if (ste == null)
                    {
                        ParametersSteInitialEntity parametersTseInitialEntityaux = new ParametersSteInitialEntity();

                        parametersTseInitialEntityaux.IdParamSTEInitialId = Guid.NewGuid();
                        parametersTseInitialEntityaux.EstatusProceso = "NO_APLICA_X_FALTA_DATOS_INICIO_FIN";
                        parametersTseInitialEntityaux.FECHA_REP = registrox.StartDateTime;
                        parametersTseInitialEntityaux.TOTAL_MINUTOS = getMins(registrox.TotalDuration);
                        parametersTseInitialEntityaux.totalHoras = registrox.TotalDuration;
                        parametersTseInitialEntityaux.HorasInicio = 0;
                        parametersTseInitialEntityaux.HorasFin = 0;
                        parametersTseInitialEntityaux.EmployeeCode = registrox.SessionEmployeeSerialNumber;
                        parametersTseInitialEntityaux.IdCarga = new Guid(model.IdCarga);
                        parametersTseInitialEntityaux.HoraInicio = "0";
                        parametersTseInitialEntityaux.HoraFin = "0";

                        parametersTseInitialEntityaux.OutIme = "N";
                        parametersTseInitialEntityaux.Semana = 0;
                        parametersTseInitialEntityaux.HoraInicioHoraio = "0";
                        parametersTseInitialEntityaux.HoraFinHorario = "0";
                        parametersTseInitialEntityaux.OverTime = "N";
                        parametersTseInitialEntityaux.Anio = semanahorario.Year.ToString();
                        parametersTseInitialEntityaux.Festivo = "N";
                        parametersTseInitialEntityaux.Estado = "E204 NO TIENE HORARIO ASIGNADO";
                        parametersTseInitialEntityaux.Reporte = registrox.NumeroCaso;

                        listParametersInitialEntity.Add(parametersTseInitialEntityaux);
                        continue;

                    }


                     //semanahorario = DateTimeOffset.Parse(ste.StartDateTime);
                   semanahorario = DateTimeOffset.ParseExact(ste.StartDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                   
                    
                    int Semana = cul.Calendar.GetWeekOfYear(semanahorario.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                    bool bValidacionHorario = false;
                    var horario = Lsthorario.FirstOrDefault(x => x.UserEntity.EmployeeCode == ste.SessionEmployeeSerialNumber && x.week == Semana.ToString());// && x.FechaWorking== semanahorario.DateTime);
                    
                    var esfestivo = listFestivos.FirstOrDefault(x => x.DiaFestivo == semanahorario && x.CountryId == paisRegistro!.IdCounty);

                    

                  

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
                    parametersSTEInitialEntity.HoraInicio = ste.StartHours;// DateTimeOffset.ParseExact(ste.StartDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay.ToString();//DateTimeOffset.Parse(ste.StartDateTime).TimeOfDay.ToString();
                    parametersSTEInitialEntity.HoraFin = ste.EndHours;// DateTimeOffset.ParseExact(ste.EndDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay.ToString();//DateTimeOffset.Parse(ste.EndDateTime).TimeOfDay.ToString();
                    parametersSTEInitialEntity.OverTime = "N";
                    parametersSTEInitialEntity.OutIme = "N";
                    parametersSTEInitialEntity.Semana = Semana;
                    parametersSTEInitialEntity.HoraInicioHoraio = "0";
                    parametersSTEInitialEntity.HoraFinHorario = "0";
                    parametersSTEInitialEntity.IdCarga = new Guid(model.IdCarga);
                    parametersSTEInitialEntity.Reporte = ste.NumeroCaso;
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
                        parametersSTEInitialEntity.Reporte = ste.NumeroCaso;

                    }
                    else
                    {
                        //NO hay horario
                       
                        parametersSTEInitialEntity.HoraInicioHoraio = "0";
                        parametersSTEInitialEntity.HoraFinHorario = "0";

                        parametersSTEInitialEntity.EstatusProceso = "NO_APLICA_X_HORARIO";
                        parametersSTEInitialEntity.Reporte = ste.NumeroCaso;
                        listParametersInitialEntity.Add(parametersSTEInitialEntity);
                        continue;

                    }

                   

                  
                    listParametersInitialEntity.Add(parametersSTEInitialEntity);
                }

                //_dataBaseService.ParametersSteInitialEntity.AddRangeAsync(listParametersInitialEntity);
                //await _dataBaseService.SaveAsync();

                int PAGE_SIZE = listParametersInitialEntity.Count() / 10;
                List<List<ParametersSteInitialEntity>> partitions = listParametersInitialEntity.partition(PAGE_SIZE);

                for (int i = 0; i < partitions.Count(); i++)
                {
                    var endpoint = i % 2 == 0 ? "1" : "2";
                    try
                    {
                        LoadGenericSte(partitions[i], endpoint);
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;
                    }

                }














                //PROCESO DE VALIDACION OVER-LAPING!!!!
                var rowARPGral = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();
                var rowSTEGral = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();
                var rowTSEGral = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList();

                var RowGralARPTSE = rowARPGral.ToList().IntersectBy(
                    rowTSEGral.ToList().Select(r => new { EmployeeCode = r.EmployeeCode, FECHA_REP = r.FECHA_REP, HoraInicio = r.HoraInicio, HoraFin = r.HoraFin, Semana = r.Semana }), t => new { EmployeeCode = t.EmployeeCode, FECHA_REP = t.FECHA_REP, HoraInicio = t.HoraInicio, HoraFin = t.HoraFin, Semana = t.Semana }).ToList();

                var RowGralARPSTE = rowARPGral.ToList().IntersectBy(
                    rowSTEGral.ToList().Select(o => new { EmployeeCode = o.EmployeeCode, FECHA_REP = o.FECHA_REP, HoraInicio = o.HoraInicio, HoraFin = o.HoraFin, Semana = o.Semana }),
                    x => new { EmployeeCode = x.EmployeeCode, FECHA_REP = x.FECHA_REP, HoraInicio = x.HoraInicio, HoraFin = x.HoraFin, Semana = x.Semana }).ToList();

                var RowGralTSESTE = rowTSEGral.ToList().IntersectBy(
                    rowSTEGral.ToList().Select(o => new { EmployeeCode = o.EmployeeCode, FECHA_REP = o.FECHA_REP, HoraInicio = o.HoraInicio, HoraFin = o.HoraFin, Semana = o.Semana }),
                    x => new { EmployeeCode = x.EmployeeCode, FECHA_REP = x.FECHA_REP, HoraInicio = x.HoraInicio, HoraFin = x.HoraFin, Semana = x.Semana }).ToList();

                var RowGralTSEARP = rowTSEGral.ToList().IntersectBy(
                    rowARPGral.ToList().Select(r => new { EmployeeCode = r.EmployeeCode, FECHA_REP = r.FECHA_REP, HoraInicio = r.HoraInicio, HoraFin = r.HoraFin, Semana = r.Semana }), t => new { EmployeeCode = t.EmployeeCode, FECHA_REP = t.FECHA_REP, HoraInicio = t.HoraInicio, HoraFin = t.HoraFin, Semana = t.Semana }).ToList();

                var RowGralSTEARP = rowSTEGral.ToList().IntersectBy(
                    rowARPGral.ToList().Select(o => new { EmployeeCode = o.EmployeeCode, FECHA_REP = o.FECHA_REP, HoraInicio = o.HoraInicio, HoraFin = o.HoraFin, Semana = o.Semana }),
                    x => new { EmployeeCode = x.EmployeeCode, FECHA_REP = x.FECHA_REP, HoraInicio = x.HoraInicio, HoraFin = x.HoraFin, Semana = x.Semana }).ToList();

                var RowGralSTETSE = rowSTEGral.ToList().IntersectBy(
                    rowTSEGral.ToList().Select(o => new { EmployeeCode = o.EmployeeCode, FECHA_REP = o.FECHA_REP, HoraInicio = o.HoraInicio, HoraFin = o.HoraFin, Semana = o.Semana }),
                    x => new { EmployeeCode = x.EmployeeCode, FECHA_REP = x.FECHA_REP, HoraInicio = x.HoraInicio, HoraFin = x.HoraFin, Semana = x.Semana }).ToList();

                var RowGralARPTSESTE = rowARPGral.ToList().IntersectBy(
                    rowSTEGral.ToList().Select(o => new { EmployeeCode = o.EmployeeCode, FECHA_REP = o.FECHA_REP, HoraInicio = o.HoraInicio, HoraFin = o.HoraFin, Semana = o.Semana }),
                    x => new { EmployeeCode = x.EmployeeCode, FECHA_REP = x.FECHA_REP, HoraInicio = x.HoraInicio, HoraFin = x.HoraFin, Semana = x.Semana }).IntersectBy(rowTSEGral.ToList().Select(r => new { EmployeeCode = r.EmployeeCode, FECHA_REP = r.FECHA_REP, HoraInicio = r.HoraInicio, HoraFin = r.HoraFin, Semana = r.Semana }), t => new { EmployeeCode = t.EmployeeCode, FECHA_REP = t.FECHA_REP, HoraInicio = t.HoraInicio, HoraFin = t.HoraFin, Semana = t.Semana }).ToList();

                RowGralARPTSE.ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING");
                RowGralTSEARP.ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING");
                RowGralARPSTE.ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING");
                RowGralSTEARP.ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING");
                RowGralTSESTE.ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING");
                RowGralSTETSE.ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING");
                RowGralARPTSESTE.ToList().ForEach(x => x.EstatusProceso = "NO_APLICA_X_OVERLAPING");
                RowGralARPTSE.ToList().ForEach(x => x.EstatusProceso = "EN_OVERTIME");

                await _dataBaseService.SaveAsync();



























                //----------------------------------------------------------------------------------------------------------------------------------------










                //getting metrics
                int arpNoAplicaXHorario = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_HORARIO" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXOverttime = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXOverLaping = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpEnProceso = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXPais = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_PAIS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int arpNoAplicaXFaltaDatoshoras = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_FALTA_DATOS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();

                
                var cargaRegistro = _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).FirstOrDefault();
                

                int tseNoAplicaXHorario = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_HORARIO" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXOverttime = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXOverLaping = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseEnProceso = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "EN_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXPais = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_PAIS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int tseNoAplicaXFaltaDatoshoras = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_FALTA_DATOS" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();

                int steNoAplicaXHorario = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_HORARIO" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXOverttime = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERTIME" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
                int steNoAplicaXOverLaping = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(model.IdCarga)).ToList().Count();
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
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.TSEXDatosNovalidos = (tseNoAplicaXPais+ tseNoAplicaXFaltaDatoshoras).ToString());

                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXHorario = steNoAplicaXHorario.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEEXOvertime = steNoAplicaXOverttime.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXOverlaping = steNoAplicaXOverLaping.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXProceso = steEnProceso.ToString());
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(model.IdCarga)).ToList().ForEach(x => x.STEXDatosNovalidos = (steNoAplicaXPais+steNoAplicaXFaltaDatoshoras ).ToString());

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
               // summary.ARP_OMITIDOS = cargaRegistro.ARPOmitidos;

                summary.NO_APLICA_X_HORARIO_TSE = tseNoAplicaXHorario.ToString();
                summary.NO_APLICA_X_OVERTIME_TSE = tseNoAplicaXOverttime.ToString();
                summary.NO_APLICA_X_OVERLAPING_TSE = tseNoAplicaXOverLaping.ToString();
                summary.EN_PROCESO_TSE = tseEnProceso.ToString();
               // summary.TSE_OMITIDOS = cargaRegistro.TSEOmitidos;

                summary.NO_APLICA_X_HORARIO_STE =steNoAplicaXHorario.ToString();
                summary.NO_APLICA_X_OVERTIME_STE = steNoAplicaXOverttime.ToString();
                summary.NO_APLICA_X_OVERLAPING_STE = steNoAplicaXOverLaping.ToString();
                summary.EN_PROCESO_STE =steEnProceso.ToString();
                summary.IdCarga = model.IdCarga;
                //summary.STE_OMITIDOS = steOmitidos;

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
               // summary.ARP_OMITIDOS = "0";

                summary.NO_APLICA_X_HORARIO_TSE = "0";
                summary.NO_APLICA_X_OVERTIME_TSE = "0";
                summary.NO_APLICA_X_OVERLAPING_TSE = "0";
                summary.EN_PROCESO_TSE = "0";
               // summary.TSE_OMITIDOS = "0";

                summary.NO_APLICA_X_HORARIO_STE = "0";
                summary.NO_APLICA_X_OVERTIME_STE = "0";
                summary.NO_APLICA_X_OVERLAPING_STE = "0";
                summary.EN_PROCESO_STE = "0";
               // summary.STE_OMITIDOS = "0";
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
                    
                    ////_emailCommand.SendEmail(new EmailModel { To = "pruebasportaltls@gmail.com", Plantilla = "11" });
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
                    //_emailCommand.SendEmail(new EmailModel { To = "pruebasportaltls@gmail.com", Plantilla = "11" });
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
                    //_emailCommand.SendEmail(new EmailModel { To = "pruebasportaltls@gmail.com", Plantilla = "11" });
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
                //aqui
                var rowARPParameter = _dataBaseService.ParametersArpInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowTSEParameter = _dataBaseService.ParametersTseInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowSTEParameter = _dataBaseService.ParametersSteInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();

                var limitesCountryARP = _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == Guid.Parse("908465f1-4848-4c86-9e30-471982c01a2d"));
                var HorasLimiteDia = limitesCountryARP.TargetTimeDay;

                var listaCountries =_dataBaseService.CountryEntity.ToList();
                var listExeptios = _dataBaseService.UsersExceptions.ToList();
                var listHorusReport = _dataBaseService.HorusReportEntity.ToList();
                var UserLst = _dataBaseService.UserEntity.ToList();

                var CoEmple = "";
                var HoursTotEMp = 0.0;

                //format dates horusReport
                //
               /* foreach (var horus in listHorusReport)
                {
                    horus= validaFormatosFechaHorusreport(horus);
                }*/
               // listHorusReport.ToList().ForEach(b => b.StartDate = validaFormatosFechaHorusreport(b.StartDate));

                foreach (var itemARP in rowARPParameter)
                {

                   

                    TimeSpan tsReportado = DateTimeOffset.Parse(itemARP.HoraFin.ToString()).TimeOfDay - DateTimeOffset.Parse(itemARP.HoraInicio).TimeOfDay;
                    var UserRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemARP.EmployeeCode);
                    if (UserRow != null)
                    {
                        var exceptionUser = listExeptios.FirstOrDefault(x => x.UserId == UserRow.IdUser && x.StartDate.ToString("MM/dd/yyyy") == DateTimeOffset.ParseExact(itemARP.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"));
                        var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
                        
                      //var HorasARP = listHorusReport.Where(co => DateTimeOffset.ParseExact(co.StrStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy 00:00:00") == DateTimeOffset.ParseExact(itemARP.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy 00:00:00") && co.UserEntityId== UserRow.IdUser).ToList();
                        var HorasARP = listHorusReport.Where(co => co.StrStartDate == DateTimeOffset.ParseExact(itemARP.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00") && co.UserEntityId == UserRow.IdUser).ToList();

                        var HorasARPGral = HorasARP.Select(x => double.Parse(x.CountHours)).Sum();

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
                    var limitesCountryTSE =  _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == paisRegistro.IdCounty);
                    var HorasLimiteDiaTSE = limitesCountryTSE.TargetTimeDay;
                    TimeSpan tsReportadoTSE = DateTimeOffset.Parse(itemTSE.HoraFin.ToString()).TimeOfDay - DateTimeOffset.Parse(itemTSE.HoraInicio).TimeOfDay;
                    var UserRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemTSE.EmployeeCode);
                    if (UserRow != null)
                    {
                        
                        var exceptionUser = listExeptios.FirstOrDefault(x => x.UserId == UserRow.IdUser && x.StartDate.ToString("MM/dd/yyyy") == DateTimeOffset.ParseExact(itemTSE.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"));
                        var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
                        
                        //ok
                        var HorasTSE = listHorusReport.Where(co => co.StrStartDate == DateTimeOffset.ParseExact(itemTSE.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00") && co.UserEntityId == UserRow.IdUser).ToList();

                        var HorasTSEGral = HorasTSE.Select(x => double.Parse(x.CountHours)).Sum();

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
                    var limitesCountrySTE =  _dataBaseService.ParametersEntity.FirstOrDefault(x => x.CountryEntityId == paisRegistro.IdCounty);
                    var HorasLimiteDiaSTE = limitesCountrySTE.TargetTimeDay;
                    TimeSpan tsReportadoSTE = DateTimeOffset.Parse(itemSTE.HoraFin.ToString()).TimeOfDay - DateTimeOffset.Parse(itemSTE.HoraInicio).TimeOfDay;
                    var UserRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemSTE.EmployeeCode);
                    if (UserRow != null)
                    {
                        var exceptionUser = listExeptios.FirstOrDefault(x => x.UserId == UserRow.IdUser && x.StartDate.ToString("MM/dd/yyyy") == DateTimeOffset.ParseExact(itemSTE.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"));
                        var horasExceptuada = exceptionUser == null ? 0 : exceptionUser.horas;
                        
                        //var HorasSTE = listHorusReport.Where(co => co.StrStartDate == DateTimeOffset.ParseExact(itemSTE.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy 00:00:00") && co.UserEntityId == UserRow.IdUser).ToList();
                        var HorasSTE = listHorusReport.Where(co => co.StrStartDate == DateTimeOffset.ParseExact(itemSTE.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00") && co.UserEntityId == UserRow.IdUser).ToList();
                        var HorasSTEGral = HorasSTE.Select(x => double.Parse(x.CountHours)).Sum();
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

                _dataBaseService.SaveAsync();

                //los sobrantes estan en overtime, insertar en portaldb y su history, como se hace en reporte de horas stand by
                //TODO

                var rowARPParameterFinal =  _dataBaseService.ParametersArpInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowTSEParameterFinal =  _dataBaseService.ParametersTseInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowSTEParameterFinal =  _dataBaseService.ParametersSteInitialEntity.Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();

                var datax =  _dataBaseService.HorusReportEntity.ToList();
                //aqui
                foreach (var itemARPp in rowARPParameterFinal)
                {
                    //------------------------------------------------------------------------------------------------------------------
                    var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemARPp.EmployeeCode);

                  

                    var startTime = DateTime.Parse(itemARPp.HoraInicio);
                    var endTime = DateTime.Parse(itemARPp.HoraFin);

                    DateTime fechaHoraOriginal = DateTime.ParseExact(itemARPp.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");

                   
                    var data = _dataBaseService.HorusReportEntity
                        .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == userRow.IdUser)
                        .AsEnumerable()
                        .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemARPp.HoraInicio, itemARPp.HoraFin) ||
                        (TimeInRange(h.StartTime, startTime, endTime) &&
                        TimeInRange(h.EndTime, startTime, endTime)))
                        .ToList();




                    if (data.Count > 0)
                    {
                        itemARPp.EstatusProceso = "NO_APLICA_X_OVERLAPING";

                    }
                    //------------------------------------------------------------------------------------------------------------------
                }

                foreach (var itemTSEp in rowTSEParameterFinal)
                {
                    var UserRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemTSEp.EmployeeCode);

                   

                    var startTime = DateTime.Parse(itemTSEp.HoraInicio);
                    var endTime = DateTime.Parse(itemTSEp.HoraFin);

                    DateTime fechaHoraOriginal = DateTime.ParseExact(itemTSEp.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");

                    var data = _dataBaseService.HorusReportEntity
                        .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == UserRow.IdUser)
                        .AsEnumerable()
                        .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemTSEp.HoraInicio, itemTSEp.HoraFin) ||
                        (TimeInRange(h.StartTime, startTime, endTime) &&
                        TimeInRange(h.EndTime, startTime, endTime)))
                        .ToList();





                    if (data.Count > 0)
                    {
                        itemTSEp.EstatusProceso = "NO_APLICA_X_OVERLAPING";

                    }
                }

                foreach (var itemSTEp in rowSTEParameterFinal)
                {
                    var UserRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemSTEp.EmployeeCode);

                    var startTime = DateTime.Parse(itemSTEp.HoraInicio);
                    var endTime = DateTime.Parse(itemSTEp.HoraFin);

                    DateTime fechaHoraOriginal = DateTime.ParseExact(itemSTEp.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");

                    var data = _dataBaseService.HorusReportEntity
                        .Where(h => h.StrStartDate == nuevaFechaHoraFormato && h.UserEntityId == UserRow.IdUser)
                        .AsEnumerable()
                        .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, itemSTEp.HoraInicio, itemSTEp.HoraFin) ||
                        (TimeInRange(h.StartTime, startTime, endTime) &&
                        TimeInRange(h.EndTime, startTime, endTime)))
                        .ToList();

                    if (data.Count > 0)
                    {
                        itemSTEp.EstatusProceso = "NO_APLICA_X_OVERLAPING";
                    }
                }


                await _dataBaseService.SaveAsync();

                var rowARPParameterGral =  _dataBaseService.ParametersArpInitialEntity.AsNoTracking().Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowTSEParameterGral =  _dataBaseService.ParametersTseInitialEntity.AsNoTracking().Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                var rowSTEParameterGral =  _dataBaseService.ParametersSteInitialEntity.AsNoTracking().Where(op => op.IdCarga == Guid.Parse(idCarga) && op.EstatusProceso == "EN_OVERTIME").ToList();
                int arpNoAplicaXOverLaping = _dataBaseService.ParametersArpInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(idCarga)).ToList().Count();
                int tseNoAplicaXOverLaping = _dataBaseService.ParametersTseInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(idCarga)).ToList().Count();
                int steNoAplicaXOverLaping = _dataBaseService.ParametersSteInitialEntity.Where(e => e.EstatusProceso == "NO_APLICA_X_OVERLAPING" && e.IdCarga == new Guid(idCarga)).ToList().Count();

                summary.Mensaje = "Carga procesada";
                summary.REGISTROS_PORTALDB = (rowARPParameterGral.Count() + rowSTEParameterGral.Count()+ rowTSEParameterGral.Count()).ToString();
                summary.NO_APLICA_X_OVERLAPING_ARP = arpNoAplicaXOverLaping.ToString();
                summary.NO_APLICA_X_OVERLAPING_TSE = tseNoAplicaXOverLaping.ToString();
                summary.NO_APLICA_X_OVERLAPING_STE = steNoAplicaXOverLaping.ToString();

                List<HorusReportEntity> rowsHorusNew = new();
                HorusReportEntity rowAdd = new();
                List<Domain.Entities.AssignmentReport.AssignmentReport> rowAssignments = new();
                Domain.Entities.AssignmentReport.AssignmentReport rowAddAssig = new();

                //ARP
                //--------------------------------------------------------------------------
                var Maxen = 0;
                try
                {
                    Maxen =  _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
                }
                catch (Exception)
                {
                    Maxen = 0;
                }
                
                foreach (var itemARPNew in rowARPParameterGral)
                {
                    Maxen++;
                    DateTime fechaHoraOriginal = DateTime.ParseExact(itemARPNew.FECHA_REP, "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);
                   // string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("yyyy-MM-dd 00:00:00");
                    string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("dd/MM/yyyy 00:00:00");
                    var userRow = UserLst.FirstOrDefault(op => op.EmployeeCode == itemARPNew.EmployeeCode);

                    rowAdd = new()
                    {
                        IdHorusReport = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        DateApprovalSystem = DateTime.Now,
                        NumberReport = Maxen,
                        StartDate = fechaHoraOriginal,// DateTimeOffset.Parse(nuevaFechaHoraFormato).Date,
                        StrStartDate = nuevaFechaHoraFormato,//itemARPNew.FECHA_REP,
                        StartTime = itemARPNew.HoraInicio,
                        EndTime = itemARPNew.HoraFin,
                        Description = itemARPNew.EstatusProceso + " Generado por proceso overtime",
                        UserEntityId = userRow.IdUser,
                        ClientEntityId = Guid.Parse("00000000-0000-0000-0000-000000000000"),//   dc606c5a-149e-4f9b-80b3-ba555c7689b9"),
                        TipoReporte = 2,
                        Acitivity = 0,
                        CountHours = itemARPNew.totalHoras,
                        ApproverId = userRow.IdUser.ToString(),
                        ARPLoadingId = idCarga
                    };
                    rowsHorusNew.Add(rowAdd);

                    //var UserRow = _dataBaseService.UserEntity.FirstOrDefault(op => op.EmployeeCode == itemARPNew.EmployeeCode);

                    rowAddAssig = new()
                    {
                        IdAssignmentReport = Guid.NewGuid(),
                        HorusReportEntityId = rowAdd.IdHorusReport,
                        UserEntityId = userRow.IdUser,
                        Description = "PROCESO_OVERTIME",
                        State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente

                    };
                    rowAssignments.Add(rowAddAssig);
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

                    rowAdd = new()
                    {
                        IdHorusReport = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        DateApprovalSystem = DateTime.Now,
                        NumberReport = Maxen,
                        StartDate = fechaHoraOriginal,//DateTimeOffset.Parse(nuevaFechaHoraFormato).Date,
                        StrStartDate = nuevaFechaHoraFormato,
                        StartTime = itemTSENew.HoraInicio,
                        EndTime = itemTSENew.HoraFin,
                        Description = itemTSENew.EstatusProceso + " Generado por proceso overtime",
                        UserEntityId = userRow.IdUser,
                        ClientEntityId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                        TipoReporte = 2,
                        Acitivity = 0,
                        CountHours = itemTSENew.totalHoras,
                        ApproverId = userRow.IdUser.ToString(),
                        ARPLoadingId = idCarga

                    };
                    rowsHorusNew.Add(rowAdd);



                    rowAddAssig = new()
                    {
                        IdAssignmentReport = Guid.NewGuid(),
                        HorusReportEntityId = rowAdd.IdHorusReport,
                        UserEntityId = userRow.IdUser,
                        Description = "PROCESO_OVERTIME",
                        State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente

                    };
                    rowAssignments.Add(rowAddAssig);
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

                    rowAdd = new()
                    {
                        IdHorusReport = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        DateApprovalSystem = DateTime.Now,
                        NumberReport = Maxen,
                        StartDate = fechaHoraOriginal,//DateTimeOffset.Parse(nuevaFechaHoraFormato).Date,
                        StrStartDate = nuevaFechaHoraFormato,
                        StartTime = itemSTENew.HoraInicio,
                        EndTime = itemSTENew.HoraFin,
                        Description = itemSTENew.EstatusProceso + " Generado por proceso overtime",
                        UserEntityId = userRow.IdUser,
                        ClientEntityId = Guid.Parse("00000000-0000-0000-0000-000000000000"), // Guid.Parse("dc606c5a-149e-4f9b-80b3-ba555c7689b9"),
                        TipoReporte = 2,
                        Acitivity = 0,
                        CountHours = itemSTENew.totalHoras,
                        ApproverId = userRow.IdUser.ToString(),
                        ARPLoadingId = idCarga
                    };
                    rowsHorusNew.Add(rowAdd);



                    rowAddAssig = new()
                    {
                        IdAssignmentReport = Guid.NewGuid(),
                        HorusReportEntityId = rowAdd.IdHorusReport,
                        UserEntityId = userRow.IdUser,
                        Description = "PROCESO_OVERTIME",
                        State = (byte)Enums.Enums.AprobacionPortalDB.Pendiente,
                        DateApprovalCancellation = DateTime.Now,

                    };
                    rowAssignments.Add(rowAddAssig);
                }


                _dataBaseService.HorusReportEntity.AddRange(rowsHorusNew);
               await _dataBaseService.SaveAsync();


                 _dataBaseService.assignmentReports.AddRangeAsync(rowAssignments);
                await _dataBaseService.SaveAsync();


                //finishing load process
                _dataBaseService.ARPLoadEntity.Where(e => e.IdArpLoad == new Guid(idCarga)).ToList().
                                              ForEach(x => x.Estado = 2);
                await _dataBaseService.SaveAsync();

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
    }
}
