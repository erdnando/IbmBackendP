using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Delete
{
    public class DeleteReportExceptionCommand : IDeleteReportExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public DeleteReportExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<bool> DeleteReportException(Guid ReportExceptionId)
        {
            var DeleteReportException = await _dataBaseService.ReportExceptionEntity
                .Where(r => r.IdReportException == ReportExceptionId)
                .FirstOrDefaultAsync();

            if (DeleteReportException != null)
            {
                _dataBaseService.ReportExceptionEntity.Remove(DeleteReportException);
                await _dataBaseService.SaveAsync();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
