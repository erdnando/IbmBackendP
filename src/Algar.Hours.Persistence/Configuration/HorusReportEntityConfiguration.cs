using Algar.Hours.Domain.Entities.HorusReport;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class HorusReportEntityConfiguration
    {
        public HorusReportEntityConfiguration(EntityTypeBuilder<HorusReportEntity> entityBuilder) 
        {

            entityBuilder.HasKey(x => x.IdHorusReport);
            entityBuilder.Property(x => x.UserEntityId).IsRequired();
            entityBuilder.Property(x => x.StrStartDate).IsRequired();  
            entityBuilder.Property(x => x.EndTime).IsRequired();            
            entityBuilder.Property(x => x.strCreationDate).IsRequired();
            //entityBuilder.Property(x => x.TipoReporte).IsRequired();

        
        } 

    }
}
