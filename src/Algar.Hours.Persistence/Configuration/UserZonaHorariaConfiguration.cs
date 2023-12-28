using Algar.Hours.Domain.Entities;
using Algar.Hours.Domain.Entities.Aprobador;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class UserZonaHorariaConfiguration
    {
        public UserZonaHorariaConfiguration(EntityTypeBuilder<UserZonaHoraria> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdUserZone);
        }

    }
}
