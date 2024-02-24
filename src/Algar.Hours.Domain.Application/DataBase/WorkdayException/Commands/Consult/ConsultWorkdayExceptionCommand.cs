using Algar.Hours.Application.DataBase.Festivos.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Consult
{
    public class ConsultWorkdayExceptionCommand : IConsultWorkdayExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultWorkdayExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<WorkdayExceptionModel>> List()
        {
            var entity = await _dataBaseService.WorkdayExceptionEntity
                .Include(x => x.UserEntity)
                .ToListAsync();
            var model = _mapper.Map<List<WorkdayExceptionModel>>(entity);
            return model;
        }
    }
}
