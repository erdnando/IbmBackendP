using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Domain.Entities.User;
using Algar.Hours.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Update
{
    public class UpdateUsuarioCommand : IUpdateUsuarioCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public UpdateUsuarioCommand(IDataBaseService dataBaseService, IMapper mapper)
        { 
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<Boolean> Update(CreateUserModelc createUserModel)
        {
            var message = new BaseResponseModel();
            var usuario = await _dataBaseService.UserEntity.FirstOrDefaultAsync(u => u.IdUser == createUserModel.IdUser);
            if (usuario == null)
            {
                return false;
            }

            usuario.NameUser = createUserModel.NameUser;
            usuario.surnameUser = createUserModel.surnameUser;
            usuario.Email = createUserModel.Email;    
            usuario.EmployeeCode = createUserModel.EmployeeCode;
            usuario.Password = createUserModel.Password;
            usuario.RoleEntityId = createUserModel.RoleEntityId;
            usuario.CountryEntityId = createUserModel.CountryEntityId;

            _dataBaseService.UserEntity.Update(usuario);
            _dataBaseService.SaveAsync();

            return true;
        }
    }
}
