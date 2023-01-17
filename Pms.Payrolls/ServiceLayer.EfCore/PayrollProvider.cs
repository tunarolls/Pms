using Microsoft.EntityFrameworkCore;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return context.Payrolls.ToList();
        }

        public IEnumerable<Payroll> GetMonthlyPayrolls(int month, string payrollCode)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return context.Payrolls
                .Include(p => p.EE)
                .Where(p => p.Cutoff.CutoffDate.Month == month)
                .Where(p => p.PayrollCode == payrollCode)
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

        public IEnumerable<Payroll> GetPayrolls(string cutoffId)
        {
            using PayrollDbContext context = _factory.CreateDbContext();
            return context.Payrolls
                .Include(p => p.EE)
                .Where(p => p.CutoffId == cutoffId)
                .ToList();
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
    }
}
