using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pms.Timesheets
{
    public class EmployeeViewConfiguration : IEntityTypeConfiguration<EmployeeView>
    {
        public void Configure(EntityTypeBuilder<EmployeeView> builder)
        {
            builder.ToView("masterlist").HasKey(ee => ee.EEId);
        }
    }
}
