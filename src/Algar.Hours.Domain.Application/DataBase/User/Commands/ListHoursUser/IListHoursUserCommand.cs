using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using Algar.Hours.Application.DataBase.HorusReport.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.ListHoursUser
{
    public interface IListHoursUserCommand
    {
        Task<List<ListHorursUserModel>> Execute(Guid IdClient);

    }
}
