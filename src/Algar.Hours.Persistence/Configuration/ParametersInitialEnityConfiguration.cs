using Algar.Hours.Domain.Entities.ParametrosInicial;
using Algar.Hours.Domain.Entities.Rol;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class ParametersInitialEnityConfiguration
    {
        public ParametersInitialEnityConfiguration(EntityTypeBuilder<ParametersArpInitialEntity> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdParametersInitialEntity);
            entityBuilder.HasOne(x => x.Carga).WithMany(x => x.arpParameters).HasForeignKey(x => x.IdCarga).IsRequired(false);

        }

    }
}
