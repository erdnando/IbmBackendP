namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Update
{
	public interface IUpdateHorusReportCommand
	{
		Task<Boolean> Update(HorusReportModel model);
	}
}
