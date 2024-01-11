using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public class LoadHoursReportManagerModel
    {
        public long UserEntityManagerId { get; set; }
        [JsonProperty("Employee ID")]
        public long EmployeeID { get; set; }
        public string Worker { get; set; }
        [JsonProperty("Reported Date")]
        public DateTime ReportedDate { get; set; }
        [JsonProperty("Calculated Quantity")]
        public double Quantity { get; set; }
        public string Status { get; set; }
    }
}
