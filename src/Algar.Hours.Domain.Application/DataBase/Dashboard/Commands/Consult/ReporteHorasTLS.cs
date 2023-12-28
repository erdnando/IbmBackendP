using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Dashboard.Commands.Consult
{
    public class GralReportesHrsTSO
    {
        public List<GralReportHoras> ReposterGral { get; set; }
    }
    public class GralReportHoras
    {
        public string ReporteName { get; set; }
        public List<ReporteHorasTLS> ReportesTLS { get; set; }
    }
    public class ReporteHorasTLS
    {
        public string Tool { get; set; }
        public List<WeekDaysTls> weekDaysTls { get; set; }
    }

    public class WeekDaysTls
    {
        public string Fecha { get; set; }
        public double TotalHoras { get; set; }
    }

    public class GralReporteHorasMesTLS
    {
        public List<ReporteHorasMesTLS> ReportesGral { get; set; }
    }

    public class ReporteHorasMesTLS
    {
        public string Tool { get; set; }
        public int Anio { get; set; }
        public List<MonthTls> monthTls { get; set; }
    }

    public class MonthTls
    {
        public int Mes { get; set; }
        public double TotalHoras { get; set; }
    }
}
