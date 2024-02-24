using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Activate
{
    public class ActivateWorkdayExceptionCommand : IActivateWorkdayExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public ActivateWorkdayExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<bool> ActivateWorkdayException(Guid WorkdayExceptionId)
        {
            var activateWorkdayException = await _dataBaseService.WorkdayExceptionEntity
                .Where(r => r.IdWorkdayException == WorkdayExceptionId)
                .FirstOrDefaultAsync();

            if (activateWorkdayException != null) {
                activateWorkdayException.Active = true;
                _dataBaseService.WorkdayExceptionEntity.Update(activateWorkdayException);
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
