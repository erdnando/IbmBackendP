using Algar.Hours.Domain.Entities.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Parameters.Commands.ConsulParameters
{
    public class ConsultParametersModel
    {
        public Guid IdParametersEntity { get; set; }
        public double TargetTimeDay { get; set; }
        public double TargetHourWeek { get; set; }
        public double TargetHourMonth { get; set; }
        public double TargetHourYear { get; set; }
        public int TypeLimits { get; set; }
        public int TypeHours { get; set; }
        public Guid CountryEntityId { get; set; }

    }
}
