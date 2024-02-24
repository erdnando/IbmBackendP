using Algar.Hours.Application.DataBase.WorkdayException.Commands.Consult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Update
{
    public interface IUpdateWorkdayExceptionCommand
    {
        Task<Boolean> Update(WorkdayExceptionModel model);
    }
}
