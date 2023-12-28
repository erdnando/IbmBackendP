using Algar.Hours.Domain.Entities.Parameters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Algar.Hours.Persistence.Configuration
{
    public class ParametersEntityConfiguration
    {
        public ParametersEntityConfiguration(EntityTypeBuilder<ParametersEntity> entityBuilder) 
        { 
            entityBuilder.HasKey(x => x.IdParametersEntity);
            entityBuilder.Property(x => x.TargetHourMonth).IsRequired();
            entityBuilder.Property(x => x.CountryEntityId).IsRequired();
            entityBuilder.Property(x=> x.TargetTimeDay).IsRequired();
            entityBuilder.Property(x => x.TypeLimits).IsRequired();


            
        }

    }
}
