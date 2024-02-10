using Algar.Hours.Domain.Entities.AssignmentReport;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.User;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed
{
    public class ListAproveedModel
    {
        public Guid IdAssignmentReport { get; set; }
        public UserEntity UserEntity { get; set; }
        public Guid UserEntityId { get; set; }
        public HorusReportEntity HorusReportEntity { get; set; }
        public Guid HorusReportEntityId { get; set; }
        public int State { get; set; }
        public string Description { get; set; }
        public string StrReport { get; set; }
        public DateTime DateApprovalCancellation { get; set; }
        

    }
}
