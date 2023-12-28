using Algar.Hours.Domain.Entities.Menu;
using Algar.Hours.Domain.Entities.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.RolMenu.Commands.Consult
{
    public class RolMenuModel
    {
        public Guid IdRoleMenu { get; set; }
        public Guid RoleId { get; set; }
        public RoleEntity Role { get; set; }
        public Guid MenuEntityId { get; set; }
        public MenuEntity MenuEntity { get; set; }
    }
}
