using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.PaisRelacionGMT
{
    public class PaisRelacionGMTEntity
    {
        public Guid IdPaisRelacionGMTEntity { get; set; }
        public string NameCountrySelected { get; set; }
        public string NameCountryCompare { get; set; }
        public int TimeDifference { get; set; }
    }
}
