using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class DownloadContentProvider : IDownloadContentProvider
    {
        TimeDownloaderAdapter _adapter;

        public DownloadContentProvider(TimeDownloaderAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task<DownloadContent<Timesheet>?> DownloadTimesheets(DateTime[] cutoffRange, string payrollCode, int page, string site = "MANILA")
        {
            return await _adapter.GetPageContent<DownloadContent<Timesheet>>(cutoffRange[0], cutoffRange[1], page, payrollCode, site);
        }


        public async Task<DownloadSummary<Timesheet>?> GetTimesheetSummary(DateTime[] cutoffRange, string payrollCode, string site = "MANILA")
        {
            return await _adapter.GetSummary<DownloadSummary<Timesheet>>(cutoffRange[0], cutoffRange[1], payrollCode, site);
        }
    }
}
