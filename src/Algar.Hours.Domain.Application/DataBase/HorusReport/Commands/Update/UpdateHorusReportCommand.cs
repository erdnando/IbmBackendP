using Algar.Hours.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Update
{
	public class UpdateHorusReportCommand : IUpdateHorusReportCommand
	{
		private readonly IDataBaseService _dataBaseService;
		private readonly IMapper _mapper;
		public UpdateHorusReportCommand(IDataBaseService dataBaseService, IMapper mapper)
		{
			_dataBaseService = dataBaseService;
			_mapper = mapper;
		}

		public async Task<Boolean> Update(HorusReportModel model)
		{
			var message = new BaseResponseModel();
			var cliente = await _dataBaseService.HorusReportEntity.FirstOrDefaultAsync(d => d.IdHorusReport == model.IdHorusReport);
			if (cliente == null)
			{
				return false;
			}

			cliente.UserEntityId = model.UserEntityId;
			cliente.StrStartDate = model.StrStartDate;
			cliente.StartTime = model.StartTime;
			cliente.EndTime = model.EndTime;
			cliente.ClientEntityId = model.ClientEntityId;
			cliente.strCreationDate = model.strCreationDate;
			
			_dataBaseService.HorusReportEntity.Update(cliente);
			_dataBaseService.SaveAsync();

			return true;
		}
	}
}
