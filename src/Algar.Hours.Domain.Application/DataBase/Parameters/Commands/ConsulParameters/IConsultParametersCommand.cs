using Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Parameters.Commands.ConsulParameters
{
    public interface IConsultParametersCommand
    {
        Task<List<ConsultParametersModel>> Execute(Guid paisId);

    }
}
