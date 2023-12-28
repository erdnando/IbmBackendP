using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Create
{
    public class CreateHorusReportModel
    {
        [Required]
        public Guid UserEntityId { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string StartTime { get; set; }

        [Required]
        public string EndTime { get; set; }

        public Guid? ClientEntityId { get; set; }

        public string Description { get; set; }

        [Required]
        public int TipoReporte { get; set; }

        [Required]
        public int Activity { get; set; }

        public string CountHours { get; set; }

        public Guid ApproverId { get; set; }

    }
}
