using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Rol.Commands;
using Algar.Hours.Domain.Entities.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.CreateUser
{
    public class CreateUserModel
    {
        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public string surnameUser { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string Password { get; set; }    
        public Guid RoleEntityId { get; set; }
        public RolModel RoleEntity { get; set; }
        public Guid CountryEntityId { get; set; }
        public CountryModel CountryEntity { get; set; }

    }
}
