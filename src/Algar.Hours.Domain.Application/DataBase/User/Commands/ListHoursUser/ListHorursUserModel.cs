using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.ListHoursUser
{
    public class ListHorursUserModel
    {
        public Guid IdHorusReport { get; set; } //ok
        public Guid UserEntityId { get; set; }//ok
        public UserEntity UserEntity { get; set; }
        public DateTime StartDate { get; set; }//ok
        public Guid ClientEntityId { get; set; }
        public ClientEntity ClientEntity { get; set; }//ok
        public string Description { get; set; }//ok
        public DateTime CreationDate { get; set; }//ok
        public int Acitivity { get; set; }//ok
        public string CountHours { get; set; }//ok
        public string ApproverId { get; set; }//ok
        public string ApproverId2 { get; set; }//ok
        //public string ApproverNameId { get; set; }
       // public string ApproverIdName2 { get; set; }
        public int NumberReport { get; set; }//ok
        public int TipoReport { get; set; }//ok
        public string StrReport { get; set; }//ok
        public string StartTime { get; set; }//ok
        public string EndTime { get; set; }//ok
        public int Estado { get; set; }//ok
        public string ARPLoadingId { get; set; }//ok
        public DateTime DateApprovalSystem { get; set; }//ok
        public string StrStartTime { get; set; }//ok
        public string StrCreationDate { get; set; }//ok


    }
}
