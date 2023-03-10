using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using Pms.Adjustments.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.ServiceLayer.EfCore
{
    public class BillingProvider : IProvideBillingService
    {
        protected IDbContextFactory<AdjustmentDbContext> _factory;

        public BillingProvider(IDbContextFactory<AdjustmentDbContext> factory)
        {
            _factory = factory;
        }


        public double GetTotalAdvances(string eeId, string cutoffId)
        {
            var context = _factory.CreateDbContext();
            return context.Billings
                .Where(b => b.EEId == eeId)
                .Where(b => b.CutoffId == cutoffId)
                .Where(b => b.Applied)
                .Sum(b => b.Amount);
        }

        //public IEnumerable<Billing> GetBillings(string eeId, string cutoffId)
        //{
        //    var context = _factory.CreateDbContext();
        //    return context.Billings
        //        .Where(b => b.EEId == eeId)
        //        .Where(b => b.CutoffId == cutoffId)
        //        .ToList();
        //}

        public IEnumerable<Billing> GetBillings(string cutoffId)
        {
            var context = _factory.CreateDbContext();
            return context.Billings
                .Include(b => b.EE)
                .Where(b => b.CutoffId.Contains(cutoffId))
                .ToList();
        }

        public async Task<ICollection<Billing>> GetBillings(string cutoffId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Billings.Include(t => t.EE).Where(t => t.CutoffId == cutoffId).ToListAsync(cancellationToken);
        }

        public async Task<ICollection<Billing>> GetBillings(string? cutoffId, string? payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Billings
                .Include(t => t.EE)
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode)
                .ToListAsync(cancellationToken);
        }

        //public IEnumerable<Billing> GetBillings(string cutoffId, string adjustmentName)
        //{
        //    var context = _factory.CreateDbContext();
        //    return context.Billings
        //        .Where(b => b.AdjustmentName == adjustmentName)
        //        .Where(b => b.CutoffId == cutoffId)
        //        .ToList();
        //}

    }
}
