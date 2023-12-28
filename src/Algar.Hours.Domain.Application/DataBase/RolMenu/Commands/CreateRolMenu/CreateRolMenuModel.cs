using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.RolMenu.Commands.CreateRolMenu
{
    public class CreateRolMenuModel
    {
        public Guid IdRoleMenu { get; set; }
        public Guid RoleId { get; set; }

        public Guid MenuEntityId { get; set; }
    }
}
