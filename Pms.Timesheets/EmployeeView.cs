using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Pms.Timesheets
{
    public class EmployeeView
    {
        public TimesheetBankChoices Bank { get; init; }
        public string EEId { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;

        public string FullName
        {
            get
            {
                var fullName = string.Join(", ", new[] { LastName, FirstName }.Where(t => !string.IsNullOrEmpty(t)));
                var end = string.Join(" ", new string[] { MiddleName.Length > 0 ? MiddleName[0].ToString() : "", NameExtension }.Where(t => !string.IsNullOrEmpty(t)));
                end += !string.IsNullOrEmpty(end) ? "." : "";
                return string.Join(" ", new[] { fullName, end }.Where(t => !string.IsNullOrEmpty(t)));
            }
        }

        public string LastName { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
        public string MiddleName { get; init; } = string.Empty;
        public string NameExtension { get; init; } = string.Empty;
        public string PayrollCode { get; init; } = string.Empty;
    }
}
