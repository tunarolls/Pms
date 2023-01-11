using Microsoft.EntityFrameworkCore;
using System;

namespace Pms.Payrolls.Persistence
{
    public class PayrollDbContext : DbContext
    {
        public PayrollDbContext(DbContextOptions options) : base(options) { }

        public DbSet<CompanyView> Companies => Set<CompanyView>();
        public DbSet<EmployeeView> Employees => Set<EmployeeView>();
        public DbSet<Payroll> Payrolls => Set<Payroll>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PayrollConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeViewConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        }
    }
}
