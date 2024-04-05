using Algar.Hours.Application.DataBase.UserException.Commands.Create;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
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
        private ICreateLogCommand _logCommand;

        public UpdateUsersExceptionCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;
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
            usersE.ReportType = model.ReportType;

            _dataBaseService.UsersExceptions.Update(usersE);
            await _dataBaseService.SaveAsync();

            await _logCommand.Log(model.UserId.ToString(), "Actualiza excepcion", model);

            return true;
        }
    }
}
