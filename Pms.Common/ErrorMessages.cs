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
        public const string PayrollCodeIsEmpty = "Payroll code is empty.";
        public const string SiteIsEmpty = "Site is empty.";
        public const string CutoffIsEmpty = "Cutoff is empty.";
        public const string CompanyIsEmpty = "Company is empty.";
        public const string AdjustmentIsEmpty = "Adjustment is empty.";
    }
}
