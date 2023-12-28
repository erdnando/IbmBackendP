using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.ListGetUser
{
    public class ListGetUserModel
    {
        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public string surnameUser { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public Guid RoleEntityId { get; set; }
        public RoleEntity RoleEntity { get; set; }
        public Guid CountryEntityId { get; set; }
        public CountryEntity CountryEntity { get; set; }

    }
}
