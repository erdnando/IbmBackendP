using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Menu.Commands
{
    public class MenuModel
    {
        public Guid IdMenu { get; set; }
        public string NameMenu { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
    }
}
