using Algar.Hours.Domain.Entities.Parameters;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Parameters.Commands.ConsulParameters
{
    public class ConsultParametersCommand : IConsultParametersCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultParametersCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }
        public async Task<List<ConsultParametersModel>> Execute(Guid paisId)
        {
            var entity =  _dataBaseService.ParametersEntity.Where(x => x.CountryEntityId == paisId).ToList();
            var model =   _mapper.Map<List<ConsultParametersModel>>(entity);
            return model;

        }
    }
}
