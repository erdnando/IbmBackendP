using Algar.Hours.Domain.Entities.Festivos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Algar.Hours.Application.DataBase.Festivos.Create
{
    public class CreateFestivoCommand : ICreateFestivoCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateFestivoCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;

        }

        public async Task<Boolean> Execute(List<CreateFestivoModel> model)
        {
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    var entity = _mapper.Map<FestivosEntity>(item);

                    var existingEntity = await _dataBaseService.FestivosEntity
                        .FirstOrDefaultAsync(e => e.DiaFestivo == item.DiaFestivo && e.CountryId == entity.CountryId);

                    if (existingEntity != null)
                    {  
                        return false;
                    }

                    if (entity.IdFestivo == Guid.Empty)
                    {
                        entity.IdFestivo = Guid.NewGuid();
                    }

                    await _dataBaseService.FestivosEntity.AddAsync(entity);
                }
           
                await _dataBaseService.SaveAsync();
                return true;
            }

            return false;
        }
    }
}
