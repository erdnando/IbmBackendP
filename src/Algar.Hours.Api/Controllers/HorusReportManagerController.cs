using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class HorusReportManagerController : ControllerBase
    {

        [HttpPost("create")]
        public async Task<IActionResult> Create(
        [FromBody] CreateHorusReportManagerModel model, [FromServices] ICreateHorusReportManagerCommand createHorusReportManagerCommand)
        {
            var data = await createHorusReportManagerCommand.Execute(model);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
    }
}
