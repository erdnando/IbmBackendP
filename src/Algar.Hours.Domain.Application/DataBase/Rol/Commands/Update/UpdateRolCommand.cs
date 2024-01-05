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
            var rolMenuEnt = await _dataBaseService.RoleMenuEntity.Where(r => r.RoleId == rol.IdRole).ToListAsync();
            if (rolMenuEnt.Count > 0)
            {
                _dataBaseService.RoleMenuEntity.RemoveRange(rolMenuEnt);
            }

            if (rol == null)
            {
                return false;
            }

            foreach (var modellist in model.MenuEntity)
            {
                var entityMenu = _mapper.Map<MenuModelc>(modellist);
                RoleMenuEntity rolMenu = new()
                {
                    IdRoleMenu = Guid.NewGuid(),
                    MenuEntityId = entityMenu.IdMenu,
                    RoleId = rol.IdRole,
                };
                _dataBaseService.RoleMenuEntity.Add(rolMenu);
            }
            await _dataBaseService.SaveAsync();
            return true;
        }
    }
}
