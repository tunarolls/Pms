using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Pms.Timesheets.Persistence
{
    public class TimesheetDbContext : DbContext
    {
        public DbSet<Timesheet> Timesheets => Set<Timesheet>();
        public DbSet<EmployeeView> Employees => Set<EmployeeView>();

        public TimesheetDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TimesheetConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeViewConfiguration());
        }

        public override int SaveChanges()
        {
            var timesheetEntries = ChangeTracker.Entries()
                .Where(e =>
                    e.Entity is Timesheet && (
                        e.State == EntityState.Added ||
                        e.State == EntityState.Modified)
                    );
            foreach (var entityEntry in timesheetEntries)
                ((Timesheet)entityEntry.Entity).DateCreated = DateTime.Now;

            return base.SaveChanges();
        }
    }
}
