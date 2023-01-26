using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public static class ErrorMessages
    {
        public const string HrmsHttpRequestError = "HTTP Request failed. Please check your HRMS Configuration.";

        // main errors
        public const string MainIsNull = "Main is not initialized.";
        public const string PayrollCodeIsNull = "Payroll code is empty.";
        public const string SiteIsNull = "Site is empty.";
        public const string CompanyIsNull = "Company is empty.";
    }
}
