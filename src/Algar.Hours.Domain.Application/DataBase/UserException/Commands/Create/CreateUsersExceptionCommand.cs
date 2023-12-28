using Algar.Hours.Domain.Entities.UsersExceptions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Create
{
    public class CreateUsersExceptionCommand : ICreateUsersExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateUsersExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<UsersExceptions> Execute(UsersExceptionsModelC createUsersException)
        {
            var entity = _mapper.Map<UsersExceptions>(createUsersException);
            entity.IdUsersExceptions = Guid.NewGuid();

            await _dataBaseService.UsersExceptions.AddAsync(entity);

            await _dataBaseService.SaveAsync();

            return entity;
        }
    }
}
