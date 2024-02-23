using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Http.Headers;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

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

        public async Task<FileStreamResult> LoadExcel(JsonArray models) {
            List<LoadHoursReportManagerModel> foundedWorkdays = new List<LoadHoursReportManagerModel>();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Workday");

            worksheet.Range("A1:F1").Style.Font.FontColor = XLColor.White;
            worksheet.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
            worksheet.Column("A").Width = 30;
            worksheet.Column("B").Width = 20;
            worksheet.Column("C").Width = 30;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 30;
            worksheet.Column("F").Width = 60;
            worksheet.Cell("A1").Value = "ID EMPLEADO";
            worksheet.Cell("B1").Value = "FECHA";
            worksheet.Cell("C1").Value = "TIPO";
            worksheet.Cell("D1").Value = "HORAS";
            worksheet.Cell("E1").Value = "ESTADO FINAL";
            worksheet.Cell("F1").Value = "LOG";

            var initialRow = 2;
            var currentRow = initialRow;
            for (var i = 0; i < models.Count(); i++) {
                var model = models[i];
                var loadWorkdayModel = Newtonsoft.Json.JsonConvert.DeserializeObject<LoadHoursReportManagerModel>(model.ToJsonString());

                var ampm = loadWorkdayModel.StartTime.Substring(6);
                var startHour = Convert.ToInt32(loadWorkdayModel.StartTime.Substring(0, 2));
                startHour += ampm == "PM"? 12 : 0;

                ampm = loadWorkdayModel.EndTime.Substring(6);
                var endHour = Convert.ToInt32(loadWorkdayModel.EndTime.Substring(0, 2));
                endHour += ampm == "PM" ? 12 : 0;

                var startTime = startHour.ToString("00") + loadWorkdayModel.StartTime.Substring(2, 3);
                var endTime = endHour.ToString("00") + loadWorkdayModel.EndTime.Substring(2, 3);

                var entity = _dataBaseService.HorusReportEntity
                    .Include(x => x.UserEntity)
                    .AsEnumerable()
                    .Where(x => x.UserEntity.EmployeeCode == loadWorkdayModel.EmployeeID && DateTime.ParseExact(x.StrStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) == loadWorkdayModel.ReportedDate && x.StartTime == startTime && x.EndTime == endTime)
                    .FirstOrDefault();
                if (entity != null)
                {
                    /*var modeloHorario = await CreateModeloHorario(convert);*/
                    var statusFinal = (entity.EstatusFinal == "FINAL" || entity.EstatusFinal == "SUBMITED") ? "EN PROCESO" : entity.EstatusFinal;
                    worksheet.Cell(currentRow, 1).Value = loadWorkdayModel.EmployeeID;
                    worksheet.Cell(currentRow, 2).Value = loadWorkdayModel.ReportedDate;
                    worksheet.Cell(currentRow, 3).Value = entity.EstatusOrigen;
                    worksheet.Cell(currentRow, 4).Value = loadWorkdayModel.Quantity;
                    worksheet.Cell(currentRow, 5).Value = statusFinal;
                    worksheet.Cell(currentRow, 6).Value = "";
                    
                }
                else {
                    worksheet.Cell(currentRow, 1).Value = loadWorkdayModel.EmployeeID;
                    worksheet.Cell(currentRow, 2).Value = loadWorkdayModel.ReportedDate;
                    worksheet.Cell(currentRow, 3).Value = "";
                    worksheet.Cell(currentRow, 4).Value = loadWorkdayModel.Quantity;
                    worksheet.Cell(currentRow, 5).Value = "RECHAZADO";
                    worksheet.Cell(currentRow, 6).Value = "";
                }

                currentRow++;
            }
            
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml") { FileDownloadName = "workday.xlsx" };
            
        }

        private async Task<CreateHorusReportManagerModel> CreateModeloHorario(LoadHoursReportManagerModel convert)
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
        }

        private string FormatTime(string time)
        {
            string[] splitTime = time.Split(':');
            splitTime[0] = splitTime[0].PadLeft(2, '0');
            return string.Join(":", splitTime);
        }
    }
}
