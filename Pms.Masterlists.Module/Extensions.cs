using Pms.Masterlists.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module
{
    internal static class Extensions
    {
        internal static IEnumerable<Employee> FilterPayrollCode(this IEnumerable<Employee> employees, string payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? employees.Where(p => p.PayrollCode == payrollCode)
                : employees;
        }

        internal static IEnumerable<Employee> FilterSearchInput(this IEnumerable<Employee> employees, string filter)
        {
            return !string.IsNullOrEmpty(filter)
                ? employees.Where(ts => ts.EEId.Contains(filter) || ts.Fullname.Contains(filter) || ts.CardNumber.Contains(filter) || ts.AccountNumber.Contains(filter))
                : employees;
        }

        internal static IEnumerable<Employee> HideArchived(this IEnumerable<Employee> employees, bool hideArchived)
        {
            return hideArchived
                ? employees.Where(t => t.Active)
                : employees;
        }
    }
}
