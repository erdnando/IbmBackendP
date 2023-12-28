using Algar.Hours.Domain.Entities.Menu;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Menu.Commands.Update
{
    public class UpdateMenuCommand : IUpdateMenuCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public UpdateMenuCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

        }

        public async Task<bool> Execute(MenuModel menuModel)
        {
            var entity = _mapper.Map<MenuEntity>(menuModel);
            _dataBaseService.MenuEntity.Update(entity);
            await _dataBaseService.SaveAsync();
            return true;

        }


    }
}
