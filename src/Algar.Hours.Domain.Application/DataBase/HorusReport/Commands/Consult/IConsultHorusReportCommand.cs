using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.HorusReport.Commands.Consult;

namespace Algar.Hours.Application.DataBase.HoursReport.Commands.Consult
{
	public interface IConsultHorusReportCommand
	{
		Task<HorusReportModel> Consult(Guid id);
		Task<List<ConsultMoldeHosrusReportModel>> List();
	}
}
