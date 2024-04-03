using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Consult
{
    public interface IConsultAprobadorCommand
    {
        Task<List<AprobadorUsuarioModel>> Execute(int nivel, Guid idPais);
        Task<List<AprobadorModel>> ListAll();
        Task<List<AprobadorModel>> ConsultById(Guid AprobadorId);
    }
}
