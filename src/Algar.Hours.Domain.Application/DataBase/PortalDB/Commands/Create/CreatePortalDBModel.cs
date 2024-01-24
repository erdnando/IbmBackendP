using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.PortalDB.Commands.Create
{
    public class CreatePortalDBModel
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
        public DateTime CreationDate { get; set; }
        [Required]
        public int TipoReporte { get; set; }
        [Required]
        public int Acitivity { get; set; }
        public string CountHours { get; set; }
        public int NumberReport { get; set; }
        public string ApproverId { get; set; }
        public int State { get; set; }
    }
}
