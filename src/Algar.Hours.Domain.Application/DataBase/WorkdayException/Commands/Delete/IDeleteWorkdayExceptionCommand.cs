using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Delete
{
    public interface IDeleteWorkdayExceptionCommand {
        Task<bool> DeleteWorkdayException(Guid WorkdayExceptionId);
    }
}
