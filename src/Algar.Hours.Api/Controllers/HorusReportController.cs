using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using Algar.Hours.Application.DataBase.HorusReport.Commands.DetailAssigment;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Update;
using Algar.Hours.Application.DataBase.HoursReport.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.ListHoursUser;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	[TypeFilter(typeof(ExceptionManager))]
	public class HorusReportController : ControllerBase
	{
		[HttpPost("create")]
		public async Task<IActionResult> Create(
		[FromBody] CreateHorusReportModel model, [FromServices] ICreateHorusReportCommand createHorusReportCommand)
		{
			var data = await createHorusReportCommand.Execute(model);
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

		}
		[HttpGet("Consult")]
		public async Task<IActionResult> Consult(
		 [FromQuery] Guid id, [FromServices] IConsultHorusReportCommand consultHorusReportCommand)
		{
			var data = await consultHorusReportCommand.Consult(id);
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
		}

		[HttpGet("List")]
		public async Task<IActionResult> List(
		 [FromServices] IConsultHorusReportCommand listHorusReportCommand)
		{
			var data = await listHorusReportCommand.List();
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
		}
		[HttpPost("Update")]
		public async Task<IActionResult> Update(
		  [FromBody] HorusReportModel model, [FromServices] IUpdateHorusReportCommand updateHorusReportCommand)
		{

			var data = await updateHorusReportCommand.Update(model);
			return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

		}

        [HttpGet("ListHoursUser")]
        public async Task<IActionResult> ListHoursUser([FromQuery] Guid User,
         [FromServices] IListHoursUserCommand listHorusReportCommand)
        {
            var data = await listHorusReportCommand.Execute(User);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpGet("DetailAssigmentHoursUser")]
        public async Task<IActionResult> DetailAssigmentHoursUser([FromQuery] Guid IdReportHours,
         [FromServices] IConsultDetailAssigmentCommand listDetailAssigmentHoursUser)
        {
            var data = await listDetailAssigmentHoursUser.Execute(IdReportHours);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

    }
}
