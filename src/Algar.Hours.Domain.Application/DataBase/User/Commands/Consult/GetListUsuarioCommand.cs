using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Consult
{
    public class GetListUsuarioCommand : IGetListUsuarioCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public GetListUsuarioCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<CreateUserModel>> List()
        {
            var entities = await _dataBaseService.UserEntity
                .Include(u => u.RoleEntity) 
                .Include(u => u.CountryEntity) 
                .ToListAsync();

            var models = _mapper.Map<List<CreateUserModel>>(entities);
            return models;
        }

        public async Task<CreateUserModelc> Consult(Guid Id)
        {
            var data = await _dataBaseService.UserEntity.Where(u => u.IdUser == Id).FirstOrDefaultAsync();
            var entity = _mapper.Map<CreateUserModelc>(data);
            return entity;
        }

        public async Task<Guid> GetUserIdByEmployeeCode(string employeeCode, Guid countryId)
        {
            var userEntity = await _dataBaseService.UserEntity
                .FirstOrDefaultAsync(u => u.EmployeeCode == employeeCode && u.CountryEntityId == countryId);
            if (userEntity == null)
            {
               return Guid.Empty;
            }
            return userEntity.IdUser;
        }

        public async Task<List<CreateUserModel>> ConsultUsersByRoleId(Guid roleId)
        {
            var userbyrolid = await _dataBaseService.UserEntity
                .Include (u => u.RoleEntity)
                .Include(u => u.CountryEntity)
                .Where(u => u.RoleEntityId == roleId)
                .ToListAsync();

            var userModels = _mapper.Map<List<CreateUserModel>>(userbyrolid);
            return userModels;
        }
    }
}
