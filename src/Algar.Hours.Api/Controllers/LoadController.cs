using Algar.Hours.Application.DataBase.LoadData.LoadData;
using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Algar.Hours.Domain.Entities.ParametrosInicial;
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

        [HttpGet("GeneraCarga")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> GeneraCarga( [FromServices] ILoadHoursReport loadHoursReport)
        {
            var dataFinal = await loadHoursReport.GeneraCarga();

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }


        [HttpGet("CargaAvance")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> CargaAvance([FromQuery] string idCarga,[FromServices] ILoadHoursReport loadHoursReport)
        {
            var dataFinal = await loadHoursReport.CargaAvance(idCarga);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }

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


        
        [HttpPost("ValidaLimitesExcepcionesOverlapping")]
       // [Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidaLimitesExcepcionesOverlapping([FromBody] Loadnotificaciones requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
            var dataFinal = await loadHoursReport.ValidaLimitesExcepcionesOverlapping(requestData.IdCarga);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }


        [HttpPost("Notificaciones")]
        [Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> NotificacionesProceso1([FromBody] Loadnotificaciones requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
            loadHoursReport.NotificacionesProceso1(requestData.IdCarga,requestData.idUserEntiyId);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, true));
        }

        [HttpPost("UploadHorizontalARP1")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadHorizontalARP1([FromBody] JsonArray requestData, [FromServices] ILoadGeneric loadgeneric)
        {
            var dataFinal = await loadgeneric.UploadHorizontalARP1(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }

        [HttpPost("UploadHorizontalARP2")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadHorizontalARP2([FromBody] JsonArray requestData, [FromServices] ILoadGeneric loadgeneric)
        {
            var dataFinal = await loadgeneric.UploadHorizontalARP2(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }

        [HttpPost("UploadHorizontalTSE1")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadHorizontalTSE1([FromBody] JsonArray requestData, [FromServices] ILoadGeneric loadgeneric)
        {
            var dataFinal = await loadgeneric.UploadHorizontalTSE1(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }

        [HttpPost("UploadHorizontalTSE2")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadHorizontalTSE2([FromBody] JsonArray requestData, [FromServices] ILoadGeneric loadgeneric)
        {
            var dataFinal = await loadgeneric.UploadHorizontalTSE2(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }

        [HttpPost("UploadHorizontalSTE1")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadHorizontalSTE1([FromBody] JsonArray requestData, [FromServices] ILoadGeneric loadgeneric)
        {
            var dataFinal = await loadgeneric.UploadHorizontalSTE1(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }

        [HttpPost("UploadHorizontalSTE2")]
        //[Authorize(Roles = "standard")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadHorizontalSTE2([FromBody] JsonArray requestData, [FromServices] ILoadGeneric loadgeneric)
        {
            var dataFinal = await loadgeneric.UploadHorizontalSTE2(requestData);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }




    }
}
