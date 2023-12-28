using Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Parameters.Commands.UpdateParameters
{
    public interface IUpdtaeParametersCommand
    {
        Task<bool> Execute(UpdateParametersModel model);
    }
}
