using Algar.Hours.Domain.Entities.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.ParametrosInicial
{
    public class ParametersSteInitialEntity
    {
        public Guid IdParamSTEInitialId { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string OutIme { get; set; }
        public string OverTime { get; set; }
        public int Semana { get; set; }
        public string Festivo { get; set; }
        public int HorasInicio { get; set; }
        public int HorasFin { get; set; }
        public string Estado { get; set; }
        public Guid STELoadEntityId { get; set; }
        public STELoadEntity STELoadEntity { get; set; }
    }
}
