using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load
{
    public class LoadWorkingHoursModel
    {
        [JsonProperty("#")]
        public int numero { get; set; }
        [JsonProperty("NOMBRE DIA")]
        public string dia { get; set; }
        [JsonProperty("FECHA")]
        public string fecha { get; set; }
        [JsonProperty("CODIGO EMP")]
        public string codigo_Empleado { get; set; }

        [JsonProperty("HORA INICIO")]
        public string HoraInicio { get; set; }
        [JsonProperty("HORA FIN")]
        public string HoraFin { get; set; }
        [JsonProperty("PAIS")]
        public string pais { get; set; }
    }
}
