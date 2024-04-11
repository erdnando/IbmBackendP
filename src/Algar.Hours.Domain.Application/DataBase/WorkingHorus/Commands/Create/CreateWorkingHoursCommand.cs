using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
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

        private IEmailCommand _emailCommand;
        private IGetListUsuarioCommand _usuarioCommand;
        private string[] weekDays = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

        public CreateWorkingHoursCommand(IDataBaseService databaseService, IMapper mapper, IEmailCommand emailCommand, IGetListUsuarioCommand usuarioCommand)
        {
            _databaseService = databaseService;
            _mapper = mapper;

            _emailCommand = emailCommand;
            _usuarioCommand = usuarioCommand;

        }
        public async Task<bool> Execute(List<CreateWorkingHoursModel> model)
        {
            try
            {
                var entityList = _mapper.Map<List<workinghoursEntity>>(model);
                List<workinghoursEntity> schedules = new();
                foreach (var entity in entityList)
                {
                    schedules.AddRange(_databaseService.workinghoursEntity.Where(e => e.UserEntityId == entity.UserEntityId && e.week == entity.week && e.Ano == entity.Ano).ToList());
                }

                if (schedules.Count() > 0) 
                {
                    _databaseService.workinghoursEntity.RemoveRange(schedules);
                    await _databaseService.SaveAsync();
                }

                    foreach (var entity in entityList)
                {
                    TimeSpan startTime = TimeSpan.Parse(entity.HoraInicio);
                    TimeSpan endTime = TimeSpan.Parse(entity.HoraFin);
                    if (startTime >= endTime) continue;

                    entity.FechaWorking = entity.FechaWorking; 
                    var weekDayNumber = Convert.ToUInt32(entity.FechaWorking.DayOfWeek.ToString("d"));
                    entity.Day = this.weekDays[weekDayNumber];
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
                        //lo recrea
                        entity.IdworkinghoursEntity = Guid.NewGuid();
                        entity.FechaCreacion = DateTimeOffset.Now;


                        _databaseService.workinghoursEntity.Add(entity);
                    }
                }

                await _databaseService.SaveAsync();
                //------------------------------------------------------------------------
                if (entityList.Count > 0)
                {
                   
                        _emailCommand.SendEmail(new EmailModel{To = (await _usuarioCommand.GetByUsuarioId(entityList[0].UserEntityId)).Email,Plantilla = "6"});
                   
                }
                

                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
            
        }


    }
}
