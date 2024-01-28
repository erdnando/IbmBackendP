using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.GetManager;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NodaTime.TimeZones.TzdbZone1970Location;

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

        public async Task<ManagerEmployeeModel> GetManagerByEmployeeCode(string _employeeCode)
        {
            var data =  _dataBaseService.UserManagerEntity.Where(u => u.EmployeeCode==_employeeCode).FirstOrDefault();
            ManagerEmployeeModel newRow = new() { 
                ManagerName= data != null ? data.ManagerName:"Sin Información",
                ManagerEmail=data!= null ? data.ManagerEmail:"Sin Información"
            };

            var entity = _mapper.Map<ManagerEmployeeModel>(newRow);
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

        public async Task<Guid> GetUserIdByID(string employeeId, Guid countryId)
        {
            var userEntity = await _dataBaseService.UserEntity
                .FirstOrDefaultAsync(u => u.IdUser == Guid.Parse(employeeId) && u.CountryEntityId == countryId);
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

        public async Task<List<CreateUserModel>> ConsultUsersByCountryId(Guid countryId)
        {
            var userByCountryId = await _dataBaseService.UserEntity
                .Include(u => u.RoleEntity)
                .Include(u => u.CountryEntity)
                .Where(u => u.CountryEntityId == countryId)
                .ToListAsync();

            var userModels = _mapper.Map<List<CreateUserModel>>(userByCountryId);
            return userModels;
        }

        public async Task<UserEntity> GetByEmail(string EmailUser)
        {
            var userEntity = await _dataBaseService.UserEntity.FirstOrDefaultAsync(u => u.Email.Trim().ToUpper() == EmailUser.Trim().ToUpper());
            
            return userEntity;
        }

        public async Task<UserEntity> GetByUsuarioId(Guid Id)
        {
            var userEntity = await _dataBaseService.UserEntity
                 .FirstOrDefaultAsync(u => u.IdUser == Id);
            
            return userEntity;
        }
    }
}
