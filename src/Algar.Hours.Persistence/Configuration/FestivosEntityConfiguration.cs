using Algar.Hours.Domain.Entities.Festivos;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class FestivosEntityConfiguration
    {
        public FestivosEntityConfiguration(EntityTypeBuilder<FestivosEntity> entityTypeBuilder) 
        {
            entityTypeBuilder.HasKey(x => x.IdFestivo);
            entityTypeBuilder.Property(x => x.Descripcion).IsRequired();
            entityTypeBuilder.Property(x => x.ano).IsRequired();
            entityTypeBuilder.Property(x => x.DiaFestivo).IsRequired();
            entityTypeBuilder.Property(x => x.CountryId).IsRequired();        
        }
    }
}
