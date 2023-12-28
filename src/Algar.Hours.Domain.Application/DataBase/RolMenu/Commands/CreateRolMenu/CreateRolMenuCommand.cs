using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Domain.Entities.Rol;
using Algar.Hours.Domain.Entities.RolMenu;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.RolMenu.Commands.CreateRolMenu
{
    public class CreateRolMenuCommand : ICreateRolMenuCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateRolMenuCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }
        public async Task<CreateRolMenuModel> Execute(CreateRolMenuModel model)
        {
            var entity = _mapper.Map<RoleMenuEntity>(model);
            if (entity.IdRoleMenu == Guid.Empty)
            {
                entity.IdRoleMenu = Guid.NewGuid();
            }
            await _dataBaseService.RoleMenuEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();
            return model;

        }

    }
}
