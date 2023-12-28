using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Domain.Entities.Client;
using AutoMapper;

namespace Algar.Hours.Application.DataBase.Client.Commands.Create
{
    public class CreateClientCommand : ICreateClientCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateClientCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<ClientModel> Execute(ClientModel model)
        {
            var client = _dataBaseService.ClientEntity.Where(x => x.NameClient == model.NameClient);

            if(client != null) 
            {
                model.IdClient = Guid.NewGuid();
                var entity = _mapper.Map<ClientEntity>(model);
                await _dataBaseService.ClientEntity.AddAsync(entity);
                await _dataBaseService.SaveAsync();
                return model;
            }
            else { return null;}
         
        }

    }
}
