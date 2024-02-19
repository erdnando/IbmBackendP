using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Application.DataBase.ReportException.Commands.Create;
using Algar.Hours.Application.DataBase.ReportException.Commands.Delete;
using Algar.Hours.Application.DataBase.ReportException.Commands.Update;
using Algar.Hours.Application.DataBase.RolMenu.Commands.Delete;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class ReportExceptionController : Controller
    {

        [HttpPost("create")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Create(
           [FromBody] ReportExceptionModelC model, [FromServices] ICreateReportExceptionCommand createReportException)
        {
            var data = await createReportException.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("List")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> List(
          [FromServices] IConsultReportExceptionCommand consultReportExceptionCommand)
        {
            var data = await consultReportExceptionCommand.List();
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
        public async Task<IActionResult> Update([FromBody] ReportExceptionModel model, [FromServices] IUpdateReportExceptionCommand updateReportException)
        {
            var data = await updateReportException.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpDelete("Delete")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Delete([FromQuery] Guid Id, [FromServices] IDeleteReportExceptionCommand deleteReportException)
        {
            var data = await deleteReportException.DeleteReportException(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
    }
}
