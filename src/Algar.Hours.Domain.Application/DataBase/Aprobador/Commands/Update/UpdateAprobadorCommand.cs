using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using Algar.Hours.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Update
{
    public class UpdateAprobadorCommand : IUpdateAprobadorCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private ICreateLogCommand _logCommand;

        public UpdateAprobadorCommand(IDataBaseService dataBaseService, IMapper mapper, ICreateLogCommand logCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _logCommand = logCommand;

        }

        public async Task<bool> Update(AprobadorModel model)
        {
            var message = new BaseResponseModel();
            var aprobador = await _dataBaseService.Aprobador.FirstOrDefaultAsync(a => a.IdAprobador == model.IdAprobador);

            if (aprobador == null)
            {
                return false;
            }

            aprobador.Descripcion = model.Descripcion;
            aprobador.Nivel = model.Nivel;

            _dataBaseService.Aprobador.Update(aprobador);
            await _dataBaseService.SaveAsync();

            await _logCommand.Log(model.idUserEntiyId, "Actualiza aprobador", model);

            return true;
        }
    }
}
