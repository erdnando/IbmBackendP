namespace Algar.Hours.Domain.Entities.PortalDB
{
    public class PortalDBAproveHistoryEntity
    {
        public Guid IdPortalDBAproveHistory { get; set; }
        public Guid IdPortalDB { get; set; }
        public PortalDBEntity PortalDBEntity { get; set; }
        public int State { get; set; }
        public DateTime DateApprovalCancellation { get; set; }
        public string ApproverId { get; set; }
        public int TipoReporte { get; set; }
        public int Acitivity { get; set; }
        public int NumberReport { get; set; }
        public string Description { get; set; }
    }
}
