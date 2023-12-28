using Algar.Hours.Application.DataBase.UserException.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Update
{
    public interface IUpdateUsersExceptionCommand
    {
        Task<Boolean> Update(UsersExceptionsModelC model);
    }
}
