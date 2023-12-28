using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.Client.Commands.Consult
{
	public class ConsultClientCommand : IConsultClientCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultClientCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<ClientModel> Consult(Guid id)
        {
			var data = await _dataBaseService.CountryEntity.FirstOrDefaultAsync(d => d.IdCounty == id);
			var entity = _mapper.Map<ClientModel>(data);
			return entity;
		}

        public async Task<List<ClientModel>> List()
        {
            var dataList = await _dataBaseService.ClientEntity.ToListAsync();
            var list = _mapper.Map<List<ClientModel>>(dataList);
            return list;

        }
    }
}
