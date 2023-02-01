using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models
{
    public class EmployeeView
    {
        public string EEId { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string MiddleName { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string PayrollCode { get; private set; } = string.Empty;

        public string FullName
        {
            get
            {
                var fullName = string.Join(", ", new[] { LastName, FirstName }.Where(t => !string.IsNullOrWhiteSpace(t)));
                var end = MiddleName.Length > 0 ? MiddleName[0].ToString() : "";
                end += !string.IsNullOrWhiteSpace(end) ? "." : "";
                return string.Join(" ", new[] { fullName, end }.Where(t => !string.IsNullOrWhiteSpace(t)));
            }
        }
    }
}
