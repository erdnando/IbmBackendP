using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.CreateUser
{
    public interface ICreateUserCommand
    {
        Task<CreateUserModelc> Execute(CreateUserModelc model);
    }
}
