using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Delete
{
    public class DeleteWorkdayExceptionCommand : IDeleteWorkdayExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public DeleteWorkdayExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<bool> DeleteWorkdayException(Guid WorkdayExceptionId)
        {
            var DeleteWorkdayException = await _dataBaseService.WorkdayExceptionEntity
                .Where(r => r.IdWorkdayException == WorkdayExceptionId)
                .FirstOrDefaultAsync();

            if (DeleteWorkdayException != null) {
                _dataBaseService.WorkdayExceptionEntity.Remove(DeleteWorkdayException);
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
