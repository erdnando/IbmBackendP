using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using Algar.Hours.Domain.Entities.RolMenu;
using Algar.Hours.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Rol.Commands.Update
{
    public class UpdateRolCommand : IUpdateRolCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public UpdateRolCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<Boolean> Update(RolModel model)
        {
            var message = new BaseResponseModel();
            var rol = await _dataBaseService.RoleEntity.FirstOrDefaultAsync(r => r.IdRole == model.IdRole);

            if (rol == null)
            {
                return false;
            }

            rol.NameRole = model.NameRole;

            //delete
            
            foreach (var modellist in model.MenuEntity)
            {

                RoleMenuEntity rolMenu = new RoleMenuEntity();

                var entityMenu = _mapper.Map<MenuModelc>(modellist);

                rolMenu.IdRoleMenu = Guid.NewGuid();
                rolMenu.MenuEntityId = entityMenu.IdMenu;
                rolMenu.RoleId = rol.IdRole;
                _dataBaseService.RoleMenuEntity.Add(rolMenu);

               // _dataBaseService.RoleMenuEntity.Update(rolMenu);
               // _dataBaseService.SaveAsync();

            }

            _dataBaseService.RoleEntity.Update(rol);
            _dataBaseService.SaveAsync();

            return true;
        }
    }
}
