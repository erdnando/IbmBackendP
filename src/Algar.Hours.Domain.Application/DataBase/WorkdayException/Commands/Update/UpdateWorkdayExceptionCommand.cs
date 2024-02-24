using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Application.DataBase.ReportException.Commands.Create;
using Algar.Hours.Application.DataBase.WorkdayException.Commands.Consult;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Update
{
    public class UpdateWorkdayExceptionCommand : IUpdateWorkdayExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public UpdateWorkdayExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<Boolean> Update(WorkdayExceptionModel model)
        {
            var reportE = await _dataBaseService.WorkdayExceptionEntity.FirstOrDefaultAsync(r => r.IdWorkdayException == model.IdWorkdayException);

            if (reportE == null) return false;

            /*reportE.Report = model.Report;*/

            _dataBaseService.WorkdayExceptionEntity.Update(reportE);
            _dataBaseService.SaveAsync();

            return true;
        }
    }
}
