using Algar.Hours.Domain.Entities.Horario;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create
{
    public class CreateWorkingHoursCommand : ICreateWorkingHoursCommand
    {
        private readonly IDataBaseService _databaseService;
        private readonly IMapper _mapper;

        public CreateWorkingHoursCommand(IDataBaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;

        }
        public async Task<bool> Execute(List<CreateWorkingHoursModel> model)
        {
            try
            {
                var entityList = _mapper.Map<List<workinghoursEntity>>(model);

                foreach (var entity in entityList)
                {
                    entity.FechaWorking = DateTime.Now;
                    var existingEntity = await  _databaseService.workinghoursEntity
                        .FirstOrDefaultAsync(e => e.UserEntityId == entity.UserEntityId && e.week == entity.week && e.Ano == entity.Ano && e.Day == entity.Day);

                    if (existingEntity != null)
                    {
                        existingEntity.HoraInicio = entity.HoraInicio;
                        existingEntity.HoraFin = entity.HoraFin;

                        _databaseService.workinghoursEntity.Update(existingEntity);
                    }
                    else
                    {
                        entity.IdworkinghoursEntity = Guid.NewGuid();
                        entity.FechaCreacion = DateTime.Now;

                        _databaseService.workinghoursEntity.Add(entity);
                    }
                }

                await _databaseService.SaveAsync();

                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
            
        }


    }
}
