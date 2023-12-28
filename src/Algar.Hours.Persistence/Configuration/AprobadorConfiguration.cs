using Algar.Hours.Domain.Entities.Aprobador;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Algar.Hours.Persistence.Configuration
{
    public class AprobadorConfiguration
    {
        public AprobadorConfiguration(EntityTypeBuilder<Aprobador> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdAprobador);
        }

    }
}
