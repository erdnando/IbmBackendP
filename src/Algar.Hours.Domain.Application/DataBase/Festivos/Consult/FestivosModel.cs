using Algar.Hours.Domain.Entities.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Festivos.Consult
{
    public class FestivosModel
    {
        public Guid IdFestivo { get; set; }
        public string Descripcion { get; set; }
        public string ano { get; set; } 
        public DateTime DiaFestivo { get; set; }
        public string sDiaFestivo { get; set; }
        public Guid CountryId { get; set; }
        public CountryEntity Country { get; set; }
    }
}
