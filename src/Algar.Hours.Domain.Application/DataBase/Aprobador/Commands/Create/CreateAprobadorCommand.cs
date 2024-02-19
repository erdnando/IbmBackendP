using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.Aprobador;
using Algar.Hours.Domain.Entities.Rol;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Create
{
    public class CreateAprobadorCommand : ICreateAprobadorCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private ICreateLogCommand _logCommand;

        public CreateAprobadorCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;
        }

        public async Task<AprobadorModel> Execute(AprobadorModel model)
        {
            var entity = _mapper.Map<Domain.Entities.Aprobador.Aprobador>(model);
            if (entity.IdAprobador == Guid.Empty)
            {
                entity.IdAprobador = Guid.NewGuid();
            }
             _dataBaseService.Aprobador.AddAsync(entity);
            await _dataBaseService.SaveAsync();

            await _logCommand.Log(model.idUserEntiyId, "Crea aprobador", model);

            return model;

        }
    }
}
