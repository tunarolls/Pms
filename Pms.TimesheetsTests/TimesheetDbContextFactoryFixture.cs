using Microsoft.EntityFrameworkCore;
using Pms.Timesheets;
using Pms.Timesheets.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.TimesheetsTests
{
    public class TimesheetDbContextFactoryFixture : IDbContextFactory<TimesheetDbContext>
    {
        public TimesheetDbContextFactory Factory { get; set; }
        private const string _connectionString = "server=localhost;database=payroll3Test_efdb;user=root;password=Soft1234;";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TimesheetDbContextFactoryFixture()
        {
            Factory = new TimesheetDbContextFactory(_connectionString);

            if (!_databaseInitialized)
            {
                using var context = Factory.CreateDbContext();
                context.Database.Migrate();
                TrySeeding(context);

                _databaseInitialized = true;
            }
        }

        public TimesheetDbContext CreateDbContext() => Factory.CreateDbContext();

        public void CreateFactory() => Factory = new TimesheetDbContextFactory(_connectionString);

        private void TrySeeding(TimesheetDbContext context)
        {
            if (!context.Timesheets.Any())
            {
                context.Timesheets.AddRange(
                    new Timesheet() { TimesheetId = "DYYJ_2208-1", CutoffId = "2208-1", EEId = "DYYJ", RawPCV = "DESERVE`300|TESTPCV`400", Allowance = 1000 }
                );
                context.SaveChanges();
            }
        }
    }
}
