using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.Login;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Consult
{
    public class ConsultWorkingHoursCommand : IConsultWorkingHoursCommand
    {

        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultWorkingHoursCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public Task<CreateUserModel> Execute(LoginUserModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CreateWorkingHoursModel>> Consult(Guid idUser, string week, string ano)
        {
            var data = await _dataBaseService.workinghoursEntity
                .Where(e => e.UserEntityId == idUser && e.week == week && e.Ano == ano).ToListAsync();
            var entity = _mapper.Map<List<CreateWorkingHoursModel>>(data);
            return entity;
        }
    }
}
