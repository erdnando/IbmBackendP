using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.RolMenu.Commands.Delete
{
    public class DeleteRolMenuCommand : IDeleteRolMenuCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public DeleteRolMenuCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<bool> DeleteRolMenu(Guid RolMenuId)
        {
            var DeleteRol = await _dataBaseService.RoleMenuEntity
                .Where(r => r.IdRoleMenu == RolMenuId)
                .FirstOrDefaultAsync();

            if (DeleteRol != null)
            {
                _dataBaseService.RoleMenuEntity.Remove(DeleteRol);
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
