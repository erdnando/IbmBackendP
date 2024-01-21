using Algar.Hours.Domain.Entities.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.ParametrosInicial
{
    public class ParametersTseInitialEntity
    {
        public Guid IdParamTSEInitialId { get; set; }
        public string EmployeeCode { get; set; }
        public string Anio { get; set; }
        public string FECHA_REP { get; set; }
        public string TOTAL_MINUTOS { get; set; }
        public string totalHoras { get; set; }
        
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string OutIme { get; set; }
        public string OverTime { get; set; }
        public int Semana { get; set; }
        public string Festivo { get; set; }
        public double HorasInicio { get; set; }
        public double HorasFin { get; set; }
        public string Estado { get; set; }
        public string EstatusProceso { get; set; }
       // public Guid TSELoadEntityIdTSELoad { get; set; }
       // public TSELoadEntity TSELoadEntity { get; set; }
    }
}
