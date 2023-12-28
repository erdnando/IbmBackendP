using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using Algar.Hours.Domain.Entities.Menu;
using Algar.Hours.Domain.Entities.Rol;
using Algar.Hours.Domain.Entities.RolMenu;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Algar.Hours.Application.DataBase.Rol.Commands.CreateRol
{
    public class CreateRolCommand : ICreateRolCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateRolCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<CreateRolModel> Execute(CreateRolModel model)
        {
            var entity = _mapper.Map<RoleEntity>(model);
            if (entity.IdRole == Guid.Empty)
            {
                entity.IdRole = Guid.NewGuid();
            }
            await _dataBaseService.RoleEntity.AddAsync(entity);

            foreach (var modellist in model.MenuEntity)
            {
                RoleMenuEntity rolMenu = new RoleMenuEntity();

                var entityMenu = _mapper.Map<MenuModelc>(modellist);

                rolMenu.IdRoleMenu = Guid.NewGuid();
                rolMenu.MenuEntityId = entityMenu.IdMenu;
                rolMenu.RoleId = entity.IdRole;
                _dataBaseService.RoleMenuEntity.Add(rolMenu);
            }

            await _dataBaseService.SaveAsync();
            return model;

        }
    }
}
