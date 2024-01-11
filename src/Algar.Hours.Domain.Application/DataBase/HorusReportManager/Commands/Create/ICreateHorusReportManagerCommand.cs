using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using Algar.Hours.Application.DataBase.HorusReport.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create
{
    public interface ICreateHorusReportManagerCommand
    {
        Task<HorusReportManagerModel> Execute(CreateHorusReportManagerModel model);
        Task<bool> ExecuteLst(List<CreateHorusReportManagerModel> model);
    }
}
