using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public class ViewNames
    {
        public const string Timesheets = nameof(Timesheets);
        public const string TimesheetDetailView = nameof(TimesheetDetailView);

        public const string PayrollsView = nameof(PayrollsView);
        public const string ImportAlphalistView = nameof(ImportAlphalistView);

        // adjustments navigation
        public const string BillingListingView = nameof(BillingListingView);
        public const string BillingRecordListingView = nameof(BillingRecordListingView);
        public const string BillingRecordDetailView = nameof(BillingRecordDetailView);

        // masterlists navigation
        public const string EmployeeListingView = nameof(EmployeeListingView);
        public const string EmployeeDetailView = nameof(EmployeeDetailView);
        public const string PayrollCodeDetailView = nameof(PayrollCodeDetailView);
        public const string SelectDateView = nameof(SelectDateView);
    }
}
