using Algar.Hours.Domain.Entities.Menu;
using AutoMapper;

namespace Algar.Hours.Application.DataBase.Menu.Commands
{
    public class CreateMenuCommand : ICreateMenuCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateMenuCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<MenuModel> Execute(MenuModel model)
        {
            var entity = _mapper.Map<MenuEntity>(model);
            if (entity.IdMenu == Guid.Empty)
            {
                entity.IdMenu = Guid.NewGuid();
            }

            entity.Icon = "fal fa-box-open";
            entity.Path = "profiles";
            await _dataBaseService.MenuEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();
            return model;
        }
    }
}
