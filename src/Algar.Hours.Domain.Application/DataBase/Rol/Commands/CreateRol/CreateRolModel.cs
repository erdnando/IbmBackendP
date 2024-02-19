using Algar.Hours.Application.DataBase.Menu.Commands;
using Algar.Hours.Application.DataBase.Menu.Commands.Consult;
using Algar.Hours.Domain.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Rol.Commands
{
    public class CreateRolModel
    {
        public Guid IdRole { get; set; }
        public string NameRole { get; set; }
        public List<MenuModelc> MenuEntity { get; set; }
        public string idUserEntiyId { get; set; }
        
    }
}
