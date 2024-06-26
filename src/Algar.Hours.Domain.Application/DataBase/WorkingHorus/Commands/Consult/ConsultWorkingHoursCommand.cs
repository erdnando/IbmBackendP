using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Application.DataBase.User.Commands.Login;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Consult
{
    public class ConsultWorkingHoursCommand : IConsultWorkingHoursCommand
    {

        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private IEmailCommand _emailCommand;
        private IGetListUsuarioCommand _usuarioCommand;

        public ConsultWorkingHoursCommand(IDataBaseService dataBaseService, IMapper mapper, IEmailCommand emailCommand, IGetListUsuarioCommand usuarioCommand)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _emailCommand = emailCommand;
            _usuarioCommand= usuarioCommand;
        }

        public Task<CreateUserModel> Execute(LoginUserModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CreateWorkingHoursModel>> Consult(Guid idUser, DateTimeOffset dateTime)
        {
            var dateTimeInicioSemana = System.DateTime.ParseExact($"{dateTime.ToString("yyyy-MM-dd")} 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(-((int)dateTime.DayOfWeek));
            var dateTimeFinSemana = System.DateTime.ParseExact($"{dateTimeInicioSemana.ToString("yyyy-MM-dd")} 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(6);
            var data = await _dataBaseService.workinghoursEntity.FromSqlRaw($"SELECT * FROM \"workinghoursEntity\" w WHERE w.\"UserEntityId\"='{idUser}' AND w.\"FechaWorking\" >= TO_TIMESTAMP('{dateTimeInicioSemana.ToString("dd/MM/yyyy")} 00:00', 'DD/MM/YYYY HH24:MI') AND w.\"FechaWorking\" <= TO_TIMESTAMP('{dateTimeFinSemana.ToString("dd/MM/yyyy")} 23:59', 'DD/MM/YYYY HH24:MI') order by TO_TIMESTAMP(substring(w.\"FechaWorking\"::text,0,11)||' '||w.\"HoraInicio\", 'YYYY-MM-DD HH24:MI') asc").ToListAsync();
            
            if (data.Count==0)
            {
                _emailCommand.SendEmail(new EmailModel
                {
                    To = (await _usuarioCommand.GetByUsuarioId(idUser)).Email,
                    Plantilla = "8"
                });
            }
            var entity = _mapper.Map<List<CreateWorkingHoursModel>>(data);
            return entity;
        }

     

        public async Task<List<CreateWorkingHoursModel>> ConsultaHorarioCompleto(Guid idUser, DateTimeOffset dateTime)
        {
            var dateTimeInicioSemana = System.DateTime.ParseExact($"{dateTime.ToString("yyyy-MM-dd")} 00:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(-((int)dateTime.DayOfWeek));
            var dateTimeFinSemana = System.DateTime.ParseExact($"{dateTimeInicioSemana.ToString("yyyy-MM-dd")} 23:59", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddDays(6);
            var data = await _dataBaseService.workinghoursEntity.FromSqlRaw($"SELECT * FROM \"workinghoursEntity\" w WHERE w.\"UserEntityId\"='{idUser}' AND w.\"FechaWorking\" >= TO_TIMESTAMP('{dateTimeInicioSemana.ToString("dd/MM/yyyy")} 00:00', 'DD/MM/YYYY HH24:MI') AND w.\"FechaWorking\" <= TO_TIMESTAMP('{dateTimeFinSemana.ToString("dd/MM/yyyy")} 23:59', 'DD/MM/YYYY HH24:MI') order by TO_TIMESTAMP(substring(w.\"FechaWorking\"::text,0,11)||' '||w.\"HoraInicio\", 'YYYY-MM-DD HH24:MI') asc").ToListAsync();

            if (data.Count == 0)
            {
                _emailCommand.SendEmail(new EmailModel
                {
                    To = (await _usuarioCommand.GetByUsuarioId(idUser)).Email,
                    Plantilla = "8"
                });
            }
            else
            {
                //complete the rest of the week

            }



            var entity = _mapper.Map<List<CreateWorkingHoursModel>>(data);
            return entity;
        }
    }
}
