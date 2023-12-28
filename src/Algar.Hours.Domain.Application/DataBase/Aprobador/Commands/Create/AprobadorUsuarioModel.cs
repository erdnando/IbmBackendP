using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Create
{
    public class AprobadorUsuarioModel
    {
        public Guid IdAprobadorUsuario { get; set; }
        public Guid UserEntityId { get; set; }
        public CreateUserModel UserEntity { get; set; }
        public Guid AprobadorId { get; set; }
        public AprobadorModel Aprobador { get; set; }



    }
}
