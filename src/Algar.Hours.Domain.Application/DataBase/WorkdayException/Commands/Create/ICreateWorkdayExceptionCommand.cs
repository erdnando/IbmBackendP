using Algar.Hours.Domain.Entities.WorkdayException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Create
{
    public interface ICreateWorkdayExceptionCommand
    {
        Task<WorkdayExceptionEntity> Execute(WorkdayExceptionModelC createWorkdayException);
    }
}
