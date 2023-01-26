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
    public class PayrollProvider : IProvidePayrollService
    {
        private IDbContextFactory<PayrollDbContext> _factory;

        public PayrollProvider(IDbContextFactory<PayrollDbContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Payroll> GetAllPayrolls()
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return context.Payrolls
                .Include(p => p.EE)
                .ToList();
        }

        public async Task<ICollection<Payroll>> GetAllPayrolls(CancellationToken cancellationToken = default)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return await context.Payrolls.Include(t => t.EE).ToListAsync(cancellationToken);
        }

        public async Task<ICollection<Payroll>> GetYearlyPayrolls(int year, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Payrolls
                .Include(t => t.EE)
                .Where(t => t.YearCovered == year)
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<Payroll> GetPayrolls(string cutoffId)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return context.Payrolls
                .Include(p => p.EE)
                .Where(p => p.CutoffId == cutoffId)
                .ToList();
        }

        public async Task<ICollection<Payroll>> GetPayrolls(string cutoffId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var p = await context.Payrolls
                .Include(t => t.EE)
                .Where(t => t.CutoffId == cutoffId)
                .ToListAsync(cancellationToken);
            return p;
        }

        public IEnumerable<Payroll> GetPayrolls(string cutoffId, string payrollCode)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return context.Payrolls
                .Include(p => p.EE)
                .Where(p => p.CutoffId == cutoffId)
                .Where(p => p.PayrollCode == payrollCode)
                .ToList();
        }

        public async Task<ICollection<Payroll>> GetPayrolls(string cutoffId, string payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Payrolls
                .Include(t => t.EE)
                .Where(t => t.CutoffId == cutoffId && t.PayrollCode == payrollCode)
                .ToListAsync(cancellationToken);
        }
        
        public IEnumerable<Payroll> GetPayrolls(int yearsCovered, string companyId)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return context.Payrolls
                .Include(p => p.EE).ToList()
                .Where(p =>
                    p.YearCovered == yearsCovered ||
                    p.Cutoff.CutoffDate.Year == yearsCovered
                )
                .Where(p => p.CompanyId == companyId)
                .ToList();
        }

        public async Task<ICollection<Payroll>> GetPayrolls(int yearsCovered, string companyId, CancellationToken cancellationToken = default)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return await context.Payrolls
                .Include(t => t.EE)
                .Where(t => t.YearCovered == yearsCovered || t.Cutoff.CutoffDate.Year == yearsCovered)
                .Where(t => t.CompanyId == companyId)
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<Payroll> GetPayrollsByCcompany(string cutoffId, string CompanyId)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return context.Payrolls
                .Include(p => p.EE)
                .Where(p => p.CutoffId == cutoffId)
                .Where(p => p.CompanyId == CompanyId)
                .ToList();
        }

        public IEnumerable<Payroll> GetNoEEPayrolls()
        {
            PayrollDbContext Context = _factory.CreateDbContext();
            IEnumerable<Payroll> validPayrolls = Context.Payrolls
                .Include(ts => ts.EE);
            IEnumerable<Payroll> payrolls = Context.Payrolls;
            payrolls = payrolls.Except(validPayrolls);
            Console.WriteLine(payrolls.Count());

            return payrolls;
        }

        public IEnumerable<MonthlyPayroll> GetMonthlyPayrolls(int month, string payrollCode)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            List<Payroll> payrolls = context.Payrolls.Include(p => p.EE).ToList();

            List<MonthlyPayroll> mpayrolls = payrolls.Where(p => p.Cutoff.CutoffDate.Month == month)
            .Where(p => p.CompanyId == payrollCode)
            .GroupBy(p => p.EEId)
            .Where(p => p.Any(p => p.Cutoff.CutoffDate.Day >= 28))
            .Select(p => new MonthlyPayroll(p.ToArray()))
            .ToList();

            return mpayrolls;
        }

        public async Task<ICollection<MonthlyPayroll>> GetMonthlyPayrolls(int month, string payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var payrolls = await context.Payrolls.Include(t => t.EE).ToListAsync(cancellationToken);
            return payrolls.Where(t => t.Cutoff.CutoffDate.Month == month)
                .Where(t => t.CompanyId == payrollCode)
                .GroupBy(t => t.EEId)
                .Where(t => t.Any(t => t.Cutoff.CutoffDate.Day >= 28))
                .Select(t => new MonthlyPayroll(t.ToArray()))
                .ToList();
        }
    }
}
