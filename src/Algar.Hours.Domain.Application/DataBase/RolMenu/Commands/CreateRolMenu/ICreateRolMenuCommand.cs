using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.RolMenu.Commands.CreateRolMenu
{
    public interface ICreateRolMenuCommand
    {
        Task<CreateRolMenuModel> Execute(CreateRolMenuModel model);

    }
}
