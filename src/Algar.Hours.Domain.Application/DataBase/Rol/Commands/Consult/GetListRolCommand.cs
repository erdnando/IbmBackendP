using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Rol.Commands.Consult
{
    public class GetListRolCommand : IGetListRolCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public GetListRolCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<RolModel>> List()
        {
            var entity = await _dataBaseService.RoleEntity.ToListAsync();
            var model = _mapper.Map<List<RolModel>>(entity);
            return model;
        }
    }
}
