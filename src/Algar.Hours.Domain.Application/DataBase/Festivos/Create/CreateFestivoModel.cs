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
        public Guid IdFestivo { get; set; }
        public string Descripcion { get; set; }
        public string ano { get; set; }
        public DateTime DiaFestivo { get; set; }
        public Guid CountryId { get; set; }
        public string idUserEntiyId { get; set; }
        
    }
}
