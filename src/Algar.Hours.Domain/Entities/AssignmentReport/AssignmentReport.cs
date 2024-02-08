using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.User;


namespace Algar.Hours.Domain.Entities.AssignmentReport
{
    public class AssignmentReport
    {
        public Guid IdAssignmentReport { get; set; }
        public Guid UserEntityId { get; set; }
        public  UserEntity UserEntity { get; set; }
        public string Employee { get; set; }
        public string TipoAssignment { get; set; }
        public Guid HorusReportEntityId { get; set; }
        public  HorusReportEntity HorusReportEntity { get; set; }
        public int State { get; set; }
        public string Description { get; set; }
        public DateTime DateApprovalCancellation { get; set; }
    }
}
