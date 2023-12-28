using Algar.Hours.Domain.Entities.AssignmentReport;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Algar.Hours.Persistence.Configuration
{
    public class AssignmentReportConfiguration
    {

        public AssignmentReportConfiguration(EntityTypeBuilder<AssignmentReport> entityBuilder)
        {

            entityBuilder.HasKey(x => x.IdAssignmentReport);
         
        }
    }
}
