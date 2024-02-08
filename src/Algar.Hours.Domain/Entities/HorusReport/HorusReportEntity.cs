using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.User;


namespace Algar.Hours.Domain.Entities.HorusReport
{
    public class HorusReportEntity
    {
        public Guid IdHorusReport { get; set; }
        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public DateTime StartDate { get; set; }
        public string? StrStartDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Guid? ClientEntityId { get; set; }
        public  ClientEntity ClientEntity { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string strCreationDate { get; set; }
        public int TipoReporte { get; set; }
        public DateTime DateApprovalSystem { get; set; }
        public int Acitivity { get; set; }
        public  string CountHours {  get; set; }
        public int NumberReport {  get; set; }
        public string StrReport { get; set; }
        public string ApproverId { get; set; }
        public string ApproverId2 { get; set; }
        public int Estado { get; set; }
        public string? ARPLoadingId { get; set; }    

    }
}
