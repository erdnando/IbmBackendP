using Algar.Hours.Application.DataBase.PortalDB.Commands;
using Algar.Hours.Application.DataBase.PortalDB.Commands.Create;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Create
{
    public interface ICreateHorusReportCommand
    {
        Task<HorusReportModel> Execute(CreateHorusReportModel model);
        Task<PortalDBModel> ExecutePortal(CreatePortalDBModel model);

    }
}
