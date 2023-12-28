using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.Create;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.UpdateAproveedNivel1;
using Algar.Hours.Application.DataBase.RolMenu.Commands.CreateRolMenu;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class AssignmentController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create(
        [FromBody] CreateAssignmentReportModel model, [FromServices] ICreateAssignmentReportCommand createAssigmentReportCommand)
        {
            var data = await createAssigmentReportCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("GetListUserAproveed")]
        public async Task<IActionResult> GetListUserAproveed(
         Guid IdUser, [FromServices] IListUserAproveedCommand createAssigmentReportCommand)
        {
            var data = createAssigmentReportCommand.Execute(IdUser);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpPost("UpdateAproveedNivel1")]
        public async Task<IActionResult> UpdateAproveedNivel2(
         [FromBody] ModelAproveed modelaprrove, [FromServices] IUpdateAproveedCommand UpdateAproveedReportCommand)
        {
            var data = UpdateAproveedReportCommand.Execute(modelaprrove);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
    }
}
