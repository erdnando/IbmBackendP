using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.CreateUser
{
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateUserCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

        }

        public async Task<CreateUserModelc> Execute(CreateUserModelc model)
        {
            var entity = _mapper.Map<UserEntity>(model);
            if (entity.IdUser == Guid.Empty)
            {
                entity.IdUser = Guid.NewGuid();
            }
            await _dataBaseService.UserEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();
            return model;
        }

        public async Task<UserEntity> ExecuteId(CreateUserModelc model)
        {
            var entity = _mapper.Map<UserEntity>(model);
            if (entity.IdUser == Guid.Empty)
            {
                entity.IdUser = Guid.NewGuid();
            }
            await _dataBaseService.UserEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();
            return entity;
        }
    }
}
