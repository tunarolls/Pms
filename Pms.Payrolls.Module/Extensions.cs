using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Module
{
    internal static class Extensions
    {
        internal static IEnumerable<Payroll> SetCompanyId(this IEnumerable<Payroll> payrolls, string companyId)
        {
            return !string.IsNullOrEmpty(companyId) ? payrolls.Where(t => t.CompanyId == companyId) : payrolls;
        }

        internal static IEnumerable<Payroll> SetPayrollCode(this IEnumerable<Payroll> payrolls, string payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode) ? payrolls.Where(t => t.PayrollCode == payrollCode) : payrolls;
        }
    }
}
