using Algar.Hours.Domain.Entities.PortalDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.PortalDBAproveHistory.Commands.Create
{
    public class CreatePortalDBAproveHistoryModel
    {
        public Guid IdPortalDBAproveHistory { get; set; }
        [Required]
        public Guid IdPortalDB { get; set; }
        public int State { get; set; }
        public DateTime DateApprovalCancellation { get; set; }
        public string ApproverId { get; set; }
        public int TipoReporte { get; set; }
        public int Acitivity { get; set; }
        public int NumberReport { get; set; }
        public string Description { get; set; }
    }
}
