using Algar.Hours.Application.DataBase.Menu.Commands.GetList;
using Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters;
using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Application.DataBase.Rol.Commands.Consult;
using Algar.Hours.Application.DataBase.Rol.Commands.Update;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class RolController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create(
           [FromBody] CreateRolModel model, [FromServices] ICreateRolCommand createRolCommand)
        {
            var data = await createRolCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("Consult")]
        public async Task<IActionResult> Consult(
        [FromBody] CreateRolModel model, [FromServices] ICreateRolCommand createClientCommand)
        {

            var data = await createClientCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(
          [FromBody] RolModel model, [FromServices] IUpdateRolCommand updateRolCommand)
        {

            var data = await updateRolCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("ListConsult")]
        public async Task<IActionResult> GetConsult([FromServices] IGetListRolCommand GetListRolCommand)
        {
            var data = await GetListRolCommand.List();
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
    }
}
