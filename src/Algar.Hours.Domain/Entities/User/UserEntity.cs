using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.Rol;
using System.Globalization;

namespace Algar.Hours.Domain.Entities.User
{
    public class UserEntity
    {
        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public string surnameUser { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string Password { get; set; }    
        public Guid RoleEntityId { get; set; }  
        public   RoleEntity RoleEntity { get; set; }
        public Guid CountryEntityId { get; set; }
        public  CountryEntity CountryEntity { get; set; }


    }
}
    