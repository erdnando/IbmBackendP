using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Create
{
    public class ReportExceptionModelC
    {
        
        public Guid UserEntityId { get; set; }
        public string Report { get; set; }

    }
}
