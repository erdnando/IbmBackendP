using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Dashboard.Commands.Consult
{
    public class Reporte1Model
    {

        public string arpLoadDetailEntityId { get; set; }
        public string idDetail { get; set; }
        public string semana { get; set; }
        public string pais { get; set; }
        public string idEmpleado { get; set; }
        public string fechaRep { get; set; }
        public string horaInicio { get; set; }
        public string horaFin { get; set; }
        public string totalMinutos { get; set; }


    }
}
