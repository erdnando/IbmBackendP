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
        public string sUserEntityId { get; set; }
        public DateTime LogDateEvent { get; set; }  
       
        public string parameters { get; set; }
        public string operation { get; set; }





    }
}
