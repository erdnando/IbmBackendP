using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.Rol;
using System.Globalization;

namespace Algar.Hours.Domain.Entities.User
{
    public class UserSessionEntity
    {
        public Guid IdSession { get; set; }
        
        public string sUserEntityId { get; set; }
        
        public DateTime LogDateEvent { get; set; }
       
        public string parameters { get; set; }
        public string operation { get; set; }
      


    }
}
    