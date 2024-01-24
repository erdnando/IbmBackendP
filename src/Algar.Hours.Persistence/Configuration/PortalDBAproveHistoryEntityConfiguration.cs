
using Algar.Hours.Domain.Entities.PortalDB;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class PortalDBAproveHistoryEntityConfiguration
    {
        public PortalDBAproveHistoryEntityConfiguration(EntityTypeBuilder<PortalDBAproveHistoryEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdPortalDBAproveHistory);
            entityBuilder.Property(x => x.IdPortalDB).IsRequired();
            entityBuilder.Property(x => x.TipoReporte).IsRequired();

        }
    }
}
