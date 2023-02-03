using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using Pms.Adjustments.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            //Cutoff cutoff = new(cutoffId);
            //TODO: UNCOMMENT ONCE DONE. COMMENTED TO TEST USING OLD DATA.
            //if (cutoff.CutoffDate < DateTime.Now)
            //    throw new OldBillingException("", cutoffId);
        }

        public void AddBilling(Billing billing)
        {
            ValidateCutoffId(billing.CutoffId);

            using AdjustmentDbContext context = _factory.CreateDbContext();
            if (context.Billings.Any(b => b.BillingId == billing.BillingId))
                context.Update(billing);
            else
                context.Add(billing);

            context.SaveChanges();
        }

        public async Task AddBilling(Billing billing, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            if (await context.Billings.AnyAsync(t => t.BillingId == billing.BillingId, cancellationToken))
            {
                context.Entry(billing).State = EntityState.Modified;
                //context.Update(billing);
            }
            else
            {
                await context.AddAsync(billing, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        public void ResetBillings(string eeId, string cutoffId)
        {
            ValidateCutoffId(cutoffId);

            using AdjustmentDbContext context = _factory.CreateDbContext();
            IQueryable<Billing> eeBillings = context.Billings.Where(b => b.EEId == eeId && b.CutoffId == cutoffId && b.Applied);
            context.Billings.RemoveRange(eeBillings);
            context.SaveChanges();
        }

        public async Task ResetBillings(string eeId, string cutoffId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var eeBillings = context.Billings.Where(t => t.EEId == eeId && t.CutoffId == cutoffId && t.Applied);
            context.Billings.RemoveRange(eeBillings);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
