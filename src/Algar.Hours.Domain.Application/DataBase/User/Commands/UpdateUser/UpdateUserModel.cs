using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.Login;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.UpdateUser
{
    public class UpdateUserModel
    {
        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public string surnameUser { get; set; }
        public string Email { get; set; }
     

    }
}
