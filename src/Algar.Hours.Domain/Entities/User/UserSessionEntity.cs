using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.Rol;
using System.Globalization;

namespace Algar.Hours.Domain.Entities.User
{
    public class UserSessionEntity
    {
        public Guid IdSession { get; set; }
        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public DateTime LogDateEvent { get; set; }
        public string eventAlias { get; set; }
        public string parameters { get; set; }
        public string operation { get; set; }
        public string resultOperation { get; set; }   
        public Guid RoleEntityId { get; set; }  
        public   RoleEntity RoleEntity { get; set; }
        public string tag { get; set; }


    }
}
    