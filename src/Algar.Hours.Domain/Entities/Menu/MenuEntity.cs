using Algar.Hours.Domain.Entities.RolMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Menu
{
    public class MenuEntity
    {
        public Guid IdMenu { get; set; }
        public string NameMenu {  get; set; }   
        public string Path {  get; set; }   
        public string Icon { get; set; }
        public int Order { get; set; }
    }
}
