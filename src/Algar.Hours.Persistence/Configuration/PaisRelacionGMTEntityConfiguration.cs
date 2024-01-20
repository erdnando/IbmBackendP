using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.PaisRelacionGMT;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class PaisRelacionGMTEntityConfiguration
    {
        public PaisRelacionGMTEntityConfiguration(EntityTypeBuilder<PaisRelacionGMTEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdPaisRelacionGMTEntity);
            entityBuilder.Property(x => x.NameCountrySelected).IsRequired();
            entityBuilder.Property(x => x.NameCountryCompare).IsRequired();
            entityBuilder.Property(x => x.TimeDifference).IsRequired();

        }
    }
}
