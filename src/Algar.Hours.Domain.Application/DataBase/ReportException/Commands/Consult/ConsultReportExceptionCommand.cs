using Algar.Hours.Application.DataBase.Festivos.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Consult
{
    public class ConsultReportExceptionCommand : IConsultReportExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultReportExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<ReportExceptionModel>> List()
        {
            var entity = await _dataBaseService.ReportExceptionEntity
                .Include(x => x.UserEntity)
                .ToListAsync();
            var model = _mapper.Map<List<ReportExceptionModel>>(entity);
            return model;
        }
    }
}
