using Algar.Hours.Domain.Entities.AprobadorUsuario;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Algar.Hours.Persistence.Configuration
{
    public class AprobadorUsuarioConfiguration
    {
        public AprobadorUsuarioConfiguration(EntityTypeBuilder<AprobadorUsuario> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdAprobadorUsuario);


       

        }


    }
}
