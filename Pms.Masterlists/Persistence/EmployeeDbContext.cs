using Microsoft.EntityFrameworkCore;
using Pms.Common;
using Pms.Masterlists.Entities;
using Pms.Masterlists.ValueObjects;

namespace Pms.Masterlists.Persistence
{
    public class EmployeeDbContext : DbContext
    {
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<PayrollCode> PayrollCodes => Set<PayrollCode>();

        public EmployeeDbContext(DbContextOptions options) : base(options) { }
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new PayrollCodeConfiguration());
        }
    }
}
