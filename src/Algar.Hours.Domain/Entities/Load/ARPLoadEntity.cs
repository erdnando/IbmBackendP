using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Load
{
    public class ARPLoadEntity
    {
        public Guid IdArpLoad { get; set;}

        public DateTime FechaCreacion { get; set; }

        public Guid userEntityId { get; set; }

        public UserEntity userEntity { get; set; }  

        public int Estado { get; set; }

    }
}
