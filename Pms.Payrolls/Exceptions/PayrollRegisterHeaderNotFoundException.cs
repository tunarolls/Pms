using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Exceptions
{
    public class PayrollRegisterHeaderNotFoundException : Exception
    {
        public PayrollRegisterHeaderNotFoundException(string header, string payrollRegisterFilePath)
        {
            Header = header;
            PayrollRegisterFilePath = payrollRegisterFilePath;
        }

        public string Header { get; set; }
        public string PayrollRegisterFilePath { get; set; }
    }
}
