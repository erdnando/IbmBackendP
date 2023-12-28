using Algar.Hours.Domain.Entities.Load;

namespace Algar.Hours.Domain.Entities.QueuesAcceptance
{
    public class QueuesAcceptanceEntitySTE
    {
        public Guid IdQueuesAcceptanceEntitySTE { get; set; }
        public string Id_empleado { get; set; }
        public string Hora_Inicio { get; set; }
        public string Hora_Fin { get; set; }
        public double Horas_Total { get; set; }
        public string Comentario { get; set; }
        public DateTime AprobadoSistema { get; set; }
        public int Estado { get; set; }
        public Guid STELoadEntityId { get; set; }
        public string FechaRe { get; set; }
        public STELoadEntity STELoadEntity { get; set; }
    }
}
