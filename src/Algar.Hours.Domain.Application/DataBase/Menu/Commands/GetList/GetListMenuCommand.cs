using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Menu.Commands.GetList
{
    public class GetListMenuCommand :IGetListMenuCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public GetListMenuCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

        }

        public async Task<List<MenuModel>> Execute()
        {
            var entity = _dataBaseService.MenuEntity.OrderBy(e => e.Order).ToList();
            var model = _mapper.Map<List<MenuModel>>(entity);
            return model;

        }


    }
}
