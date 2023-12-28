using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class QueuesAcceptanceEntityTSEConfiguration
    {
        public QueuesAcceptanceEntityTSEConfiguration(EntityTypeBuilder<QueuesAcceptanceEntityTSE> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdQueuesAcceptanceEntityTSE);
        }
    }
}
