using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.EfCore
{
    public static class PayrollProviderExtensions
    {
        public static List<string> ExtractCutoffIds(this IEnumerable<Payroll> payrolls) =>
            payrolls
                .GroupBy(ts => ts.CutoffId)
                .Select(ts => ts.First())
                .OrderByDescending(ts => ts.CutoffId)
                .Select(ts => ts.CutoffId)
                .ToList();

        public static IEnumerable<string> CutoffIds(this IEnumerable<Payroll> payrolls)
        {
            return payrolls
                .GroupBy(t => t.CutoffId)
                .Select(t => t.First())
                .OrderByDescending(t => t.CutoffId)
                .Select(t => t.CutoffId);
        }

        //public static List<string> ExtractEEIds(this IEnumerable<Payroll> payrolls) =>
        //    payrolls
        //        .GroupBy(ts => ts.EEId)
        //        .Select(ts => ts.First())
        //        .OrderByDescending(ts => ts.EEId)
        //        .Select(ts => ts.EEId)
        //        .ToList();

        public static List<string> ExtractPayrollCodes(this IEnumerable<Payroll> payrolls) =>
                    payrolls
                .Where(p => p.PayrollCode != string.Empty)
                .GroupBy(ts => ts.PayrollCode)
                .Select(ts => ts.First())
                .OrderByDescending(ts => ts.PayrollCode)
                .Select(ts => ts.PayrollCode)
                .ToList();

        public static IQueryable<Payroll> FilterByPayrollCode(this IQueryable<Payroll> payrolls, string? payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? payrolls.Where(t => t.PayrollCode == payrollCode)
                : payrolls;
        }

        public static IQueryable<Payroll> FilterByCompanyId(this IQueryable<Payroll> payrolls, string? companyId)
        {
            return !string.IsNullOrEmpty(companyId)
                ? payrolls.Where(t => t.CompanyId == companyId)
                : payrolls;
        }

        public static IQueryable<Payroll> FilterByCutoffId(this IQueryable<Payroll> payrolls, string? cutoffId)
        {
            return !string.IsNullOrEmpty(cutoffId)
                ? payrolls.Where(t => t.CutoffId == cutoffId)
                : payrolls;
        }
    }
}
