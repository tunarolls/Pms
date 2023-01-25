using Microsoft.EntityFrameworkCore;
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
    public class BillingRecordManager
    {
        protected IDbContextFactory<AdjustmentDbContext> _factory;

        public BillingRecordManager(IDbContextFactory<AdjustmentDbContext> factory)
        {
            _factory = factory;
        }

        public void Save(BillingRecord record)
        {
            record.Validate();

            using var context = _factory.CreateDbContext();
            if (context.BillingRecords.Any(r => r.RecordId == record.RecordId))
                context.Update(record);
            else
            {
                record.DateCreated = DateTime.Now;
                context.Add(record);
            }
            context.SaveChanges();
        }

        public async Task Save(BillingRecord record, CancellationToken cancellationToken = default)
        {
            record.Validate();

            using var context = _factory.CreateDbContext();
            
            if (await context.BillingRecords.AnyAsync(t => t.RecordId == record.RecordId, cancellationToken))
            {
                context.Update(record);
            }
            else
            {
                record.DateCreated = DateTime.Now;
                await context.AddAsync(record, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
