using Algar.Hours.Domain.Entities.Rol;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class RolEntityConfiguration
    {
        public RolEntityConfiguration(EntityTypeBuilder<RoleEntity> entityBuilder) 
        {
            entityBuilder.HasKey(x => x.IdRole);
            entityBuilder.Property(x => x.NameRole).IsRequired();

        }


    }
}
