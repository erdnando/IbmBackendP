using Algar.Hours.Domain.Entities.UsersExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Create
{
    public interface ICreateUsersExceptionCommand
    {
        Task<UsersExceptions> Execute(UsersExceptionsModelC createUsersException);
    }
}
