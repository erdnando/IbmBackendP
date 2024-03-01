using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Load
{
    public interface ILoadHorusReportManagerCommand
    {
        Task<FileStreamResult> LoadExcel(LoadHoursReportManagerModel model);
    }
}
