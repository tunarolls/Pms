using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common.Exceptions
{
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(string employeeId)
        {
            EmployeeId = employeeId;
        }

        public string EmployeeId { get; }
    }
}
