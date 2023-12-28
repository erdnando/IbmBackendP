using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.ListHoursUser
{
    public class ListHorursUserModel
    {
        public Guid IdHorusReport { get; set; }
        public Guid UserEntityId { get; set; }
        public UserEntity UserEntity { get; set; }
        public DateTime StartDate { get; set; }
        public Guid ClientEntityId { get; set; }
        public ClientEntity ClientEntity { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int Acitivity { get; set; }
        public string CountHours { get; set; }
        public string ApproverId { get; set; }
        public int NumberReport { get; set; }
        public string StartTime { get; set; }


    }
}
