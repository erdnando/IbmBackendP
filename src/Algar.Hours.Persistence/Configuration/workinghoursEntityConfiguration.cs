using Algar.Hours.Domain.Entities.Horario;
using Algar.Hours.Domain.Entities.UsersExceptions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class workinghoursEntityConfiguration
    {
        public workinghoursEntityConfiguration(EntityTypeBuilder<workinghoursEntity> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdworkinghoursEntity);
        }
    }
}
