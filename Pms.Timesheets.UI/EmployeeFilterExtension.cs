using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.Module
{
    internal static class EmployeeFilterExtension
    {
        internal static IEnumerable<Timesheet> FilterPayrollCode(this IEnumerable<Timesheet> timesheets, string payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? timesheets.Where(t => t.EE != null && t.EE.PayrollCode == payrollCode)
                : timesheets.Where(t => t.EE != null);
        }

        internal static IEnumerable<Timesheet> FilterSearchInput(this IEnumerable<Timesheet> timesheets, string filter)
        {
            return !string.IsNullOrEmpty(filter)
                ? timesheets.Where(t => t.EEId.Contains(filter) || (t.EE != null && t.EE.Fullname.Contains(filter)))
                : timesheets;
        }
    }
}
