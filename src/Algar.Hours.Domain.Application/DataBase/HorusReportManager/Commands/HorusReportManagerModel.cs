using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands
{
    public class HorusReportManagerModel
    {
        public Guid IdHorusReportManager { get; set; }
        public Guid UserEntityManagerId { get; set; }
        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public DateTime CreationDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TypeReport { get; set; }
        public double CountHours { get; set; }
        public string Status { get; set; }
        public string Observations { get; set; }

        
    }
}
