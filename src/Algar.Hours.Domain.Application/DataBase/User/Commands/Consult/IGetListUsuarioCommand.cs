﻿using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.GetManager;
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
        Task<ManagerEmployeeModel> GetManagerByEmployeeCode(string _employeeCode);
        Task<List<CreateUserModel>> ConsultUsersByRoleId(Guid roleId);
        Task<List<CreateUserModel>> ConsultUsersByCountryId(Guid countryId);
        Task<Guid> GetUserIdByEmployeeCode(string employeeCode, Guid? countryId);
        Task<Guid> GetUserIdByID(string employeeId, Guid countryId);
        Task<UserEntity> GetByEmail(string EmailUser);
        Task<UserEntity> GetByUsuarioId(Guid Id);
        Task<UserEntity> GetByEmployeeCode(string employeeCode);
    }
}
