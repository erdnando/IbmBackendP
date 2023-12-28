using Algar.Hours.Domain.Entities.Country;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public  class CountryConfiguration
    {
        public CountryConfiguration(EntityTypeBuilder<CountryEntity> entityBuilder) 
        {

            entityBuilder.HasKey(x => x.IdCounty);
            entityBuilder.Property(x => x.NameCountry).IsRequired();

        }

    }
}
