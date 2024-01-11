using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create
{
    public class CreateHorusReportManagerModel
    {
        [Required]
        public Guid IdHorusReportManager { get; set; }
        [Required]
        public Guid UserEntityManagerId { get; set; }
        [Required]
        public Guid UserEntityId { get; set; }
        [Required]
        public UserEntity UserEntity { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string StartTime { get; set; }
        [Required]
        public string EndTime { get; set; }
        [Required]
        public string TypeReport { get; set; }
        [Required]
        public double CountHours { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Observations { get; set; }
    }
}
