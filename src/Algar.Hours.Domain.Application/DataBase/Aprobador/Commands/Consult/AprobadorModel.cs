using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Aprobador.Commands.Consult
{
    public class AprobadorModel
    {
        public Guid IdAprobador { get; set; }
        public string Descripcion { get; set; }
        public int Nivel { get; set; }

    }
}
