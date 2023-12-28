using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Country.Commands.Update
{
	public class UpdateCountryCommand : IUpdateCountryCommand
	{
		private readonly IDataBaseService _dataBaseService;
		private readonly IMapper _mapper;
		public UpdateCountryCommand(IDataBaseService dataBaseService, IMapper mapper)
		{
			_dataBaseService = dataBaseService;
			_mapper = mapper;
		}

		public async Task<Boolean> Update(CountryModel model)
		{
			var message = new BaseResponseModel();
			var cliente = await _dataBaseService.CountryEntity.FirstOrDefaultAsync(d => d.IdCounty == model.IdCounty);
			if (cliente == null)
			{
				return false;
			}

			cliente.NameCountry = model.NameCountry;

			_dataBaseService.CountryEntity.Update(cliente);
			_dataBaseService.SaveAsync();

			return true;
		}
	}
}
