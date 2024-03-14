using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.Festivos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Algar.Hours.Application.DataBase.Festivos.Create
{
    public class CreateFestivoCommand : ICreateFestivoCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private ICreateLogCommand _logCommand;

        public CreateFestivoCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;
        }

        public async Task<Boolean> Execute(List<CreateFestivoModel> model)
        {
            var idUserEntiyId = "";
            var savedDatesCount = 0;
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    /* var dia =item.DiaFestivo.Day;
                     var mes = item.DiaFestivo.Month;
                     var anio = item.DiaFestivo.Year;
                     var newDate = new DateTime(anio, dia, mes);

                     item.DiaFestivo = newDate;*/

                    
                    var entity = _mapper.Map<FestivosEntity>(item);

                    //var existingEntity = await _dataBaseService.FestivosEntity
                       // .FirstOrDefaultAsync(e => e.DiaFestivo == item.DiaFestivo && e.CountryId == entity.CountryId);
                    var existingEntity = await _dataBaseService.FestivosEntity
                        .FirstOrDefaultAsync(e => e.sDiaFestivo == item.sDiaFestivo && e.CountryId == entity.CountryId);

                    if (existingEntity != null) { continue; }

                    savedDatesCount++;

                    if (entity.IdFestivo == Guid.Empty)
                    {
                        entity.IdFestivo = Guid.NewGuid();
                    }

                    idUserEntiyId = item.idUserEntiyId;
                    // entity.DiaFestivo = DateTime.Parse(item.sDiaFestivo);
                    try
                    {
                        entity.DiaFestivo = DateTime.ParseExact($"{item.sDiaFestivo} 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {

                        
                    }
                    

                    await _dataBaseService.FestivosEntity.AddAsync(entity);
                }
           
                await _dataBaseService.SaveAsync();

                await _logCommand.Log(idUserEntiyId, "Modifica festivos", model);

            }

            return savedDatesCount > 0;
        }
    }
}
