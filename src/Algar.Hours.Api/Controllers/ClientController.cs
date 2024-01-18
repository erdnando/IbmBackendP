using Algar.Hours.Application.DataBase.Client.Commands;
using Algar.Hours.Application.DataBase.Client.Commands.Consult;
using Algar.Hours.Application.DataBase.Client.Commands.Create;
using Algar.Hours.Application.DataBase.Client.Commands.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class ClientController : ControllerBase
    {
        [HttpPost("create")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Create(
        [FromBody] ClientModel model, [FromServices] ICreateClientCommand createClientCommand)
        {
            var data = await createClientCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("Consult")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Consult(
         [FromQuery] Guid id, [FromServices] IConsultClientCommand consultClientCommand)
        {

            var data = await consultClientCommand.Consult(id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

		[HttpGet("List")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> List(
		 [FromServices] IConsultClientCommand listClientCommand)
		{
			var data = await listClientCommand.List();
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
		}

        [HttpPost("Update")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Update(
          [FromBody] ClientModel model, [FromServices] IUpdateClientCommand updateClientCommand)
        {

            var data = await updateClientCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

    }
}
