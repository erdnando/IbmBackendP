using Algar.Hours.Domain.Entities.RolMenu;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Algar.Hours.Persistence.Configuration
{
    public class RolMenuEntityConfiguration
    {
        public RolMenuEntityConfiguration(EntityTypeBuilder<RoleMenuEntity> entityTypeBuilder) 
        { 
            entityTypeBuilder.HasKey(x => x.IdRoleMenu);
            entityTypeBuilder.Property(x => x.MenuEntityId).IsRequired();
            entityTypeBuilder.Property(x => x.RoleId).IsRequired();

           
        }

    }
}
