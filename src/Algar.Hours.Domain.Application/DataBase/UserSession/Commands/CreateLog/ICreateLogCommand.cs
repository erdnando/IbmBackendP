using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog
{
    public interface ICreateLogCommand
    {
        Task<bool> Log(string idUserEntiyId, string operation, object model);


    }
}
