using Pms.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Timesheets.ServiceLayer.TimeSystem
{
    public interface IDownloadContentProvider
    {
        Task<DownloadContent<Timesheet>?> DownloadTimesheets(DateTime[] cutoffRange, string payrollCode, int page, string site = "MANILA");
        Task<DownloadContent<Timesheet>?> DownloadTimesheets(DateTime[] cutoffRange, string payrollCode,
            int page, SiteChoices site = SiteChoices.MANILA, CancellationToken cancellationToken = default);
        Task<DownloadSummary<Timesheet>?> GetTimesheetSummary(DateTime[] cutoffRange, string payrollCode, string site = "MANILA");
        Task<DownloadSummary<Timesheet>?> GetTimesheetSummary(DateTime[] cutoffRange, string payrollCode,
            SiteChoices site = SiteChoices.MANILA, CancellationToken cancellationToken = default);
    }
}
