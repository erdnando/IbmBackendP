using Algar.Hours.Domain.Entities.Load;

namespace Algar.Hours.Domain.Entities.ParametrosInicial
{
    public class ParametersArpInitialEntity
    {
        public Guid IdParametersInitialEntity { get; set; }
        public string EmployeeCode { get; set; }
        public string Anio { get; set; }
        public string FECHA_REP { get; set; }
        public string TOTAL_MINUTOS { get; set; }
        public string totalHoras { get; set; }
        public string HoraInicio { get; set; }
        public string HoraInicioHoraio { get; set; }
        public string HoraFin { get; set; }
        public string HoraFinHorario { get; set; }
        public string OutIme { get; set; }
        public string OverTime { get; set; }
        public int Semana { get; set; }
        public string Festivo { get; set; }
        public double HorasInicio { get; set; }
        public double HorasFin { get; set; }
        public string Estado { get; set; }
        public string EstatusProceso { get; set; }
        public Guid IdCarga { get; set; }
        //public Guid ARPLoadDetailEntityId { get; set; }
        //public ARPLoadDetailEntity ARPLoadDetailEntity { get; set;}

    }
}
