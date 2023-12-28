using Algar.Hours.Domain.Entities.ParametrosInicial;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class ParametersInitialEntityTSEConfiguration
    {

        public ParametersInitialEntityTSEConfiguration(EntityTypeBuilder<ParametersTseInitialEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdParamTSEInitialId);

        }

    }
}
