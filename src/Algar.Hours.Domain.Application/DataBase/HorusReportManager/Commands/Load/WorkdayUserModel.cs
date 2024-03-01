using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public class WorkdayUserModel {
        public string Worker { get; set; }
        [JsonProperty("Employee ID")]
        public string EmployeeID { get; set; }
        [JsonProperty("Legal Name")]
        public string LegalName { get; set; }
        [JsonProperty("Preferred Name")]
        public string PreferredName { get; set; }
        [JsonProperty("Home CNUM")]
        public string HomeCNUM { get; set; }
    }
}
