using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using System;
using System.Linq;

namespace Pms.Adjustments.Persistence
{
    public class AdjustmentDbContext : DbContext
    {
        public DbSet<Billing> Billings => Set<Billing>();
        public DbSet<BillingRecord> BillingRecords => Set<BillingRecord>();
        public DbSet<EmployeeView> Employees => Set<EmployeeView>();
        public DbSet<TimesheetView> Timesheets => Set<TimesheetView>();

        public AdjustmentDbContext(DbContextOptions options) : base(options) { }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BillingConfiguration());
            modelBuilder.ApplyConfiguration(new BillingRecordConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeViewConfiguration());
            modelBuilder.ApplyConfiguration(new TimesheetConfiguration());
        }
    }
}
