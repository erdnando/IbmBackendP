using Algar.Hours.Application.DataBase.HoursReport.Commands.Consult;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Consult
{
	public class ConsultHorusReportCommand : IConsultHorusReportCommand
	{
		private readonly IDataBaseService _dataBaseService;
		private readonly IMapper _mapper;

		public ConsultHorusReportCommand(IDataBaseService dataBaseService, IMapper mapper)
		{
			_dataBaseService = dataBaseService;
			_mapper = mapper;
		}

		public async  Task<HorusReportModel> Consult(Guid id)
		{
			var data =  _dataBaseService.HorusReportEntity.Where(d => d.ClientEntityId == id);
			var entity = _mapper.Map<HorusReportModel>(data);
			return entity;
		}

		public async Task<List<ConsultMoldeHosrusReportModel>> List()
		{
			var dataList = await _dataBaseService.HorusReportEntity
				.Include(x=>x.UserEntity)
				.ThenInclude(c=>c.CountryEntity).OrderByDescending(x => x.NumberReport).ThenByDescending(y => y.Semana)
                .ToListAsync();

			var list = _mapper.Map<List<ConsultMoldeHosrusReportModel>>(dataList);


            /*foreach(var item in list) 
			{
				var country = _dataBaseService.CountryEntity.Where(x => x.IdCounty == item.UserEntity.CountryEntityId).FirstOrDefault();
				if(country != null)
				{

					item.countryEntity = country;

				}

            }*/

            //.ThenInclude(c=>c.CountryEntity).OrderByDescending(x => DateTime.ParseExact(x.StrStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture ) ) 

            return list;

		}
	}
}
