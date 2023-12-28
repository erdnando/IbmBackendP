using Algar.Hours.Application.DataBase.Country.Commands;
using Algar.Hours.Application.DataBase.Rol.Commands.Consult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.CreateUser
{
    public  class CreateUserModelc
    {

        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public string surnameUser { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string Password { get; set; }
        public Guid RoleEntityId { get; set; }
        public Guid CountryEntityId { get; set; }


    }
}
