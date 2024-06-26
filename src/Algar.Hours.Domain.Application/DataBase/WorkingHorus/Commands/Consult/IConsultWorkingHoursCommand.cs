using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.Login;
using Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Consult
{
    public interface IConsultWorkingHoursCommand
    {
        Task<CreateUserModel> Execute(LoginUserModel model);

        Task<List<CreateWorkingHoursModel>> Consult(Guid idUser, DateTimeOffset date);
        Task<List<CreateWorkingHoursModel>> ConsultaHorarioCompleto(Guid idUser, DateTimeOffset date);

    }
}
