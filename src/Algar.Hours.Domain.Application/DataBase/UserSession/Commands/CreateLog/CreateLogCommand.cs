using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using NetTopologySuite.Operation.Valid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserSession.Commands.CreateUserSession
{
    public class CreateLogCommand : ICreateLogCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateLogCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

        }

        public async Task<bool> Log(string idUserEntiyId, string operation, object model)
        {
            try
            {
                var aux = new UserSessionEntity();

                aux.IdSession = Guid.NewGuid();
                aux.LogDateEvent = DateTime.Now;

                aux.operation = operation;
                aux.parameters= JsonConvert.SerializeObject(model);
                aux.parameters = aux.parameters.Length > 3000 ? aux.parameters.Substring(0, 3000) : aux.parameters;
                aux.sUserEntityId= idUserEntiyId;

                 _dataBaseService.UserSessionEntity.Add(aux);
                await _dataBaseService.SaveAsync();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }

       
    }
}
