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

        public string Fullname
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName)) return "";
                string fullName = $"{LastName}, {FirstName}";
                return string.IsNullOrEmpty(MiddleName) ? $"{fullName}." : $"{fullName} {MiddleName[..1]}.";
            }
        }
    }
}
