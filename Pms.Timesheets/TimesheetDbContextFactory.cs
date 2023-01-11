using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class TimesheetDbContextFactory : IDbContextFactory<TimesheetDbContext>, IDesignTimeDbContextFactory<TimesheetDbContext>
    {
        private readonly string _connectionString;
        private readonly bool _lazyLoad;

        public TimesheetDbContextFactory(string connectionString, bool lazyLoad = false)
        {
            _connectionString = connectionString;
            _lazyLoad = lazyLoad;
        }

        public TimesheetDbContextFactory() =>
            _connectionString = "server=localhost;database=payroll3Test_efdb;user=root;password=Soft1234;";

        public TimesheetDbContext CreateDbContext()
        {
            DbContextOptions dbContextOptions = new DbContextOptionsBuilder()
                .UseLazyLoadingProxies(_lazyLoad)
                .UseMySQL(
                    _connectionString,
                    options => options.MigrationsHistoryTable("TimesheetsMigrationHistoryName")
                )
                .Options;
            return new TimesheetDbContext(dbContextOptions);
        }

        public TimesheetDbContext CreateDbContext(string[] args) => CreateDbContext();
    }
}
