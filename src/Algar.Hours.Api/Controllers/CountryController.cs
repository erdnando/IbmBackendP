using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Country.Commands.Consult;
using Algar.Hours.Application.DataBase.Country.Commands.Create;
using Algar.Hours.Application.DataBase.Country.Commands.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	[TypeFilter(typeof(ExceptionManager))]
	public class CountryController : ControllerBase
	{
		[HttpPost("create")]
		public async Task<IActionResult> Create(
		[FromBody] CountryModel model, [FromServices] ICreateCountryCommand createCountryCommand)
		{
			var data = await createCountryCommand.Execute(model);
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

		}
		[HttpGet("Consult")]
		public async Task<IActionResult> Consult(
		 [FromQuery] Guid id, [FromServices] IConsultCountryCommand consultCountryCommand)
		{

			var data = await consultCountryCommand.Consult(id);
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

		}

		[HttpGet("List")]
		public async Task<IActionResult> List(
		 [FromServices] IConsultCountryCommand listCountryCommand)
		{
			var data = await listCountryCommand.List();
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
		}
		[HttpPost("Update")]
		public async Task<IActionResult> Update(
		  [FromBody] CountryModel model, [FromServices] IUpdateCountryCommand updateCountryCommand)
		{

			var data = await updateCountryCommand.Update(model);
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

		}

	}
}
