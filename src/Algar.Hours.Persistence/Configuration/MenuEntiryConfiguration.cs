using Algar.Hours.Domain.Entities.Menu;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class MenuEntiryConfiguration
    {
        public MenuEntiryConfiguration(EntityTypeBuilder<MenuEntity> entityBuilder) 
        {
            entityBuilder.HasKey(x => x.IdMenu);      
        
        }


    }
}
