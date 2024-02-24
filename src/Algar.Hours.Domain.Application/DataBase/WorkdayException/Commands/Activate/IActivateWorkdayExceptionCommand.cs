using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Activate
{
    public interface IActivateWorkdayExceptionCommand {
        Task<bool> ActivateWorkdayException(Guid WorkdayExceptionId);
    }
}
