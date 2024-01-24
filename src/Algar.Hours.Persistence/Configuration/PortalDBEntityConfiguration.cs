using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.PortalDB;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class PortalDBEntityConfiguration
    {
        public PortalDBEntityConfiguration(EntityTypeBuilder<PortalDBEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdPortalDb);
            entityBuilder.Property(x => x.UserEntityId).IsRequired();
            entityBuilder.Property(x => x.StartDate).IsRequired();
            entityBuilder.Property(x => x.EndTime).IsRequired();
            entityBuilder.Property(x => x.CreationDate).IsRequired();
            entityBuilder.Property(x => x.TipoReporte).IsRequired();


        }
    }
}
