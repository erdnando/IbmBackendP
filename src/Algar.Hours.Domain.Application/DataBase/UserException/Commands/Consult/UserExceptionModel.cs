using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.UserException.Commands.Consult
{
    public class UserExceptionModel
    {
        public Guid IdUsersExceptions { get; set; }

        public Guid UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public Guid AssignedUserId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public float horas  { get; set; }

        public string Description { get; set; }
    }
}
