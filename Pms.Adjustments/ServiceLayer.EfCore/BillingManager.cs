using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using Pms.Adjustments.Services;
using System;
using System.Linq;

namespace Pms.Adjustments.ServiceLayer.EfCore
{
    public class BillingManager : IManageBillingService
    {
        protected IDbContextFactory<AdjustmentDbContext> _factory;

        public BillingManager(IDbContextFactory<AdjustmentDbContext> factory)
        {
            _factory = factory;
        }

        private void ValidateCutoffId(string cutoffId)
        {
            Cutoff cutoff = new(cutoffId);
            //TODO: UNCOMMENT ONCE DONE. COMMENTED TO TEST USING OLD DATA.
            //if (cutoff.CutoffDate < DateTime.Now)
            //    throw new OldBillingException("", cutoffId);
        }

        public void AddBilling(Billing billing)
        {
            ValidateCutoffId(billing.CutoffId);

            using AdjustmentDbContext context = _factory.CreateDbContext();
            context.Add(billing);
            context.SaveChanges();
        }

        public void ResetBillings(string eeId, string cutoffId)
        {
            ValidateCutoffId(cutoffId);

            using AdjustmentDbContext context = _factory.CreateDbContext();
            IQueryable<Billing> eeBillings = context.Billings.Where(b => b.EEId == eeId && b.CutoffId == cutoffId);
            context.Billings.RemoveRange(eeBillings);
            context.SaveChanges();
        }
    }
}
