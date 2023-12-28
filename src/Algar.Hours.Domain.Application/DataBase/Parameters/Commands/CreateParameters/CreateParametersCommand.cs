using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.Rol;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Parameters.Commands.CreateParameters
{
    public class CreateParametersCommand : ICreateParametersCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateParametersCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<CreateParametersModel> Execute(CreateParametersModel model)
        {
            var entityConsul = _dataBaseService.ParametersEntity.Where(x => x.CountryEntityId == model.CountryEntityId && x.TypeHours  == model.TypeHours ).FirstOrDefault();

            if (entityConsul == null)
            {
                var entity = _mapper.Map<ParametersEntity>(model);
                entity.IdParametersEntity = Guid.NewGuid();
                await _dataBaseService.ParametersEntity.AddAsync(entity);
                await _dataBaseService.SaveAsync();
                return model;

            }
            else
            {
                return null;
            }
           
        }
    }
}
