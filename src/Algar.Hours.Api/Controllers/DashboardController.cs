using Algar.Hours.Application.DataBase.Rol.Commands.Consult;
using Algar.Hours.Application.DataBase.Rol.Commands.Update;
using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Update;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands.UpdateAproveedNivel1;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Algar.Hours.Application.DataBase.Dashboard.Commands.Consult;

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]
    public class DashoardController : Controller
    {

        [HttpGet("Reporte1/{semana}/{usuario}/{anio}")]
        public async Task<IActionResult> Reporte1(int semana, string usuario,int anio, [FromServices] IReporte1Command reporte)
        {
            var data = await reporte.Reporte1(semana, usuario, anio);
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }

        [HttpGet("ReporteGraficas/{anio}/{usuario}")]
        public async Task<IActionResult> ReporteGraficas(int anio, string usuario, [FromServices] IReporte1Command reporte)
        {
            var data = await reporte.ReporteGraficas(anio, usuario);
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }

    }
}
