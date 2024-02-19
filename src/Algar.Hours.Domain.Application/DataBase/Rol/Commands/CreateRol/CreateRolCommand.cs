using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
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
        private ICreateLogCommand _logCommand;

        public CreateRolCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;
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


            await _logCommand.Log(model.idUserEntiyId, "Crea Rol", model);

            return model;

        }
    }
}
