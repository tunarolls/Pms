using Microsoft.EntityFrameworkCore;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Persistence;
using System.Linq;

namespace Pms.MasterlistsTests
{
    public class EmployeeDbContextFactoryFixture : IDbContextFactory<EmployeeDbContext>
    {
        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public EmployeeDbContextFactoryFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using var context = CreateDbContext();
                    context.Database.Migrate();
                    TrySeeding(context);

                    _databaseInitialized = true;
                }
            }
        }

        public EmployeeDbContextFactory Factory { get; private set; } = new();

        public EmployeeDbContext CreateDbContext() => Factory.CreateDbContext();

        public void CreateFactory() => Factory = new EmployeeDbContextFactory();

        private void TrySeeding(EmployeeDbContext context)
        {
            if (!context.Employees.Any())
            {
                context.AddRange(new Employee() { EEId = "DYYJ" });
                context.SaveChanges();
            }
        }
    }
}
