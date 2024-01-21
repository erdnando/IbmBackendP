using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Load
{
    public class ARPLoadDetailEntity
    {

        public Guid IdDetail { get; set; }
        public string DOC_NUM {  get; set; }
        public string TOOL { get; set; }
        public string PAIS { get; set; }
        public string ID_EMPLEADO { get; set; }
        public string NUMERO_CLIENTE { get; set; }
        public string NOMBRE_CLIENTE { get; set; }
        public string ESTADO { get; set; }
        public string FECHA_REP { get; set; }
        public string HORA_INICIO { get; set; }
        public string HORA_FIN { get; set; }
        public string TOTAL_MINUTOS { get; set; }
        public string TOTALHORAS { get; set; }
        public string CATEGORIA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string COMENTARIO { get; set; }
        public string FECHA_EXTRATED { get; set; }
        public Guid ARPLoadEntityId { get; set; }
        public ARPLoadEntity ARPLoadEntity { get; set; }


    }
}
