using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public class LoadHoursReportManagerModel {
        public JsonArray hours { get; set; }
        public JsonArray users { get; set; }
    }
}
