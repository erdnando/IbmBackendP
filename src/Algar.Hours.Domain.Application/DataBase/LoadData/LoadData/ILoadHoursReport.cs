using Algar.Hours.Application.DataBase.HorusReport.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public interface ILoadHoursReport
    {
        //Task<bool> LoadARP(JsonArray model);
        //Task<bool> LoadTSE(JsonArray model);
        //Task<bool> LoadSTE(JsonArray model);
        //Task<bool> Load(JsonArray model1, JsonArray model2, JsonArray model3);

        Task<string> LoadARP(LoadJsonPais model);
        Task<string> LoadTSE(LoadJsonPais model);
        Task<SummaryLoad> LoadSTE(LoadJsonPais model);
        void NotificacionesProceso1(LoadJsonPais model);
    }



}
