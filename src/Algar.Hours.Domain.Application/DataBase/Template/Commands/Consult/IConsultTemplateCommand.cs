using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.Template.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Template.Commands.Consult
{
    public interface IConsultTemplateCommand
    {
        Task<List<TemplateModel>> List();
        Task<TemplateModel> Consult(Guid id);
        //Task<List<WorkdayExceptionModel>> ConsultReportsByCountryId(Guid countryId);
    }
}
