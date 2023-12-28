using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.ListGetUser
{
    public class ListGetUserCommand : IListGetUserCommand
    {
        private readonly IDataBaseService _databaseService;
        private readonly IMapper _mapper;

        public ListGetUserCommand(IDataBaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;

        }
        public async Task<List<ListGetUserModel>> Execute()
        {
            var entity = _databaseService.UserEntity.ToList();
            var model =  _mapper.Map<List<ListGetUserModel>>(entity);

            return model;

        }
    }
}
