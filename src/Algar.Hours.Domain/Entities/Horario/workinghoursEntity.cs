using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Horario
{
    public class workinghoursEntity
    {
        public Guid IdworkinghoursEntity { get; set; }
        public Guid UserEntityId {  get; set; }
        public UserEntity UserEntity { get; set; }
        public string week { get; set; }
        public DateTimeOffset FechaCreacion { get; set; } 
        public string HoraInicio { get; set; }
        public string HoraFin {  get; set; }  
        public string Day {  get; set; }
        public string Ano {  get; set; }
        public DateTimeOffset FechaWorking { get; set; }  


    }
}
