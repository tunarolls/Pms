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
    
     public class EmployeeViewConfiguration : IEntityTypeConfiguration<EmployeeView>
    {
        public void Configure(EntityTypeBuilder<EmployeeView> builder)
        {
            builder.ToView("masterlist").HasKey(ee => ee.EEId);   
             
        }
    }
}
