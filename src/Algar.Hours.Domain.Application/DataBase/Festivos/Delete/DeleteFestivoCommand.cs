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

namespace Algar.Hours.Application.DataBase.Festivos.Delete
{
    public class DeleteFestivoCommand : IDeleteFestivoCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public DeleteFestivoCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<Boolean> Delete(CreateFestivoModel model)
        {
            var message = new BaseResponseModel();
            var festivo = await _dataBaseService.FestivosEntity.FirstOrDefaultAsync(f => f.IdFestivo == model.IdFestivo);
            if (festivo == null) 
            { 
                return false;
            }

            if (festivo != null)
            {
                _dataBaseService.FestivosEntity.Remove(festivo);
                await _dataBaseService.SaveAsync();

                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
