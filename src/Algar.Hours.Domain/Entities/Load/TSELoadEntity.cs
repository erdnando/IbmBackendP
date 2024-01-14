using Newtonsoft.Json;
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
        [JsonProperty("Recurso de servicio: Usuario: ISO 2")]
        public string UsuarioISO2 { get; set; }
        [JsonProperty("TSE: Work Order")]
        public string WorkOrder { get; set; }
        [JsonProperty("Recurso de servicio: Usuario: Número de empleado")]
        public string NumeroEmpleado { get; set; }
        [JsonProperty("Recurso de servicio: Usuario: Zona horaria")]
        public string ZonaHoraria { get; set; }
        [JsonProperty("Orden de trabajo: Caso: Account CMR Number")]
        public string AccountCMRNumber { get; set; }
        [JsonProperty("Orden de trabajo: Caso: Account Name Text")]
        public string AccountNameText { get; set; }
        [JsonProperty("TSE: Status")]
        public string Status { get; set; }
        [JsonProperty("TSE: Start Time")]
        public string StartTime {  get; set; }
        [JsonProperty("TSE: End Time")]
        public string EndTime {  get; set; }
        public string StartHours { get; set; }
        public string EndHours { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        [JsonProperty("Duration in Hours")]
        public string DurationInHours { get; set; }
        [JsonProperty("WO: Subject")]
        public string Subject { get; set; }
        public string FechaRegistro { get; set; }

    }
}
