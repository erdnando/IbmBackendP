using Algar.Hours.Domain.Entities.Aprobador;
using Algar.Hours.Domain.Entities.Load;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence
{
    public class ARPLoadConfiguration
    {
        public ARPLoadConfiguration(EntityTypeBuilder<ARPLoadEntity> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdArpLoad);
            entityBuilder.HasMany(x => x.arpParameters).WithOne(x => x.Carga).HasForeignKey(x => x.IdCarga).HasPrincipalKey(x => x.IdArpLoad).IsRequired(false);
            entityBuilder.HasMany(x => x.tseParameters).WithOne(x => x.Carga).HasForeignKey(x => x.IdCarga).HasPrincipalKey(x => x.IdArpLoad).IsRequired(false);
            entityBuilder.HasMany(x => x.steParameters).WithOne(x => x.Carga).HasForeignKey(x => x.IdCarga).HasPrincipalKey(x => x.IdArpLoad).IsRequired(false);
        }

    }
}
