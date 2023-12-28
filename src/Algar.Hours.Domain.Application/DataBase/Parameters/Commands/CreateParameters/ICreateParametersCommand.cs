using Algar.Hours.Application.DataBase.Rol.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters
{
    public interface ICreateParametersCommand
    {
        Task<CreateParametersModel> Execute(CreateParametersModel model);
    }
}
