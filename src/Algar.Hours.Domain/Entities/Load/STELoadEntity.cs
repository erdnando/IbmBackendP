using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Load
{
    public class STELoadEntity
    {
        public Guid IdSTELoad { get; set; }
        public string SessionTimeId { get; set;}
        public string SessionTimeAgentCountry { get; set;}
        public string NumeroCaso { get; set;}
        public string SessionEmployeeSerialNumber { get; set; }
        public string AccountCMRNumber { get; set;}
        public string NombreCuenta { get; set;}
        public string StartDateTime { get; set;}
        public string EndDateTime { get; set;}
        public string EndHours { get; set;}
        public string StartHours { get; set;}
        public string TotalDuration { get; set;}
        public string CaseSubject { get; set;}
        public string FechaRegistro { get; set; }
    }
}
