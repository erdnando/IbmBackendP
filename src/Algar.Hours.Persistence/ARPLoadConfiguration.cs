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
        }

    }
}
