using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public class ARPLoadEntityc
    {
        public Guid IdArpLoad { get; set; }

        public DateTime FechaCreacion { get; set; }

        public Guid userEntityId { get; set; }

        public int Estado { get; set; }
        
    }
}
