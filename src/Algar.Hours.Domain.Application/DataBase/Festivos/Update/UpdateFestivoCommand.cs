using Algar.Hours.Application.DataBase.Festivos.Consult;
using Algar.Hours.Application.DataBase.Festivos.Create;
using Algar.Hours.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Festivos.Update
{
    public class UpdateFestivoCommand : IUpdateFestivoCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public UpdateFestivoCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<Boolean> Update(CreateFestivoModel model)
        {
            var message = new BaseResponseModel();
            var festivo = await _dataBaseService.FestivosEntity.FirstOrDefaultAsync(f => f.IdFestivo == model.IdFestivo);
            if (festivo == null) 
            { 
                return false;
            }

            festivo.Descripcion = model.Descripcion;
            festivo.DiaFestivo = model.DiaFestivo;
            festivo.ano = model.ano; 
            festivo.CountryId = model.CountryId;

            _dataBaseService.FestivosEntity.Update(festivo);
            _dataBaseService.SaveAsync();

            return true;
        }
    }
}
