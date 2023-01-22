using Microsoft.EntityFrameworkCore;
using Pms.Common;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.EfCore
{
    public class PayrollCodeManager
    {
        private readonly IDbContextFactory<EmployeeDbContext> _factory;
        public PayrollCodeManager(IDbContextFactory<EmployeeDbContext> factory)
        {
            _factory = factory;
        }

        public IQueryable<PayrollCode> GetPayrollCodes()
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            return Context.PayrollCodes.AsNoTracking();
        }

        public async Task<ICollection<PayrollCode>> GetPayrollCodes(CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.PayrollCodes.ToListAsync(cancellationToken);
        }

        public void SavePayrollCode(PayrollCode payrollCode)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            if (Context.PayrollCodes.Any(pc => pc.PayrollCodeId == payrollCode.PayrollCodeId))
                Context.Update(payrollCode);
            else
                Context.Add(payrollCode);

            Context.SaveChanges();
        }

        public async Task SavePayrollCode(PayrollCode payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();

            if (await context.PayrollCodes.AnyAsync(t => t.PayrollCodeId == payrollCode.PayrollCodeId, cancellationToken))
            {
                context.Update(payrollCode);
            }
            else
            {
                await context.AddAsync(payrollCode, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
