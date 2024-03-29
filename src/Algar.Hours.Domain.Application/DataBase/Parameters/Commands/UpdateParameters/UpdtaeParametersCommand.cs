﻿using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Entities.Parameters;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Parameters.Commands.UpdateParameters
{
    public class UpdtaeParametersCommand : IUpdtaeParametersCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private IEmailCommand _emailCommand;
        private IGetListUsuarioCommand _usuarioCommand;
        private ICreateLogCommand _logCommand;


        public UpdtaeParametersCommand(IDataBaseService dataBaseService, IMapper mapper, IEmailCommand emailCommand, IGetListUsuarioCommand usuarioCommand,ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _emailCommand = emailCommand;
            _usuarioCommand = usuarioCommand;
            _logCommand = logCommand;
        }

        public async Task<bool> Execute(UpdateParametersModel model)
        {
            var entity = _dataBaseService.ParametersEntity.Where(x => x.IdParametersEntity == model.IdParametersEntity && x.TypeHours == model.TypeHours).FirstOrDefault();

            if(entity != null) 
            {
                entity.TargetHourMonth = model.TargetHourMonth;
                entity.TargetHourWeek = model.TargetHourWeek;
                entity.TargetHourYear = model.TargetHourYear;
                entity.TargetTimeDay = model.TargetTimeDay;
                entity.TypeHours = model.TypeHours;
                entity.TypeLimits = model.TypeLimits;

                _dataBaseService.ParametersEntity.Update(entity);
                await _dataBaseService.SaveAsync();



                //---------------------------------------------
                await _logCommand.Log(model.EmpleadoUserEntityId.ToString(), "Modifica paramtros", model);
                _emailCommand.SendEmail(new EmailModel { To = (await _usuarioCommand.GetByUsuarioId(model.EmpleadoUserEntityId)).Email, Plantilla = "6" });
               //here





                return true;
            }
            return false;
           
        }
    }
}
