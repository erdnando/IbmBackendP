using Algar.Hours.Application.DataBase.Festivos.Create;
using Algar.Hours.Application.DataBase.Festivos.Update;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Consult;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class HorarioController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create(
                [FromBody] List<CreateWorkingHoursModel> model, [FromServices] ICreateWorkingHoursCommand CreateWorkingHoursCommand)
        {
            var data = await CreateWorkingHoursCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update(
           [FromBody] CreateFestivoModel model, [FromServices] IUpdateFestivoCommand updateFestivoCommand)
        {
            var data = await updateFestivoCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("Consul")]
        public async Task<IActionResult> Consul(
         [FromBody] CreateFestivoModel model, [FromServices] IUpdateFestivoCommand updateFestivoCommand)
        {
            var data = await updateFestivoCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpGet("ConsultIdUserW")]
        public async Task<IActionResult> ConsultByIdUserW(
         [FromQuery] Guid IdUser, string week, string ano,[FromServices] IConsultWorkingHoursCommand consultWorkingHours)
        {
            var data = await consultWorkingHours.Consult(IdUser, week, ano);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpPost("LoadExcel")]
        public async Task<IActionResult> LoadExcel(
                [FromBody] JsonArray model, [FromServices] ILoadWorkingHoursCommand loadWorkingHoursCommand)
        {
            var data = await loadWorkingHoursCommand.LoadExcel(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
    }
}
