using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public interface ILoadGeneric
    {
        Task<bool> UploadHorizontalARP1(JsonArray partition);
        Task<bool> UploadHorizontalARP2(JsonArray partition);
        Task<bool> UploadHorizontalTSE1(JsonArray partition);
        Task<bool> UploadHorizontalTSE2(JsonArray partition);
        Task<bool> UploadHorizontalSTE1(JsonArray partition);
        Task<bool> UploadHorizontalSTE2(JsonArray partition);


    }



}
