using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Persistence
{
    public class TimesheetConfiguration : IEntityTypeConfiguration<TimesheetView>
    {

        public void Configure(EntityTypeBuilder<TimesheetView> builder)
        {
            builder.ToView("timesheet").HasKey(ee => ee.TimesheetId);

            builder.Property(cc => cc.TimesheetId).HasColumnName("id");
        }
    }
}
