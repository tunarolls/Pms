using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.AdjustmentsTests
{
    public class AdjustmentDbContextFactoryFixture : IDbContextFactory<AdjustmentDbContext>
    {
        private const string _connectionString = "server=192.168.56.101;database=payroll3Test_efdb;user=tim;password=tim@123;";
        private static bool _databaseInitialized;

        public AdjustmentDbContextFactoryFixture()
        {
            Factory = new(_connectionString);

            if (!_databaseInitialized)
            {
                using var context = Factory.CreateDbContext();
                context.Database.Migrate();
                TrySeeding(context);
                _databaseInitialized = true;
            }
        }

        public AdjustmentDbContextFactory Factory { get; set; }
        public AdjustmentDbContext CreateDbContext()
            => Factory.CreateDbContext();

        public void CreateFactory()
            => Factory = new AdjustmentDbContextFactory(_connectionString);

        private Billing AddSeedBilling(string eeId, string cutoffId, string adjustmentName, double amount, int iterator = 0)
        {
            Billing billing = new()
            {
                EEId = eeId,
                CutoffId = cutoffId,
                Amount = amount,
                Deducted = true,
                DateCreated = DateTime.Now
            };
            billing.BillingId = Billing.GenerateId(billing, iterator);

            return billing;
        }

        private void TrySeeding(AdjustmentDbContext context)
        {
            Cutoff cutoff = new();
            List<Billing> billings = new()
            {
                AddSeedBilling("DYYJ", cutoff.CutoffId, "PCV",  300),
                AddSeedBilling("DYYJ", cutoff.CutoffId, "ALLOWANCE", 300),
                AddSeedBilling("DYYJ", "2207-1", "PCV",  300),
                AddSeedBilling("FJFC",  "2207-1", "ALLOWANCE", 1000),
            };

            foreach (Billing billing in billings)
            {
                if (!context.Billings.Any(b => b.BillingId == billing.BillingId))
                    context.Add(billing);
            }
            context.SaveChanges();
        }
    }
}
