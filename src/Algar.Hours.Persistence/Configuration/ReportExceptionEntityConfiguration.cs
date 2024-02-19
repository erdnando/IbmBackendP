using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ReportException;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Algar.Hours.Persistence.Configuration
{
    public class ReportExceptionEntityConfiguration
    {
        public ReportExceptionEntityConfiguration(EntityTypeBuilder<ReportExceptionEntity> entityBuilder) 
        {

            entityBuilder.HasKey(x => x.IdReportException);
            entityBuilder.Property(x => x.UserEntityId).IsRequired();
            entityBuilder.Property(x => x.Report).IsRequired();
            entityBuilder.Property(x => x.CreationDate).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.Property(x => x.ExceptionDate);
            entityBuilder.Property(x => x.ExceptionUserEntityId);
            
        }    

    }
}
