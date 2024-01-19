using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Domain.Entities.Aprobador;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Consult
{

    public class ConsultAprobadorCommand : IConsultAprobadorCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultAprobadorCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }
        public async Task<List<AprobadorUsuarioModel>> Execute(int nivel)
        {
           var entity =  _dataBaseService.AprobadorUsuario
               .Include(e=> e.UserEntity)
               .Include(e=> e.Aprobador)
               .Where(x=> x.Aprobador.Nivel == nivel   ).ToList();

            var moldeuser = _mapper.Map<List<AprobadorUsuarioModel>>(entity);
            return moldeuser;

        }       

        public async Task<List<AprobadorModel>> ListAll()
        {
            var entity = await _dataBaseService.Aprobador.ToListAsync();
            var model = _mapper.Map<List<AprobadorModel>>(entity);
            return model;
        }

        public async Task<List<AprobadorModel>> ConsultById(Guid AprobadorId)
        {
            var aprobadorById = await _dataBaseService.Aprobador
                .Where(a => a.IdAprobador == AprobadorId)
                .ToListAsync();

            var aprobadorModel = _mapper.Map<List<AprobadorModel>>(aprobadorById);
            return aprobadorModel;
        }
    }
}
