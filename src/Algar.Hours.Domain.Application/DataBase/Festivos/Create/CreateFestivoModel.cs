using Algar.Hours.Domain.Entities.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Festivos.Create
{
    public class CreateFestivoModel
    {
        public Guid IdFestivo { get; set; }//ok
        public string Descripcion { get; set; }//ok
        public string ano { get; set; }//ok
        public DateTime DiaFestivo { get; set; }//ok
        public string sDiaFestivo { get; set; }//ok
        public Guid CountryId { get; set; }//ok
        public string idUserEntiyId { get; set; }
        
    }
}
