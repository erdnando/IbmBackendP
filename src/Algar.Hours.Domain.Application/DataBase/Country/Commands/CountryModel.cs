using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Country.Commands
{
    public class CountryModel
    {
        public Guid IdCounty { get; set; }
        public string NameCountry { get; set; }
        public int ZonaHoraria { get; set; }
        public string Descripcion { get; set; }
        public string CodigoPais { get; set; }

    }
}
