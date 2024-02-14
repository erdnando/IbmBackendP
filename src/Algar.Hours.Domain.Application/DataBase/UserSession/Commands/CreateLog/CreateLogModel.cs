using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Rol.Commands.Consult;
using Algar.Hours.Domain.Entities.Rol;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog
{
    public  class CreateLogModel
    {

        public Guid IdSession { get; set; }
        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public DateTime LogDateEvent { get; set; }
        public string eventAlias { get; set; }
        public string parameters { get; set; }
        public string operation { get; set; }
        public string resultOperation { get; set; }
        public Guid RoleEntityId { get; set; }
        public RoleEntity RoleEntity { get; set; }
        public string tag { get; set; }


    }
}
