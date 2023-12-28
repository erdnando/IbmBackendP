using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Application.DataBase.RolMenu.Commands.Consult;
using Algar.Hours.Application.DataBase.RolMenu.Commands.CreateRolMenu;
using Algar.Hours.Application.DataBase.RolMenu.Commands.Delete;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class RolMenuController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create(
        [FromBody] CreateRolMenuModel model, [FromServices] ICreateRolMenuCommand createRolMenuCommand)
        {
            var data = await createRolMenuCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("Consult")]
        public async Task<IActionResult> Consult(
      [FromBody] CreateRolMenuModel model, [FromServices] ICreateRolMenuCommand createClientCommand)
        {

            var data = await createClientCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("Login")]
        public async Task<IActionResult> Login(
         [FromBody] CreateRolMenuModel model, [FromServices] ICreateRolMenuCommand createClientCommand)
        {

            var data = await createClientCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(
          [FromBody] CreateRolMenuModel model, [FromServices] ICreateRolMenuCommand createClientCommand)
        {

            var data = await createClientCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("ListByIdRol")]
        public async Task<IActionResult> ListByIdRol([FromQuery] Guid Id, [FromServices] IConsultRolMenuCommand consultRolMenuCommand)
        {
            var data = await consultRolMenuCommand.ListByIdRol(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpDelete("DeleteRolMenu")]
        public async Task<IActionResult> DeleteRolMenu([FromQuery] Guid Id, [FromServices] IDeleteRolMenuCommand deleteRolMenu)
        {
            var data = await deleteRolMenu.DeleteRolMenu(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
    }
}
