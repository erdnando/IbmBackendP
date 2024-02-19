using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Application.DataBase.ReportException.Commands.Create;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Update
{
    public class UpdateReportExceptionCommand : IUpdateReportExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public UpdateReportExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<Boolean> Update(ReportExceptionModel model)
        {
            var reportE = await _dataBaseService.ReportExceptionEntity.FirstOrDefaultAsync(r => r.IdReportException == model.IdReportException);

            if (reportE == null)
            {
                return false;
            }

            reportE.Report = model.Report;

            _dataBaseService.ReportExceptionEntity.Update(reportE);
            _dataBaseService.SaveAsync();

            return true;
        }
    }
}
