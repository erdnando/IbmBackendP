using Algar.Hours.Domain.Entities.Gerentes;
using Algar.Hours.Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class UserManagerEntityConfiguration
    {
        public UserManagerEntityConfiguration(EntityTypeBuilder<UserManagerEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdUserManager);
            entityBuilder.Property(x => x.ManagerEmployeeCode).IsRequired();
            entityBuilder.Property(x => x.ManagerName).IsRequired();
            entityBuilder.Property(x => x.ManagerEmail).IsRequired();
            entityBuilder.Property(x => x.ManagerCountry).IsRequired();
            entityBuilder.Property(x => x.EmployeeCode).IsRequired();
            entityBuilder.Property(x => x.EmployeeName).IsRequired();
            entityBuilder.Property(x => x.EmployeeEmail).IsRequired();

        }
    }
}
