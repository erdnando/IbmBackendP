using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Delete
{
    public interface IDeleteReportExceptionCommand
    {
        Task<bool> DeleteReportException(Guid ReportExceptionId);
    }
}
