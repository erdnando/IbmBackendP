using Algar.Hours.Application.DataBase.UserException.Commands.Create;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Update
{
    public class UpdateUsersExceptionCommand : IUpdateUsersExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public UpdateUsersExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<Boolean> Update(UsersExceptionsModelC model)
        {
            var usersE = await _dataBaseService.UsersExceptions.FirstOrDefaultAsync(r => r.IdUsersExceptions == model.IdUsersExceptions);

            if (usersE == null)
            {
                return false;
            }

            usersE.UserId = model.UserId;
            usersE.AssignedUserId = model.AssignedUserId;
            usersE.StartDate = model.StartDate;
            usersE.horas = model.horas;
            usersE.Description = model.Description;

            _dataBaseService.UsersExceptions.Update(usersE);
            _dataBaseService.SaveAsync();

            return true;
        }
    }
}
