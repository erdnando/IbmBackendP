using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
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
        private ICreateLogCommand _logCommand;

        public CreateUsersExceptionCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;
        }

        public async Task<UsersExceptions> Execute(UsersExceptionsModelC createUsersException)
        {
            UsersExceptions newUserTemp = new() {
                UserId = createUsersException.UserId,
                AssignedUserId = createUsersException.AssignedUserId,
                Description = createUsersException.Description,
                horas= createUsersException.horas,
                IdUsersExceptions = Guid.NewGuid(),
                StartDate = createUsersException.StartDate,
            };
            //var entity = _mapper.Map<UsersExceptions>(createUsersException);
            //entity.IdUsersExceptions = Guid.NewGuid();

            await _dataBaseService.UsersExceptions.AddAsync(newUserTemp);

            await _dataBaseService.SaveAsync();


            await _logCommand.Log(createUsersException.UserId.ToString(), "Crea excepcion", createUsersException);

            return newUserTemp;
        }
    }
}
