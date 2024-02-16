using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.AprobadorUsuario;
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
        private ICreateLogCommand _logCommand;
        public UpdateUsuarioCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        { 
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;
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
            await _dataBaseService.SaveAsync();

            //Validate if is necesary to delete this record, bacause it was appover before updating
            var aprobadorUsuariox = await _dataBaseService.AprobadorUsuario.FirstOrDefaultAsync(r => r.UserEntityId == createUserModel.IdUser);
            if (aprobadorUsuariox != null)
            {
                var entity = _mapper.Map<Domain.Entities.AprobadorUsuario.AprobadorUsuario>(aprobadorUsuariox);
                _dataBaseService.AprobadorUsuario.Remove(entity);
                await _dataBaseService.SaveAsync();
            }




            //validate if createUserModel.RoleEntityId; is an approver
            var rolUsuario = await _dataBaseService.RoleEntity.FirstOrDefaultAsync(r => r.IdRole == createUserModel.RoleEntityId);
            if(rolUsuario.NameRole == "Usuario Aprobador N1" || rolUsuario.NameRole == "Usuario Aprobador N2")
            {
                    //insert relation
                    AprobadorUsuarioModel au =new AprobadorUsuarioModel();

                    au.IdAprobadorUsuario= Guid.NewGuid();
                    au.UserEntityId= createUserModel.IdUser;
                    au.AprobadorId = rolUsuario.NameRole == "Usuario Aprobador N1" ? Guid.Parse("4666494f-60d3-42da-89fd-998b20fb40bf") : Guid.Parse("bdb101ed-5e37-4c74-86c8-112961948d7e");

                    var entity = _mapper.Map<Domain.Entities.AprobadorUsuario.AprobadorUsuario>(au);

                    _dataBaseService.AprobadorUsuario.Add(entity);
                    await _dataBaseService.SaveAsync();
            }
            await _logCommand.Log(createUserModel.idUserEntiyId, "Actualiza usuario", createUserModel);

            return true;
        }
    }
}
