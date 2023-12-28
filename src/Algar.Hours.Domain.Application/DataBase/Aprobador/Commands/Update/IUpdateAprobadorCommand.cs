using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Update
{
    public interface IUpdateAprobadorCommand
    {
        Task<bool> Update(AprobadorModel model);
    }
}
