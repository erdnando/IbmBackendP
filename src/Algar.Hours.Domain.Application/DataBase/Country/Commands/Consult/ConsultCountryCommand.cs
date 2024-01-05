using Algar.Hours.Domain.Entities.Client;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Algar.Hours.Application.Enums.Enums;

namespace Algar.Hours.Application.DataBase.Country.Commands.Consult
{
	public class ConsultCountryCommand : IConsultCountryCommand
	{
		private readonly IDataBaseService _dataBaseService;
		private readonly IMapper _mapper;

		public ConsultCountryCommand(IDataBaseService dataBaseService, IMapper mapper)
		{
			_dataBaseService = dataBaseService;
			_mapper = mapper;
		}

		public async Task<CountryModel> Consult(Guid id)
		{
			var data = await _dataBaseService.CountryEntity.FirstOrDefaultAsync(d => d.IdCounty == id);
			var entity = _mapper.Map<CountryModel>(data);			
			return entity;
		}

        public async Task<CountryModel> ConsultIdbyCode(string codigo)
        {
            var data = await _dataBaseService.CountryEntity.FirstOrDefaultAsync(d => d.CodigoPais == codigo);
            var entity = _mapper.Map<CountryModel>(data);
            return entity;
        }

        public async Task<Guid> ConsultIdbyName(string pais)
        {
            var data = await _dataBaseService.CountryEntity.FirstOrDefaultAsync(d => d.NameCountry == pais);
            var entity = _mapper.Map<CountryModel>(data);
            return entity.IdCounty;
        }
        

        public async Task<List<CountryModel>> List()
		{
			var dataList = await _dataBaseService.CountryEntity.ToListAsync();
			var list = _mapper.Map<List<CountryModel>>(dataList);
			return list;
		}
	}
}
