using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Rol.Commands.Update
{
    public interface IUpdateRolCommand
    {
        Task<Boolean> Update(RolModel model);
    }
}
