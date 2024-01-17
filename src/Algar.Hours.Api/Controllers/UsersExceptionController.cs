using Algar.Hours.Application.DataBase.HoursReport.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.Update;
using Algar.Hours.Application.DataBase.UserException.Commands.Consult;
using Algar.Hours.Application.DataBase.UserException.Commands.Create;
using Algar.Hours.Application.DataBase.UserException.Commands.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class UsersExceptionController : Controller
    {

        [HttpPost("create")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Create(
           [FromBody] UsersExceptionsModelC model, [FromServices] ICreateUsersExceptionCommand createUsersException)
        {
            var data = await createUsersException.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("List")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> List(
          [FromServices] IConsultUserExceptionCommand consultUserExceptionCommand)
        {
            var data = await consultUserExceptionCommand.List();
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpPost("Update")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Update([FromBody] UsersExceptionsModelC model, [FromServices] IUpdateUsersExceptionCommand updateUsersException)
        {
            var data = await updateUsersException.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
    }
}
