using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Pms.Masterlists.Persistence
{
    public class EmployeeDbContextFactory : IDbContextFactory<EmployeeDbContext>,IDesignTimeDbContextFactory<EmployeeDbContext>
    {
        private readonly string _connectionString;
        public EmployeeDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public EmployeeDbContextFactory()
        {
            //_connectionString = "server=localhost;database=payroll3Test_efdb;user=root;password=Soft1234;";
            _connectionString = "server=192.168.56.101;database=payroll3_efdb;user=tim;password=tim@123";
        }

        public EmployeeDbContext CreateDbContext()
        {
            DbContextOptions dbContextOptions = new DbContextOptionsBuilder()
                .UseMySQL(_connectionString, options => options.MigrationsHistoryTable("EmployeesMigrationHistoryName"))
                .Options;

            return new EmployeeDbContext(dbContextOptions);
        }

        public EmployeeDbContext CreateDbContext(string[] args) => CreateDbContext();
    }
}
