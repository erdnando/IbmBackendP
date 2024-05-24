using Algar.Hours.Domain.Entities.ParametrosInicial;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class ParametersInitialEntitySTEConfiguration
    {
        public ParametersInitialEntitySTEConfiguration(EntityTypeBuilder<ParametersSteInitialEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdParamSTEInitialId);
            entityBuilder.HasOne(x => x.Carga).WithMany(x => x.steParameters).HasForeignKey(x => x.IdCarga).IsRequired(false);

        }
    }
}
