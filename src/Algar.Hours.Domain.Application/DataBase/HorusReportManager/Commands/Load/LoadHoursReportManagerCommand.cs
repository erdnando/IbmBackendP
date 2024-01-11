using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
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

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public class LoadHoursReportManagerCommand: ILoadHorusReportManagerCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private readonly ICreateHorusReportManagerCommand _ICreateHorusReportManagerCommand;
        
        public LoadHoursReportManagerCommand
            (IDataBaseService dataBaseService, IMapper mapper , ICreateHorusReportManagerCommand iCreateHorusReportManagerCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _ICreateHorusReportManagerCommand = iCreateHorusReportManagerCommand;


        }

        public async Task<bool> LoadExcel(JsonArray model)
        {
            try
            {
                List<List<CreateHorusReportManagerModel>> listaSemanasTotales = new List<List<CreateHorusReportManagerModel>>();
                List<CreateHorusReportManagerModel> listaSemana = new List<CreateHorusReportManagerModel>();

                var count = 0;

                foreach (var entity in model)
                {

                    count += 1;

                    var convert = Newtonsoft.Json.JsonConvert.DeserializeObject<LoadHoursReportManagerModel>(entity.ToJsonString());

                    var modeloHorario = await CreateModeloHorario(convert);
                    listaSemana.Add(modeloHorario);
                }

                listaSemanasTotales.Add(listaSemana);

                foreach (var lista in listaSemanasTotales)
                {
                    await _ICreateHorusReportManagerCommand.ExecuteLst(lista);
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
            
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
