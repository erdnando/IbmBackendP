using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Rol;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Country.Commands.Create
{
    public class CreateCountryCommand : ICreateCountryCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private ICreateLogCommand _logCommand;

        public CreateCountryCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;

        }

        public async Task<CountryModel> Execute(CountryModel model)
        {
            if (!string.IsNullOrEmpty(model.NameCountry))
            {
                var entity = _mapper.Map<CountryEntity>(model);
                if (entity.IdCounty == Guid.Empty)
                {
                    entity.IdCounty = Guid.NewGuid();
                }
                 _dataBaseService.CountryEntity.AddAsync(entity);
                await _dataBaseService.SaveAsync();

                await _logCommand.Log(model.idUserEntiyId, "Crea pais", model);
                return model;
            }
            else
            {
                return null;
            }
           
        }
    }
}
