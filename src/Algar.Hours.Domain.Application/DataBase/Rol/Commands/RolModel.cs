using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Rol.Commands
{
    public class RolModel
    {
        public Guid IdRole { get; set; }
        public string NameRole { get; set; }
        public List<MenuModelc> MenuEntity { get; set; }
    }
}
