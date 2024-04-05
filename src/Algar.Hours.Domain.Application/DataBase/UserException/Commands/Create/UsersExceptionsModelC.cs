using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Create
{
    public class UsersExceptionsModelC
    {
        public Guid IdUsersExceptions { get; set; }

        public Guid UserId { get; set; }
        public Guid AssignedUserId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public float horas { get; set; }

        public string Description { get; set; }
        public string ReportType { get; set; }
    }
}
