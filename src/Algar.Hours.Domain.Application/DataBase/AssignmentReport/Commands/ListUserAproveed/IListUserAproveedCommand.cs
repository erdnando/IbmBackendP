using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed
{
    public interface IListUserAproveedCommand
    {
        List<ListAproveedModel> Execute(Guid UserId);

    }
}
