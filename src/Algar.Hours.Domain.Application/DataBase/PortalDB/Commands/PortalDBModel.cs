using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.PortalDB.Commands
{
    public class PortalDBModel
    {
        public Guid IdPortalDb { get; set; }
        public Guid UserEntityId { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Guid? ClientEntityId { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int TipoReporte { get; set; }
        public int Acitivity { get; set; }
        public string CountHours { get; set; }
        public int NumberReport { get; set; }
        public string ApproverId { get; set; }
        public int State { get; set; }

    }
}
