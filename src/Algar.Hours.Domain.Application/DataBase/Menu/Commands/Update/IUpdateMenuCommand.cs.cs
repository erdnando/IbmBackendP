using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Menu.Commands.Update
{
    public interface IUpdateMenuCommand
    {
        Task<bool> Execute(MenuModel menuModel);
    }
}
