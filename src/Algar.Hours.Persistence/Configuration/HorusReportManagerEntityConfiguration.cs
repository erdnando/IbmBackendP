using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.HorusReportManagerEntity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Persistence.Configuration
{
    public class HorusReportManagerEntityConfiguration
    {
        public HorusReportManagerEntityConfiguration(EntityTypeBuilder<HorusReportManagerEntity> entityBuilder)
        {
            entityBuilder.HasKey(x => x.IdHorusReportManager);
            entityBuilder.Property(x => x.UserEntityManagerId).IsRequired();
            entityBuilder.Property(x => x.UserEntityId).IsRequired();
            entityBuilder.Property(x => x.CreationDate).IsRequired();
            entityBuilder.Property(x => x.StartTime).IsRequired();
            entityBuilder.Property(x => x.EndTime).IsRequired();
            entityBuilder.Property(x => x.TypeReport).IsRequired();
            entityBuilder.Property(x => x.CountHours).IsRequired();
            entityBuilder.Property(x => x.Status).IsRequired();
            entityBuilder.Property(x => x.Observations).IsRequired();
        }
    }
}
