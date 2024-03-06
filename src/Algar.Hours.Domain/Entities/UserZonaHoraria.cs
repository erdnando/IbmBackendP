using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities
{
    public class UserZonaHoraria
    {
        public Guid IdUserZone { get; set; }

        [JsonProperty("Número de empleado")]
        public string EmployeeCode { get; set; }

        [JsonProperty("Zona horaria")]
        public string ZonaHorariaU { get; set;}
    }
}
