using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Update
{
    public interface IUpdateUsuarioCommand
    {
        Task<bool> Update(CreateUserModelc createUserModel);
    }
}
