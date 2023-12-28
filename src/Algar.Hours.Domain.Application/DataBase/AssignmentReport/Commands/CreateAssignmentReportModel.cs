using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands
{
    public class CreateAssignmentReportModel
    {
        public Guid IdAssignmentReport { get; set; }
        public Guid UserEntityId { get; set; }
        public Guid HorusReportEntityId { get; set; }
        public int State { get; set; }
        public string Description { get; set; }
        public DateTime DateApprovalCancellation { get; set; }
    }


}
