using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Consult
{
    public class ConsultMoldeHosrusReportModel
    {
        public Guid IdHorusReport { get; set; }
        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public DateTime StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Guid ClientEntityId { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        
        public string strCreationDate { get; set; }
        public int TipoReporte { get; set; }
        public DateTime DateApprovalSystem { get; set; }
        public int Acitivity { get; set; }
        public string CountHours { get; set; }
        public string ApproverId { get; set; }

        public int NumberReport { get; set; }

        public CountryEntity countryEntity { get; set; }    

    }
}
