using Algar.Hours.Application.DataBase.Festivos.Consult;
using Algar.Hours.Application.DataBase.Festivos.Create;
using Algar.Hours.Application.DataBase.Festivos.Delete;
using Algar.Hours.Application.DataBase.Festivos.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class FestivosController : Controller
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create(
           [FromBody] List<CreateFestivoModel> model, [FromServices] ICreateFestivoCommand createFestivoCommand)
        {
            var data = await createFestivoCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpPost("update")]
        public async Task<IActionResult> Update(
           [FromBody] CreateFestivoModel model, [FromServices] IUpdateFestivoCommand updateFestivoCommand)
        {
            var data = await updateFestivoCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(
          [FromBody] CreateFestivoModel model, [FromServices] IDeleteFestivoCommand deleteFestivoCommand)
        {
            var data = await deleteFestivoCommand.Delete(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

        [HttpGet("ListAllFestivo")]
        public async Task<IActionResult> ListAll(
        [FromQuery] Guid CountryId,[FromServices] IConsultFestivosCommand consultFestivosCommand)
        {
            var data = await consultFestivosCommand.ListAll(CountryId);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
    }
}
