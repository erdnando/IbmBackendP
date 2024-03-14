using Algar.Hours.Domain.Entities.Parameters;
using Algar.Hours.Domain.Entities.ReportException;
using Algar.Hours.Domain.Entities.Template;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Algar.Hours.Domain.Entities.WorkdayException;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Algar.Hours.Persistence.Configuration
{
    public class TemplateEntityConfiguration
    {
        public TemplateEntityConfiguration(EntityTypeBuilder<TemplateEntity> entityBuilder) 
        {

            entityBuilder.HasKey(x => x.IdTemplate);
            entityBuilder.Property(x => x.Name).IsRequired();
            entityBuilder.Property(x => x.Description).IsRequired();
            entityBuilder.Property(x => x.FileName).IsRequired();
            entityBuilder.Property(x => x.FileContentType).IsRequired();
            entityBuilder.Property(x => x.FileData).IsRequired();
            entityBuilder.Property(x => x.CreationDate).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        }    

    }
}
