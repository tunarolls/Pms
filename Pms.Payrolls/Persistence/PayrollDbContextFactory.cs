using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Pms.Payrolls.Persistence
{
    public class PayrollDbContextFactory : IDbContextFactory<PayrollDbContext>, IDesignTimeDbContextFactory<PayrollDbContext>
    {
        private readonly bool _lazyLoad;
        private readonly string _connectionString;

        public PayrollDbContextFactory(string connectionString, bool lazyLoad=false)
        {
            _connectionString = connectionString;
            _lazyLoad = lazyLoad;
        }

        public PayrollDbContextFactory() =>
            _connectionString = "server=192.168.56.101;database=payroll3Test_efdb;user=tim;password=tim@123;";

        public PayrollDbContext CreateDbContext()
        {
            DbContextOptions options = new DbContextOptionsBuilder()
                .UseLazyLoadingProxies(_lazyLoad)
                .UseMySQL(
                    _connectionString, 
                    options => options.MigrationsHistoryTable("payrollMigrationHistory")
                )
                .Options;
            return new PayrollDbContext(options);
        }

        public PayrollDbContext CreateDbContext(string[] args) => CreateDbContext();
    }
}
