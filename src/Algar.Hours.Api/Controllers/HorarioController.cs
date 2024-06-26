using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.Festivos.Create;
using Algar.Hours.Application.DataBase.Festivos.Update;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load;
using Algar.Hours.Application.DataBase.Template.Commands.Consult;
using Algar.Hours.Application.DataBase.Template.Commands.Create;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Consult;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.ServiceModel.Channels;
using System.Text.Json.Nodes;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class HorarioController : ControllerBase
    {

        private readonly IConfiguration _config;

        public HorarioController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("create")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Create(
                [FromBody] List<CreateWorkingHoursModel> model, [FromServices] ICreateWorkingHoursCommand CreateWorkingHoursCommand)
        {
            var data = await CreateWorkingHoursCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpPost("update")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Update(
           [FromBody] CreateFestivoModel model, [FromServices] IUpdateFestivoCommand updateFestivoCommand)
        {
            var data = await updateFestivoCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("Consul")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Consul(
         [FromBody] CreateFestivoModel model, [FromServices] IUpdateFestivoCommand updateFestivoCommand)
        {
            var data = await updateFestivoCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpGet("ConsultIdUserW")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> ConsultByIdUserW(
         [FromQuery] Guid IdUser, DateTimeOffset date, [FromServices] IConsultWorkingHoursCommand consultWorkingHours)
        {
            var data = await consultWorkingHours.Consult(IdUser, date);
            if(data.Count() > 0)
            {
                return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
            }
            else
            {

                return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, null));
            } 
        }

        [HttpGet("ConsultaHorarioCompleto")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> ConsultaHorarioCompleto(
         [FromQuery] Guid IdUser, DateTimeOffset date, [FromServices] IConsultWorkingHoursCommand consultWorkingHours)
        {
            var data = await consultWorkingHours.ConsultaHorarioCompleto(IdUser, date);
            if (data.Count() > 0)
            {
                return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
            }
            else
            {

                return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, null));
            }
        }

        [HttpPost("LoadExcel")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> LoadExcel(
                [FromBody] JsonArray model, [FromServices] ILoadWorkingHoursCommand loadWorkingHoursCommand)
        {
            var data = await loadWorkingHoursCommand.LoadExcel(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpPost("LoadExcelMan")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> LoadExcelMan(
                [FromBody] LoadHoursReportManagerModel model, [FromServices] ILoadHorusReportManagerCommand loadHorusReportManagerCommand)
        {
            var data = await loadHorusReportManagerCommand.LoadExcel(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpGet("Template")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Template([FromServices] ICreateTemplateCommand createTemplateCommand, [FromServices] IConsultTemplateCommand consultTemplateCommand) {
            var template = await consultTemplateCommand.Consult(Guid.Parse("4e24352f-9175-4046-9e39-3ee844b9f8f4"));
            var bytes = template.FileData;
            return File(bytes, template.FileContentType, template.FileName);
        }
    }
}
