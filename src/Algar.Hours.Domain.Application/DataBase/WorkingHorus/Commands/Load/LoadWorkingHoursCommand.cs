﻿using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using Algar.Hours.Domain.Entities.Load;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load
{
    public class LoadWorkingHoursCommand : ILoadWorkingHoursCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private IGetListUsuarioCommand _getUserId;
        private IConsultCountryCommand _getCountryId;
        private ICreateWorkingHoursCommand _createHorario;
        public LoadWorkingHoursCommand
            (IDataBaseService dataBaseService, IMapper mapper, IGetListUsuarioCommand getUserId, IConsultCountryCommand getCountryId, ICreateWorkingHoursCommand createHorario)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _getUserId = getUserId;
            _getCountryId = getCountryId;
            _createHorario = createHorario;
        }

        public async Task<bool> LoadExcel(JsonArray model)
        {
            List<CreateWorkingHoursModel> lista = new List<CreateWorkingHoursModel>();

            var count = 0;

            var datosInvalidos = false;
            foreach (var entity in model)
            {

                count += 1;

                var convert = Newtonsoft.Json.JsonConvert.DeserializeObject<LoadWorkingHoursModel>(entity.ToJsonString());
                var diaVacio = convert.dia == "" || convert.dia == null;
                var codigoEmpleadoVacio = convert.codigo_Empleado == "" || convert.codigo_Empleado == null;
                var paisVacio = convert.pais == "" || convert.pais == null;
                var horaInicioVacio = convert.HoraInicio == "" || convert.HoraInicio == null;
                var horaFinVacio = convert.HoraFin == "" || convert.HoraFin == null;
                var horarioVacio = diaVacio && codigoEmpleadoVacio && paisVacio && horaInicioVacio && horaFinVacio;
                if (horarioVacio) continue;

                if (diaVacio || codigoEmpleadoVacio || paisVacio || horaInicioVacio || horaFinVacio) 
                {
                    datosInvalidos = true;
                    break;
                }
                var modeloHorario = await CreateModeloHorario2(convert);
                lista.Add(modeloHorario);
            }

            if (datosInvalidos) {
                return false;
            }

            await _createHorario.Execute(lista);
            return true;
        }

        private async Task<CreateWorkingHoursModel> CreateModeloHorario(LoadWorkingHoursModel convert)
        {
            var modeloHorario = new CreateWorkingHoursModel();
            try
            {
                Guid idPais = await _getCountryId.ConsultIdbyName(convert.pais);
                //Guid idUser = await _getUserId.GetUserIdByEmployeeCode(convert.codigo_Empleado, idPais);
                Guid idUser = await _getUserId.GetUserIdByID(convert.codigo_Empleado, idPais);

                var fechasr = convert.fecha.Split("/");

                DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(convert.fecha, "M/d/yy", CultureInfo.InvariantCulture);
                Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                string weekOfYear = ((int) dateTimeOffset.DayOfWeek).ToString();
                string year = dateTimeOffset.Year.ToString();

                modeloHorario.UserEntityId = idUser;
                modeloHorario.week = weekOfYear;
                modeloHorario.HoraInicio = FormatTime(convert.HoraInicio);
                modeloHorario.HoraFin = FormatTime(convert.HoraFin);
                modeloHorario.Day = convert.dia;
                modeloHorario.Ano = year;
                modeloHorario.FechaWorking = dateTimeOffset;

            }
            catch (Exception ex)
            {

                throw;
            }
            

            return modeloHorario;
        }

        private async Task<CreateWorkingHoursModel> CreateModeloHorario2(LoadWorkingHoursModel convert)
        {
            var modeloHorario = new CreateWorkingHoursModel();
            try
            {
                Guid idPais = await _getCountryId.ConsultIdbyName(convert.pais);
                Guid idUser = await _getUserId.GetUserIdByEmployeeCode(convert.codigo_Empleado, idPais);
                //Guid idUser = await _getUserId.GetUserIdByID(convert.codigo_Empleado, idPais);

                convert.HoraInicio = DateTime.Parse(convert.HoraInicio).ToString("HH:mm");
                convert.HoraFin = DateTime.Parse(convert.HoraFin).ToString("HH:mm");

                DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact($"{convert.fecha} 00:00:00", "M/d/yy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime dateTime = dateTimeOffset.DateTime;
                Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                string weekOfYear = calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString();
                string year = dateTime.Year.ToString();

                modeloHorario.UserEntityId = idUser;
                modeloHorario.week = weekOfYear;
                modeloHorario.HoraInicio = FormatTime(convert.HoraInicio);
                modeloHorario.HoraFin = FormatTime(convert.HoraFin);
                modeloHorario.Day = convert.dia;
                modeloHorario.Ano = year;
                modeloHorario.FechaWorking = dateTimeOffset;

            }
            catch (Exception ex)
            {

                throw;
            }


            return modeloHorario;
        }

        private string FormatTime(string time)
        {
            string[] splitTime = time.Split(':');
            splitTime[0] = splitTime[0].PadLeft(2, '0');
            return string.Join(":", splitTime);
        }

    }
}
