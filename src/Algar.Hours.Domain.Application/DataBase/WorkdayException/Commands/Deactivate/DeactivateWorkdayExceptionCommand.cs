using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Deactivate
{
    public class DeactivateWorkdayExceptionCommand : IDeactivateWorkdayExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public DeactivateWorkdayExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<bool> DeactivateWorkdayException(Guid WorkdayExceptionId)
        {
            var activateWorkdayException = await _dataBaseService.WorkdayExceptionEntity
                .Where(r => r.IdWorkdayException == WorkdayExceptionId)
                .FirstOrDefaultAsync();

            if (activateWorkdayException != null) {
                activateWorkdayException.Active = false;
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
