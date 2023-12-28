using Algar.Hours.Domain.Entities.AssignmentReport;
using Algar.Hours.Domain.Entities.QueuesAcceptance;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class QueuesAcceptanceEntityConfiguration
    {
        public QueuesAcceptanceEntityConfiguration(EntityTypeBuilder<QueuesAcceptanceEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdQueuesAcceptanceEntity);

        }
    }
}
