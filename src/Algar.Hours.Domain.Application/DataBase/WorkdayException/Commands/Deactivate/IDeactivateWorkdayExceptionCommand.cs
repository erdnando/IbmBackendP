using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Deactivate
{
    public interface IDeactivateWorkdayExceptionCommand {
        Task<bool> DeactivateWorkdayException(Guid WorkdayExceptionId);
    }
}
