using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
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
        private ICreateLogCommand _logCommand;
        public UpdateCountryCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
		{
			_dataBaseService = dataBaseService;
			_mapper = mapper;
            _logCommand = logCommand;
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
			await _dataBaseService.SaveAsync();

            await _logCommand.Log(model.idUserEntiyId, "Actualiza pais", model);

            return true;
		}
	}
}
