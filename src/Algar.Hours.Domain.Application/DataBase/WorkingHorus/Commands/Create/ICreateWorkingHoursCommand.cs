using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create
{
    public interface ICreateWorkingHoursCommand
    {
        Task<bool> Execute(List<CreateWorkingHoursModel> model);

    }
}
