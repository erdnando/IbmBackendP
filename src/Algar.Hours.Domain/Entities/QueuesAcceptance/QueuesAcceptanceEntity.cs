using Algar.Hours.Domain.Entities.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.QueuesAcceptance
{
    public class QueuesAcceptanceEntity
    {
        public Guid IdQueuesAcceptanceEntity { get; set; }  
        public string Id_empleado { get; set; }
        public string Hora_Inicio { get; set; }
        public string Hora_Fin { get; set; }
        public double Horas_Total { get; set; }
        public string Comentario { get; set; }
        public DateTime AprobadoSistema { get; set; } 
        public int Estado { get; set; }
        public Guid ARPLoadDetailEntityId { get; set; }
        public string FechaRe { get; set; }
        public ARPLoadDetailEntity ARPLoadDetailEntity { get; set; }

    }
}
