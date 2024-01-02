using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Dashboard.Commands.Consult
{
    public interface IReporte1Command
    {
        Task<GralReportesHrsTSO> Reporte1(int semana, string usuario, int anio);
        Task<GralReporteHorasMesTLS> ReporteGraficas(int anio, string usuario);

    }
}
