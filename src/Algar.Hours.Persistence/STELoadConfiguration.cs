using Algar.Hours.Domain.Entities.Load;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence
{
    public class STELoadConfiguration
    {
        public STELoadConfiguration(EntityTypeBuilder<STELoadEntity> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdSTELoad);
        }
    }
}
