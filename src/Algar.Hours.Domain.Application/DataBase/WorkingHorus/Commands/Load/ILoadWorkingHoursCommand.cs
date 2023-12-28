using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load
{
    public interface ILoadWorkingHoursCommand
    {
        Task<bool> LoadExcel(JsonArray model);
    }
}
