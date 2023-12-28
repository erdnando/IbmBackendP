namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Create
{
    public interface ICreateHorusReportCommand
    {
        Task<HorusReportModel> Execute(CreateHorusReportModel model);

    }
}
