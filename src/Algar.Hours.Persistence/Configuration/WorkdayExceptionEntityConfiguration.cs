using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ReportException;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Algar.Hours.Domain.Entities.WorkdayException;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Algar.Hours.Persistence.Configuration
{
    public class WorkdayExceptionEntityConfiguration
    {
        public WorkdayExceptionEntityConfiguration(EntityTypeBuilder<WorkdayExceptionEntity> entityBuilder) 
        {

            entityBuilder.HasKey(x => x.IdWorkdayException);
            entityBuilder.Property(x => x.UserEntityId).IsRequired();
            entityBuilder.Property(x => x.EmployeeCode).IsRequired();
            entityBuilder.Property(x => x.EmployeeName).IsRequired();
            entityBuilder.Property(x => x.CountryEntityId).IsRequired();
            entityBuilder.Property(x => x.OriginalDate).IsRequired();
            entityBuilder.Property(x => x.OriginalStartTime).IsRequired();
            entityBuilder.Property(x => x.OriginalEndTime).IsRequired();
            entityBuilder.Property(x => x.RealDate).IsRequired();
            entityBuilder.Property(x => x.RealStartTime).IsRequired();
            entityBuilder.Property(x => x.RealEndTime).IsRequired();
            entityBuilder.Property(x => x.ReportType).IsRequired();
            entityBuilder.Property(x => x.Justification).IsRequired();
            entityBuilder.Property(x => x.ApprovingManager).IsRequired();
            entityBuilder.Property(x => x.CreationDate).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entityBuilder.Property(x => x.Active).HasDefaultValue(true);
        }    

    }
}
