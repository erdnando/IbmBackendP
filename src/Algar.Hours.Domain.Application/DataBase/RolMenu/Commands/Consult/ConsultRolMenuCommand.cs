using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.RolMenu.Commands.Consult
{
    public class ConsultRolMenuCommand : IConsultRolMenuCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultRolMenuCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<RolMenuModel>> ListByIdRol(Guid RolId)
        {
            var entities = await _dataBaseService.RoleMenuEntity
                .Include(i => i.MenuEntity)
                .Where(rm => rm.RoleId == RolId)
                .ToListAsync();

            var models = _mapper.Map<List<RolMenuModel>>(entities);

            return models;
        }
    }
}
