using Algar.Hours.Domain.Entities.AssignmentReport;
using Algar.Hours.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class ClientConfiguration
    {

        public ClientConfiguration(EntityTypeBuilder<ClientEntity> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdClient);
            entityBuilder.Property(x => x.NameClient).IsRequired();
            

        }
    }
}
