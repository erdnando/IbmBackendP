using Algar.Hours.Domain.Entities.Load;

namespace Algar.Hours.Domain.Entities.ParametrosInicial
{
    public class ParametersArpInitialEntity
    {
        public Guid IdParametersInitialEntity { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string OutIme { get; set; }
        public string OverTime { get; set; }
        public int Semana { get; set; }
        public string Festivo { get; set; }
        public int HorasInicio { get; set; }
        public int HorasFin { get; set; }
        public string Estado { get; set; }
        public Guid ARPLoadDetailEntityId { get; set; }
        public ARPLoadDetailEntity ARPLoadDetailEntity { get; set;}

    }
}
