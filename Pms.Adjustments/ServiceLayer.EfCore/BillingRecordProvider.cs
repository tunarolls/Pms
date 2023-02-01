using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.ServiceLayer.EfCore
{
    public class BillingRecordProvider
    {
        protected IDbContextFactory<AdjustmentDbContext> _factory;

        public BillingRecordProvider(IDbContextFactory<AdjustmentDbContext> factory)
        {
            _factory = factory;
        }


        public IEnumerable<BillingRecord> GetBillingRecords()
        {
            using var context = _factory.CreateDbContext();
            return context.BillingRecords.Include(r => r.EE).ToList();
        }

        public IEnumerable<BillingRecord> GetBillingRecordsByPayrollCode(string payrollCode)
        {
            using var context = _factory.CreateDbContext();
            return context.BillingRecords
                .Include(r => r.EE)
                .Where(r => r.EE != null && r.EE.PayrollCode == payrollCode)
                .ToList();
        }

        public async Task<ICollection<BillingRecord>> GetBillingRecordsByPayrollCode(string payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.BillingRecords
                .Include(t => t.EE)
                .Where(t => t.EE != null && t.EE.PayrollCode == payrollCode)
                .ToListAsync(cancellationToken);
        }
    }
}
