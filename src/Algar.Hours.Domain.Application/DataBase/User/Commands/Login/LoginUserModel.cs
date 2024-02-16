using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Login
{
    public class LoginUserModel
    {
        public string UserName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

    }
}
