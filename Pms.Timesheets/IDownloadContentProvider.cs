using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public interface IDownloadContentProvider
    {
        Task<DownloadContent<Timesheet>?> DownloadTimesheets(DateTime[] cutoffRange, string payrollCode, int page, string site = "MANILA");
        Task<DownloadSummary<Timesheet>?> GetTimesheetSummary(DateTime[] cutoffRange, string payrollCode, string site = "MANILA");
    }
}
