using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Load
{
    public class STELoadEntity
    {
        public Guid IdSTELoad { get; set; }
        [JsonProperty("Session Time Unique ID")]
        public string SessionTimeId { get; set;}
        [JsonProperty("Session Time Support Agent Country")]
        public string SessionTimeAgentCountry { get; set;}
        [JsonProperty("Número del caso")]
        public string NumeroCaso { get; set;}
        [JsonProperty("Session Time Creator Employee Serial Number")]
        public string SessionEmployeeSerialNumber { get; set; }
        [JsonProperty("Account CMR Number")]
        public string AccountCMRNumber { get; set;}
        [JsonProperty("Nombre de la cuenta: Nombre de la cuenta")]
        public string NombreCuenta { get; set;}
        [JsonProperty("Start Date/Time")]
        public string StartDateTime { get; set;}
        [JsonProperty("TSE: Status")]
        public string Status { get; set; }
        [JsonProperty("End Date/Time")]
        public string EndDateTime { get; set;}
        public string EndHours { get; set;}
        public string StartHours { get; set;}
        [JsonProperty("Session Time: Total Duration")]
        public string TotalDuration { get; set;}
        [JsonProperty("Case Subject")]
        public string CaseSubject { get; set;}
        public string FechaRegistro { get; set; }
    }
}
