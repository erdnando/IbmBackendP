using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.WorkdayException.Commands.Create
{
    public class WorkdayExceptionModelC {

        [Required]
        public Guid UserEntityId { get; set; }
        [Required]
        public string EmployeeCode { get; set; }
        [Required]
        public string EmployeeName { get; set; }
        [Required]
        public Guid CountryEntityId { get; set; }
        [Required]
        public DateTime OriginalDate { get; set; }
        [Required]
        public TimeSpan OriginalStartTime { get; set; }
        [Required]
        public TimeSpan OriginalEndTime { get; set; }
        [Required]
        public DateTime RealDate { get; set; }
        [Required]
        public TimeSpan RealStartTime { get; set; }
        [Required]
        public TimeSpan RealEndTime { get; set; }
        [Required]
        public string ReportType { get; set; }
        [Required]
        public string Justification { get; set; }
        [Required]
        public string ApprovingManager { get; set; }

        public bool Active { get; set; }

    }
}
