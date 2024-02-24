using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Consult
{
    public interface IConsultWorkdayExceptionCommand
    {
        Task<List<WorkdayExceptionModel>> List();
        //Task<List<WorkdayExceptionModel>> ConsultReportsByCountryId(Guid countryId);
    }
}
