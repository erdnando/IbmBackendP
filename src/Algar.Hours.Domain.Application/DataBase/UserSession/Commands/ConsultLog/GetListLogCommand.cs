using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.GetManager;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NodaTime.TimeZones.TzdbZone1970Location;

namespace Algar.Hours.Application.DataBase.UserSession.Commands.Consult
{
    public class GetListLogCommand : IGetListLogCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public GetListLogCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<CreateLogModel>> List()
        {
            var entities = await _dataBaseService.UserEntity
                .Include(u => u.RoleEntity) 
                .Include(u => u.CountryEntity) 
                .ToListAsync();

            var models = _mapper.Map<List<CreateLogModel>>(entities);
            return models;
        }

        public async Task<CreateLogModel> Consult(Guid Id)
        {
            var data = await _dataBaseService.UserSessionEntity.Where(u => u.IdSession == Id).FirstOrDefaultAsync();
            var entity = _mapper.Map<CreateLogModel>(data);
            return entity;
        }

       

    }
}
