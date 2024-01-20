using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public class LoadDTO
    {
        public JsonArray Data { get; set; }
        public JsonArray Data2 { get; set; }
        public JsonArray Data3 { get; set; }

    }

    public class LoadJsonPais
    {
        public JsonArray Data { get; set; }
        public string PaisSel { get; set; }

    }
}
