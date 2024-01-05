using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Consult
{
    public interface IGetListUsuarioCommand
    {
        Task<List<CreateUserModel>> List();
        Task<CreateUserModelc> Consult(Guid Id);
        Task<List<CreateUserModel>> ConsultUsersByRoleId(Guid roleId);
        Task<Guid> GetUserIdByEmployeeCode(string employeeCode, Guid countryId);
        Task<UserEntity> GetByEmail(string EmailUser);
    }
}
