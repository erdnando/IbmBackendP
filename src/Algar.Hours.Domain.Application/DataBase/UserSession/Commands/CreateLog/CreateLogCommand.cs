using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserSession.Commands.CreateUserSession
{
    public class CreateLogCommand : ICreateLogCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateLogCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

        }

        public async Task<CreateLogModel> Execute(CreateLogModel model)
        {
            var entity = _mapper.Map<UserSessionEntity>(model);
            if (entity.IdSession == Guid.Empty)
            {
                entity.IdSession = Guid.NewGuid();
            }
            await _dataBaseService.UserSessionEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();
            return model;
        }

        public async Task<UserSessionEntity> ExecuteId(CreateLogModel model)
        {
            var entity = _mapper.Map<UserSessionEntity>(model);
            if (entity.IdSession == Guid.Empty)
            {
                entity.IdSession = Guid.NewGuid();
            }
            await _dataBaseService.UserSessionEntity.AddAsync(entity);
            await _dataBaseService.SaveAsync();
            return entity;
        }
    }
}
