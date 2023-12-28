using Algar.Hours.Application.DataBase.Festivos.Create;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Festivos.Consult
{
    public class ConsultFestivosCommand : IConsultFestivosCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultFestivosCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<List<FestivosModel>> ListAll(Guid CountryId)
        {
            var entity = await _dataBaseService.FestivosEntity
                .Include(e => e.Country)
                .Where(e => e.CountryId == CountryId)
                .ToListAsync();
            var model = _mapper.Map<List<FestivosModel>>(entity);
            return model;
        }
    }
}
