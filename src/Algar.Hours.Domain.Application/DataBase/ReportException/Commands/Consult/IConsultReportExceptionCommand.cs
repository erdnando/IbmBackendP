using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Consult
{
    public interface IConsultReportExceptionCommand
    {
        Task<List<ReportExceptionModel>> List();
        //Task<List<ReportExceptionModel>> ConsultReportsByCountryId(Guid countryId);
    }
}
