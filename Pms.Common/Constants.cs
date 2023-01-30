using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public class PmsConstants
    {
        public const string ConfigFilename = "appsettings.json";
        public const string DevelopmentConnectionName = "Development";
        public const string ProductionConnectionName = "Production";

        public const string Timesheets = nameof(Timesheets);
        public const string Timesheet = nameof(Timesheet);
        public const string Site = nameof(Site);
        public const string PayrollCode = nameof(PayrollCode);
        public const string Cutoff = nameof(Cutoff);
        public const string Main = nameof(Main);
        public const string SelectedDate = nameof(SelectedDate);
        public const string Employee = nameof(Employee);
        public const string BillingRecord = nameof(BillingRecord);
    }

    public class DialogNames
    {
        public const string CancelDialog = nameof(CancelDialog);
        public const string PromptDialog = nameof(PromptDialog);
    }

    public class DialogParameterNames
    {
        public const string Message = nameof(Message);
        public const string Title = nameof(Title);
        public const string TaskCompletion = nameof(TaskCompletion);
        public const string CancellationTokenSource = nameof(CancellationTokenSource);
        public const string PromptDialogButton = nameof(PromptDialogButton);
        public const string PromptDialogResult = nameof(PromptDialogResult);
        public const string MoreInfo = nameof(MoreInfo);
    }

    public class FileFilters
    {
        public const string BillingRecordImport = "Billing Record Import Files(*.xls)|*.xls";
    }
}
