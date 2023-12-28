using Algar.Hours.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.Client.Commands.Update
{
	public class UpdateClientCommand : IUpdateClientCommand
	{
		private readonly IDataBaseService _dataBaseService;
		private readonly IMapper _mapper;
		public UpdateClientCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
			_dataBaseService = dataBaseService;
			_mapper = mapper;
		}

		public async Task<Boolean> Update(ClientModel model)
		{
			
			var cliente = await _dataBaseService.ClientEntity.FirstOrDefaultAsync(d => d.IdClient == model.IdClient);
			if (cliente == null)
			{				
				return false;
			}

			cliente.NameClient = model.NameClient;
			_dataBaseService.ClientEntity.Update(cliente);
			_dataBaseService.SaveAsync();

			return true;
		}
	}
}
