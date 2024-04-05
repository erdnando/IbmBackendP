using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Algar.Hours.Persistence.Configuration
{
    public class UserExceptionsConfiguration
    {
        public UserExceptionsConfiguration(EntityTypeBuilder<UsersExceptions> entityBuilder) 
        {

            entityBuilder.HasKey(x => x.IdUsersExceptions);
            entityBuilder.Property(x => x.StartDate).IsRequired();
            entityBuilder.Property( x => x.horas).IsRequired();
            entityBuilder.Property(x => x.UserId).IsRequired();
            entityBuilder.Property(x=> x.AssignedUserId).IsRequired();
            entityBuilder.Property(x=> x.ReportType).IsRequired();
            entityBuilder.Property(x => x.Description).IsRequired();


        }    

    }
}
