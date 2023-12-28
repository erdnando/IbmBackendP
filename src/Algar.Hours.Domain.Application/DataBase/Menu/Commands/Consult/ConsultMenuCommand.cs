using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Menu.Commands.Consult
{
    public class ConsultMenuCommand :IConsultMenuCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultMenuCommand(IDataBaseService dataBaseService, IMapper mapper) 
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

        }

        public async Task<MenuModel> Execute(Guid Idmenu)
        {
            var entity = _dataBaseService.MenuEntity.Where(x=> x.IdMenu == Idmenu).FirstOrDefault();
            var model = _mapper.Map<MenuModel>(entity);
            return model;

        }


    }
}
