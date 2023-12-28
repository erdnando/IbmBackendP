using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Create
{
    public interface ICreateAprobadorCommand
    {
        Task<AprobadorModel> Execute(AprobadorModel model);
    }
}
