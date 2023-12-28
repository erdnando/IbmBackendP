using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using Algar.Hours.Application.DataBase.Menu.Commands.GetList;
using Algar.Hours.Application.DataBase.Menu.Commands.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]

    public class MenuController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create(
          [FromBody] MenuModel model, [FromServices] ICreateMenuCommand createMenuCommand)
        {
            var data = await createMenuCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpGet("Consult")]
        public async Task<IActionResult> Consult(
       [FromQuery] Guid id, [FromServices] IConsultMenuCommand consultMenuCommand)
        {
            var data = await consultMenuCommand.Execute(id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(
          [FromBody] MenuModel model, [FromServices] IUpdateMenuCommand updateClientCommand)
        {

            var data = await updateClientCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("ListConsult")]
        public async Task<IActionResult> GetConsult(
       [FromServices] IGetListMenuCommand GetListMenuCommand)
        {

            var data = await GetListMenuCommand.Execute();
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
    }
}
