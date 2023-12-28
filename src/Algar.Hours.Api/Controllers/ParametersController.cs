using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Parameters.Commands.ConsulParameters;
using Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters;
using Algar.Hours.Application.DataBase.Parameters.Commands.UpdateParameters;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class ParametersController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create(
        [FromBody] CreateParametersModel model, [FromServices] ICreateParametersCommand createParametersCommand)
        {
            var data = await createParametersCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        
        }
        [HttpGet("Consult")]
        public async Task<IActionResult> Consult(
       [FromQuery] Guid IdCountry, [FromServices] IConsultParametersCommand createClientCommand)
        {
            var data = await createClientCommand.Execute(IdCountry);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(
          [FromBody] UpdateParametersModel model, [FromServices] IUpdtaeParametersCommand updateParametersCommand)
        {

            var data = await updateParametersCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }



    }
}
