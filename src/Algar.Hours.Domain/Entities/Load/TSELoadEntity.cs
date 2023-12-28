using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Load
{
    public class TSELoadEntity
    {
        public Guid IdTSELoad { get; set; }
        public string UsuarioISO2 { get; set; }
        public string WorkOrder { get; set; }
        public string NumeroEmpleado { get; set; }
        public string ZonaHoraria { get; set; }
        public string AccountCMRNumber { get; set; }
        public string AccountNameText { get; set; }
        public string Status { get; set; }
        public string StartTime {  get; set; }
        public string EndTime {  get; set; }
        public string StartHours { get; set; }
        public string EndHours { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string DurationInHours { get; set; }
        public string Subject { get; set; }
        public string FechaRegistro { get; set; }

    }
}
