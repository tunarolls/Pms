using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Persistence
{
    public class AdjustmentDbContextFactory : IDbContextFactory<AdjustmentDbContext>, IDesignTimeDbContextFactory<AdjustmentDbContext>
    {

        private readonly string _connectionString;

        public AdjustmentDbContextFactory(string connectionString) =>
            _connectionString = connectionString;

        public AdjustmentDbContextFactory() =>
            _connectionString = "server=localhost;database=payroll3Test_efdb;user=root;password=Soft1234;";

        public AdjustmentDbContext CreateDbContext()
        {
            DbContextOptions dbContextOptions = new DbContextOptionsBuilder()
                .UseMySQL(
                    _connectionString,
                    options => options.MigrationsHistoryTable("AdjustmentsMigrationHistory")
                )
                .Options;

            return new AdjustmentDbContext(dbContextOptions);
        }

        public AdjustmentDbContext CreateDbContext(string[] args) => CreateDbContext();
    }
}
