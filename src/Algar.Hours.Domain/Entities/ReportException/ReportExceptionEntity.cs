using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.ReportException
{
    public class ReportExceptionEntity
    {
        public Guid IdReportException { get; set; }

        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public string Report { get; set; }

        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset? ExceptionDate { get; set; }

        public Guid? ExceptionUserEntityId { get; set; }
        public UserEntity? ExceptionUserEntity { get; set; }


    }
}
