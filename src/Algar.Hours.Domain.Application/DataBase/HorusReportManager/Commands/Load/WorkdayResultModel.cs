using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public class WorkdayResultModel {
        public string employeeCode { get; set; }
        public string employeeName { get; set; }
        public string type { get; set; }
        public DateTime date { get; set; }
        public double hours { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public string finalStatus { get; set; }
    }
}
