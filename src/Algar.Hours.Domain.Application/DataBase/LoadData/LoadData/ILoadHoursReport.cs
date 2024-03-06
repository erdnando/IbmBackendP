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
        Task<string> GeneraCarga();
        Task<string> LoadARP(LoadJsonPais model);
        Task<string> LoadUserGMT(LoadJsonUserGMT model);
        Task<string> LoadTSE(LoadJsonPais model);
        Task<SummaryLoad> LoadSTE(LoadJsonPais model);
        Task<bool> NotificacionesProceso1(string model,string idUserEntiyId);
        Task<SummaryPortalDB> ValidaLimitesExcepcionesOverlapping(string idCarga);
        Task<CountsCarga> CargaAvance(string idCarga);

    }



}
