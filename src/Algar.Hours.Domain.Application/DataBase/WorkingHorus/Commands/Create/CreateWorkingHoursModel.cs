using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Create
{
    public class CreateWorkingHoursModel
    {
        public Guid UserEntityId { get; set; }
        public string week { get; set; }

        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }

        public string Day { get; set; }
        public string Ano { get; set; } 
        public DateTimeOffset FechaWorking { get; set; }

    }
}
