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

        public string ARPCarga { get; set; }
        public string TSECarga { get; set; }
        public string STECarga { get; set; }
        public string ARPOmitidos { get; set; }
        public string TSEOmitidos { get; set; }
        public string STEOmitidos { get; set; }

        public string ARPXHorario { get; set; }
        public string ARPXOvertime { get; set; }
        public string ARPXOverlaping { get; set; }
        public string ARPXProceso { get; set; }

        public string TSEXHorario { get; set; }
        public string TSEXOvertime { get; set; }
        public string TSEXOverlaping { get; set; }
        public string TSEXProceso { get; set; }

        public string STEXHorario { get; set; }
        public string STEEXOvertime { get; set; }
        public string STEXOverlaping { get; set; }
        public string STEXProceso { get; set; }

        public string ARPOmitidosXDuplicidad { get; set; }
        public string TSEOmitidosXDuplicidad { get; set; }
        public string STEOmitidosXDuplicidad { get; set; }
        
        public string ARPXDatosNovalidos { get; set; }
        public string TSEXDatosNovalidos { get; set; }
        public string STEXDatosNovalidos { get; set; }

        public string EstadoCarga { get; set; }

    }
}
