using Microsoft.EntityFrameworkCore;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.EfCore
{
    public class PayrollManager : IManagePayrollService
    {
        private IDbContextFactory<PayrollDbContext> _factory;

        public PayrollManager(IDbContextFactory<PayrollDbContext> factory) =>
            _factory = factory;

        public void SavePayroll(Payroll payroll)
        {
            payroll.Validate();

            using PayrollDbContext context = _factory.CreateDbContext();
            if (context.Payrolls.Any(p => p.PayrollId == payroll.PayrollId))
                context.Update(payroll);
            else
                context.Add(payroll);
            context.SaveChanges();
        }

        public async Task SavePayroll(Payroll payroll, CancellationToken cancellationToken = default)
        {
            payroll.Validate();
            using var context = _factory.CreateDbContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (await context.Payrolls.AnyAsync(t => t.PayrollId == payroll.PayrollId, cancellationToken))
                {
                    context.Update(payroll);
                }
                else
                {
                    await context.AddAsync(payroll, cancellationToken);
                }

                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public void ValidatePayroll(Payroll payroll)
        {

        }
    }
}
