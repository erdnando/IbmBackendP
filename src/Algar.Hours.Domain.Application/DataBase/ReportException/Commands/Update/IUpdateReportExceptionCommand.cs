using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Application.DataBase.ReportException.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Update
{
    public interface IUpdateReportExceptionCommand
    {
        Task<Boolean> Update(ReportExceptionModel model);
    }
}
