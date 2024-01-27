using Algar.Hours.Application.DataBase.LoadData.LoadData;
using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class LoadController : ControllerBase
    {
        //[HttpPost("createARP")]
        //[Authorize(Roles = "standard")]
        //public async Task<IActionResult> CreateARP(
        //[FromBody] JsonArray model, [FromServices] ILoadHoursReport createLoadReportCommand)
        //{
        //    var data = await createLoadReportCommand.LoadARP(model);
        //    return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        //}

        //[HttpPost("createTSE")]
        //[Authorize(Roles = "standard")]
        //public async Task<IActionResult> CreateTSE([FromBody] JsonArray model, [FromServices] ILoadHoursReport createLoadReportCommand)
        //{
        //    var data = await createLoadReportCommand.LoadTSE(model);
        //    return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        //}
        //[HttpPost("createSTE")]
        //[Authorize(Roles = "standard")]
        //public async Task<IActionResult> CreateSTE([FromBody] JsonArray model, [FromServices] ILoadHoursReport createLoadReportCommand)
        //{
        //    var data = await createLoadReportCommand.LoadSTE(model);
        //    return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        //}      

        //[HttpPost("CreateFinal")]
        //[Authorize(Roles = "standard")]
        //public async Task<IActionResult> CreateFinal([FromBody] LoadDTO requestData, [FromServices] ILoadHoursReport loadHoursReport)
        //{
           
        //    var data = requestData.Data;
        //    var data2 = requestData.Data2;
        //    var data3 = requestData.Data3;

        //    var dataFinal = await loadHoursReport.Load(data, data2, data3);

        //    return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        //}

        [HttpPost("UploadARP")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadARP([FromBody] LoadJsonPais requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
            var dataFinal = await loadHoursReport.LoadARP(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }
        [HttpPost("UploadTSE")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadTSE([FromBody] LoadJsonPais requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
            var dataFinal = await loadHoursReport.LoadTSE(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }
        [HttpPost("UploadSTE")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadSTE([FromBody] LoadJsonPais requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
            var dataFinal = await loadHoursReport.LoadSTE(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }

        [HttpPost("Notificaciones")]
        [Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> NotificacionesProceso1([FromBody] Loadnotificaciones requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
            loadHoursReport.NotificacionesProceso1(requestData.IdCarga);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, true));
        }

        
        [HttpPost("ValidaLimitesExcepcionesOverlapping")]
       // [Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidaLimitesExcepcionesOverlapping([FromBody] Loadnotificaciones requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
            loadHoursReport.ValidaLimitesExcepcionesOverlapping(requestData.IdCarga);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, true));
        }


    }
}
