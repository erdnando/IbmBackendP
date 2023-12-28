using Algar.Hours.Application.DataBase.Festivos.Consult;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Consult
{
    public class ConsultUserExceptionCommand : IConsultUserExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultUserExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<UserExceptionModel>> List()
        {
            var entity = await _dataBaseService.UsersExceptions
                .Include(e => e.User)       
                .ToListAsync();
            var model = _mapper.Map<List<UserExceptionModel>>(entity);
            return model;
        }
    }
}
