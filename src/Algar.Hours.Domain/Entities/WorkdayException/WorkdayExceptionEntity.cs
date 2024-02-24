using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.WorkdayException
{
    public class WorkdayExceptionEntity
    {
        [Key]
        public Guid IdWorkdayException { get; set; }

        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Guid CountryEntityId { get; set; }
        public CountryEntity CountryEntity { get; set; }
        public DateTime OriginalDate { get; set; }
        public TimeSpan OriginalStartTime { get; set; }
        public TimeSpan OriginalEndTime { get; set; }
        public DateTime RealDate { get; set; }
        public TimeSpan RealStartTime { get; set; }
        public TimeSpan RealEndTime { get; set; }
        public string ReportType { get; set; }
        public string Justification { get; set; }
        public string ApprovingManager { get; set; }

        public DateTime CreationDate { get; set; }
        public bool Active { get; set; }
        
    }
}
