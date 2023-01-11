using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
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

        public static List<string> ExtractEEIds(this IEnumerable<Payroll> payrolls) =>
            payrolls
                .GroupBy(ts => ts.EEId)
                .Select(ts => ts.First())
                .OrderByDescending(ts => ts.EEId)
                .Select(ts => ts.EEId)
                .ToList();

        public static List<string> ExtractPayrollCodes(this IEnumerable<Payroll> payrolls) =>
                    payrolls
                .Where(p=>p.PayrollCode !=string.Empty)
                .GroupBy(ts => ts.PayrollCode)
                .Select(ts => ts.First())
                .OrderByDescending(ts => ts.PayrollCode)
                .Select(ts => ts.PayrollCode)
                .ToList();
    }
}
