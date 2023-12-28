using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkingHorus.Commands.Load
{
    public class LoadWorkingHoursModel
    {
        public string fecha { get; set; }
        public string codigo_Empleado { get; set; }

        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }

        public string dia { get; set; }
        public string pais { get; set; }
    }
}
