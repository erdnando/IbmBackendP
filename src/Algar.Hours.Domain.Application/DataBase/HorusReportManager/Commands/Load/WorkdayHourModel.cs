﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public class WorkdayHourModel {
        [JsonProperty("Employee ID")]
        public string EmployeeID { get; set; }
        [JsonProperty("Time Type")]
        public string Type { get; set; }
        public string Worker { get; set; }
        [JsonProperty("Reported Date")]
        public DateTime ReportedDate { get; set; }
        [JsonProperty("Original Reported Quantity")]
        public double OriginalQuantity { get; set; }
        [JsonProperty("Calculated Quantity")]
        public double Quantity { get; set; }
        public string Status { get; set; }
        [JsonProperty("In Time")]
        public string StartTime { get; set; }
        [JsonProperty("Out Time")]
        public string EndTime { get; set; }
    }
}
