using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Consult
{
    public interface IConsultUserExceptionCommand
    {
        Task<List<UserExceptionModel>> List();
        Task<List<UserExceptionModel>> ConsultUsersByCountryId(Guid countryId);
    }
}
