using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class AprobadorController : Controller
    {
        [HttpPost("create")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Create(
           [FromBody] AprobadorModel model, [FromServices] ICreateAprobadorCommand createAprobador)
        {
            var data = await createAprobador.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpPost("update")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Update(
           [FromBody] AprobadorModel model, [FromServices] IUpdateAprobadorCommand updateAprobador)
        {
            var data = await updateAprobador.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("ListConsult")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> ListAll([FromServices] IConsultAprobadorCommand consultAprobadorCommand)
        {
            var data = await consultAprobadorCommand.ListAll();
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpGet("ConsultById")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> ConsultById([FromQuery] Guid Id, [FromServices] IConsultAprobadorCommand consultAprobador)
        {
            var data = await consultAprobador.ConsultById(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
    }
}
