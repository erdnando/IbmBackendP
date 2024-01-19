using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Gerentes
{
    public class UserManagerEntity
    {
        public Guid IdUserManager { get; set; }
        public string ManagerEmployeeCode { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerCountry { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        
    }
}
