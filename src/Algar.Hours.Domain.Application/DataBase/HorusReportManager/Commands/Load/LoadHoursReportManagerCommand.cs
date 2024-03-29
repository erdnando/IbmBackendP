﻿using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Http.Headers;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Algar.Hours.Domain.Entities.WorkdayException;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Algar.Hours.Domain.Entities.HorusReport;
using MySqlX.XDevAPI.Common;
using Npgsql;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public class LoadHoursReportManagerCommand: ILoadHorusReportManagerCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private readonly ICreateHorusReportManagerCommand _ICreateHorusReportManagerCommand;
        private IGetListUsuarioCommand _usuarioCommand;

        public LoadHoursReportManagerCommand
            (IDataBaseService dataBaseService, IMapper mapper , ICreateHorusReportManagerCommand iCreateHorusReportManagerCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _ICreateHorusReportManagerCommand = iCreateHorusReportManagerCommand;
            

        }

        public async Task<FileStreamResult> LoadExcel(LoadHoursReportManagerModel models) {
            var workbook = new XLWorkbook();
            var wsSummary = workbook.Worksheets.Add("Summary");
            var worksheet = workbook.Worksheets.Add("Workday");

            wsSummary.Column("A").Width = 30;

            worksheet.Range("A1:G1").Style.Font.FontColor = XLColor.White;
            worksheet.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
            worksheet.Column("A").Width = 30;
            worksheet.Column("B").Width = 35;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 30;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 30;
            worksheet.Column("G").Width = 60;
            worksheet.Cell("A1").Value = "ID EMPLEADO";
            worksheet.Cell("B1").Value = "NOMBRE EMPLEADO";
            worksheet.Cell("C1").Value = "FECHA";
            worksheet.Cell("D1").Value = "TIPO";
            worksheet.Cell("E1").Value = "HORAS";
            worksheet.Cell("F1").Value = "ESTADO FINAL";
            worksheet.Cell("G1").Value = "LOG";

            var initialRow = 2;
            var currentRow = initialRow;
            List<WorkdayExceptionEntity> exceptions = new List<WorkdayExceptionEntity>();
            List<string> employeeCodes = new List<string>();
            List<WorkdayHourModel> whModels = new List<WorkdayHourModel>();
            List<WorkdayUserModel> wuModels = new List<WorkdayUserModel>();

            DateTime dateTime = DateTime.Now;
            for (var i = 0; i < models.hours.Count(); i++) {
                var model = models.hours[i];
                var loadWorkdayModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkdayHourModel>(model.ToJsonString());
                if (loadWorkdayModel.StartTime == null || loadWorkdayModel.EndTime == null) continue;
                whModels.Add(loadWorkdayModel);

                if (loadWorkdayModel.ReportedDate < dateTime) dateTime = loadWorkdayModel.ReportedDate;
            }

            for (var i = 0; i < models.users.Count(); i++) {
                var model = models.users[i];
                var loadWorkdayModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkdayUserModel>(model.ToJsonString());
                wuModels.Add(loadWorkdayModel);
                employeeCodes.Add(loadWorkdayModel.HomeCNUM);
            }

            String employeeCodesIn = $"'{String.Join("','", employeeCodes.ToArray())}'";
            exceptions = _dataBaseService.WorkdayExceptionEntity.FromSqlRaw($"SELECT * FROM \"WorkdayExceptionEntity\" WHERE \"Active\" = true AND \"EmployeeCode\" IN ({employeeCodesIn}) AND \"RealDate\" >= TO_DATE('{dateTime.ToString("dd/MM/yyyy")}', 'DD/MM/YYYY')").ToList();
            var entities = _dataBaseService.HorusReportEntity.FromSqlRaw($"SELECT * FROM \"HorusReportEntity\" h INNER JOIN \"UserEntity\" u on u.\"IdUser\" = h.\"UserEntityId\" WHERE u.\"EmployeeCode\" IN ({employeeCodesIn}) AND TO_TIMESTAMP(h.\"StrStartDate\", 'DD/MM/YYYY HH24:MI') >= TO_TIMESTAMP('{dateTime.ToString("dd/MM/yyyy HH:mm")}', 'DD/MM/YYYY HH24:MI')")
                .Include(x => x.UserEntity)
                .ToList();

            for (var i = 0; i < whModels.Count(); i++) {
                WorkdayHourModel workdayHourModel = whModels[i];
                WorkdayUserModel workdayUserModel = null;
                foreach (var model in wuModels) {
                    if (workdayHourModel.EmployeeID == model.EmployeeID) {
                        workdayUserModel = model; break;
                    }
                }

                if (workdayUserModel == null) continue;
                if (workdayHourModel.Type != "Standby" && workdayHourModel.Type != "Overtime" && workdayHourModel.Type != "Vacation" && workdayHourModel.Type != "Holiday Worked") continue;

                var startTime = DateTime.Parse(workdayHourModel.StartTime.Substring(0, 8)).ToString("HH:mm");
                var endTime = DateTime.Parse(workdayHourModel.EndTime.Substring(0, 8)).ToString("HH:mm");

                HorusReportEntity horusReportEntity = null;
                foreach (var entity in entities) {
                    if (entity.UserEntity.EmployeeCode == workdayUserModel.HomeCNUM && DateTime.ParseExact(entity.StrStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) == workdayHourModel.ReportedDate && entity.StartTime == startTime && entity.EndTime == endTime) { horusReportEntity = entity; break; };
                }

                if (horusReportEntity != null) {
                    var statusFinal = (horusReportEntity.EstatusFinal == "FINAL" || horusReportEntity.EstatusFinal == "SUBMITTED") ? "ENPROCESO" : "APROBADO";
                    worksheet.Cell(currentRow, 1).Value = horusReportEntity.UserEntity.EmployeeCode;
                    worksheet.Cell(currentRow, 2).Value = workdayUserModel.Worker;
                    worksheet.Cell(currentRow, 3).Value = DateTime.ParseExact(horusReportEntity.StrStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 4).Value = horusReportEntity.EstatusOrigen;
                    worksheet.Cell(currentRow, 5).Value = horusReportEntity.CountHours;
                    worksheet.Cell(currentRow, 6).Value = statusFinal;
                    worksheet.Cell(currentRow, 7).Value = "";
                }
                else {
                    var exception = exceptions.Where(x => x.EmployeeCode == workdayUserModel.HomeCNUM && x.RealDate.ToString("dd/MM/yyyy") == workdayHourModel.ReportedDate.ToString("dd/MM/yyyy") && x.RealStartTime.ToString(@"hh\:mm") == startTime && x.RealEndTime.ToString(@"hh\:mm") == endTime)
                    .FirstOrDefault();

                    worksheet.Cell(currentRow, 1).Value = workdayUserModel.EmployeeID;
                    worksheet.Cell(currentRow, 2).Value = workdayUserModel.Worker;
                    worksheet.Cell(currentRow, 3).Value = workdayHourModel.ReportedDate;
                    worksheet.Cell(currentRow, 4).Value = workdayHourModel.Type;
                    worksheet.Cell(currentRow, 5).Value = workdayHourModel.Quantity;
                    worksheet.Cell(currentRow, 7).Value = "";

                    if (exception != null) {
                        worksheet.Cell(currentRow, 6).Value = "APROBADO POR EXCEPCION";
                    }
                    else {
                        worksheet.Cell(currentRow, 6).Value = "RECHAZADO";
                    }

                }

                currentRow++;

            }

            var pivotTable = wsSummary.PivotTables.Add("pvt", wsSummary.Cell("A1"), worksheet.Range($"A1:F{(currentRow-1)}"));
            pivotTable.RowLabels.Add("TIPO");
            pivotTable.ColumnLabels.Add("ESTADO FINAL");
            pivotTable.Values.Add("TIPO", "TIPOS").SetSummaryFormula(XLPivotSummary.Count);
            pivotTable.SetShowGrandTotalsRows(true);

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml") { FileDownloadName = "workday.xlsx" };
            
        }

        /*private async Task<CreateHorusReportManagerModel> CreateModeloHorario(LoadHoursReportManagerModel convert)
        {
            var modeloHorario = new CreateHorusReportManagerModel();
            try
            {
                var fechasr = convert.ReportedDate.ToString().Split("/");

                DateTime dateTime = new DateTime(int.Parse(fechasr[2].Substring(0, 4)),int.Parse(fechasr[1]), int.Parse(fechasr[0]));
                Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                string weekOfYear = calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString();
                string year = dateTime.Year.ToString();

                modeloHorario.UserEntityManagerId = new Guid("3696718d-d05a-4831-96ce-ed500c5bbc98");
                modeloHorario.UserEntityId = new Guid("3696718d-d05a-4831-96ce-ed500c5bbc97");
                modeloHorario.CountHours = convert.Quantity;
                modeloHorario.CreationDate = convert.ReportedDate;
                modeloHorario.Status = convert.Status;
                modeloHorario.Observations= "Ninguna";
                modeloHorario.StartTime = "01:00";
                modeloHorario.EndTime = "09:00";
                modeloHorario.TypeReport="1";

            }
            catch (Exception ex)
            {

                throw;
            }




            return modeloHorario;
        }*/


        private string FormatTime(string time)
        {
            string[] splitTime = time.Split(':');
            splitTime[0] = splitTime[0].PadLeft(2, '0');
            return string.Join(":", splitTime);
        }
    }
}
