using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Domain.Entities.WorkdayException;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Create
{
    public class CreateWorkdayExceptionCommand : ICreateWorkdayExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateWorkdayExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<WorkdayExceptionEntity> Execute(WorkdayExceptionModelC createWorkdayException)
        {
            var workdayException = _mapper.Map<WorkdayExceptionEntity>(createWorkdayException);
            workdayException.IdWorkdayException = Guid.NewGuid();
            await _dataBaseService.WorkdayExceptionEntity.AddAsync(workdayException);

            await _dataBaseService.SaveAsync();

            return workdayException;
        }
    }
}
