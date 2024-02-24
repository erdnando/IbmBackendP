using Algar.Hours.Application.DataBase.WorkdayException.Commands.Activate;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Consult;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Create;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Deactivate;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Delete;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class WorkdayExceptionController : Controller {
        
        [HttpPost("create")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Create([FromBody] WorkdayExceptionModelC model, [FromServices] ICreateWorkdayExceptionCommand createWorkdayException) {
            var data = await createWorkdayException.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpGet("List")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> List([FromServices] IConsultWorkdayExceptionCommand consultWorkdayExceptionCommand) {
            var data = await consultWorkdayExceptionCommand.List();
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        /*[HttpGet("GetByCountryList")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> ListByCountryId([FromQuery] Guid Id, [FromServices] IConsultReportExceptionCommand consultReportExceptionCommand)
        {
            var data = await consultReportExceptionCommand.ConsultReportsByCountryId(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }*/
        [HttpPost("Update")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Update([FromBody] WorkdayExceptionModel model, [FromServices] IUpdateWorkdayExceptionCommand updateWorkdayException) {
            var data = await updateWorkdayException.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("Activate")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Activate([FromQuery] Guid Id, [FromServices] IActivateWorkdayExceptionCommand activateWorkdayException) {
            var data = await activateWorkdayException.ActivateWorkdayException(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("Deactivate")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Deactivate([FromQuery] Guid Id, [FromServices] IDeactivateWorkdayExceptionCommand deactivateWorkdayException) {
            var data = await deactivateWorkdayException.DeactivateWorkdayException(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
    }
}
