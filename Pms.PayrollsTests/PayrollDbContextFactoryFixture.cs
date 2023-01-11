using Microsoft.EntityFrameworkCore;
using Pms.Payrolls.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Tests
{
    class PayrollDbContextFactoryFixture : IDbContextFactory<PayrollDbContext>
    {
        private const string _connectionString = "server=localhost;database=payroll3Test_efdb;user=root;password=Soft1234;";
        private static bool _databaseInitialized;

        public PayrollDbContextFactoryFixture()
        {
            Factory = new PayrollDbContextFactory(_connectionString);

            if (!_databaseInitialized)
            {
                using var context = Factory.CreateDbContext();
                context.Database.Migrate();
                _databaseInitialized = true;
            }
        }

        public PayrollDbContextFactory Factory { get; set; }

        public PayrollDbContext CreateDbContext()
        {
            return Factory.CreateDbContext();
        }

        public void CreateFactory()
        {
            Factory = new PayrollDbContextFactory(_connectionString);
        }

        private void TrySeeding(PayrollDbContext context)
        {
            Cutoff cutoff = new();
            List<Payroll> payrolls = new()
            {
                Seeder.GenerateSeedPayroll("DYYZ", cutoff.CutoffId, "MANILAIDCSI0000", 10000, 10000, 10000, 1000, -500, -1000),
                Seeder.GenerateSeedPayroll("DYYN", cutoff.CutoffId, "MANILAIDCSI0000", 10000, 10000, 10000, 1000, -500, -1000),
                Seeder.GenerateSeedPayroll("DYYK", cutoff.CutoffId, "MANILAIDCSI0000", 10000, 10000, 10000, 1000, -500, -1000),
            };

            foreach (Payroll payroll in payrolls)
            {
                if (!context.Payrolls.Any(b => b.PayrollId == payroll.PayrollId))
                {
                    context.Add(payroll);
                }
            }

            context.SaveChanges();
        }
    }

}
