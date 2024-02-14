using Algar.Hours.Domain.Entities.Rol;
using Algar.Hours.Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Algar.Hours.Persistence.Configuration
{
    public class UserSessionConfiguration
    {
        public UserSessionConfiguration(EntityTypeBuilder<UserSessionEntity> entityBuilder) 
        {

            entityBuilder.HasKey(x => x.IdSession);
            entityBuilder.Property(x=> x.UserEntityId).IsRequired();
            entityBuilder.Property(x=> x.LogDateEvent).IsRequired();
            entityBuilder.Property(x=> x.eventAlias).IsRequired();
            entityBuilder.Property(x=> x.tag).IsRequired();


    }

    }
}
