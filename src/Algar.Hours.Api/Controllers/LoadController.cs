﻿using Algar.Hours.Application.DataBase.LoadData.LoadData;
using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class LoadController : ControllerBase
    {
        [HttpPost("createARP")]
        public async Task<IActionResult> CreateARP(
        [FromBody] JsonArray model, [FromServices] ILoadHoursReport createLoadReportCommand)
        {
            var data = await createLoadReportCommand.LoadARP(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }

        [HttpPost("createTSE")]
        public async Task<IActionResult> CreateTSE([FromBody] JsonArray model, [FromServices] ILoadHoursReport createLoadReportCommand)
        {
            var data = await createLoadReportCommand.LoadTSE(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpPost("createSTE")]
        public async Task<IActionResult> CreateSTE([FromBody] JsonArray model, [FromServices] ILoadHoursReport createLoadReportCommand)
        {
            var data = await createLoadReportCommand.LoadSTE(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }      

        [HttpPost("CreateFinal")]
        public async Task<IActionResult> CreateFinal([FromBody] LoadDTO requestData, [FromServices] ILoadHoursReport loadHoursReport)
        {
           
            var data = requestData.Data;
            var data2 = requestData.Data2;
            var data3 = requestData.Data3;

            var dataFinal = await loadHoursReport.Load(data, data2, data3);

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, dataFinal));
        }
    }
}
