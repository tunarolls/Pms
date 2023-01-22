using Pms.Timesheets;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Timesheets.Module
{
    public class Timesheets
    {
        private readonly IDownloadContentProvider _downloadProvider;
        private readonly TimesheetManager _timesheetManager;
        private readonly IProvideTimesheetService _timesheetProvider;
        public Timesheets(IProvideTimesheetService timesheetProvider, IDownloadContentProvider downloadProvider, TimesheetManager timesheetManager)
        {
            _timesheetProvider = timesheetProvider;
            _downloadProvider = downloadProvider;
            _timesheetManager = timesheetManager;
        }

        public async Task<IEnumerable<Timesheet>> DownloadContent(Cutoff cutoff, string payrollCodeName, string site, int page)
        {
            DownloadContent<Timesheet>? rawTimesheets = await _downloadProvider.DownloadTimesheets(cutoff.CutoffRange, payrollCodeName, page, site);
            if (rawTimesheets is not null && rawTimesheets.Message is not null)
            {
                foreach (Timesheet timesheet in rawTimesheets.Message)
                    _timesheetManager.SaveTimesheet(timesheet, cutoff.CutoffId, page);

                return rawTimesheets.Message;
            }
            return new List<Timesheet>();
        }

        public async Task<DownloadSummary<Timesheet>?> DownloadContentSummary(Cutoff cutoff, string payrollCode, string site) =>
             await _downloadProvider.GetTimesheetSummary(cutoff.CutoffRange, payrollCode, site);

        public EmployeeView FindEmployeeView(string eeId) =>
            _timesheetProvider.FindEmployeeView(eeId);

        public int[] GetMissingPages(string cutoffId, string payrollCode) =>
            _timesheetProvider.GetMissingPages(cutoffId, payrollCode).ToArray();

        public int[] GetPageWithUnconfirmedTS(Cutoff cutoff, string payrollCode) =>
            _timesheetProvider.GetPageWithUnconfirmedTS(cutoff.CutoffId, payrollCode).ToArray();

        public async Task<ICollection<Timesheet>> GetTimesheets(string cutoffId, CancellationToken cancellationToken = default)
        {
            return await _timesheetProvider.GetTimesheets(cutoffId, cancellationToken);
        }

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId, string payrollCodeId) =>
            _timesheetProvider.GetTimesheets(cutoffId, payrollCodeId);

        public IEnumerable<Timesheet> GetTwoPeriodTimesheets(string cutoffId) =>
            _timesheetProvider.GetTwoPeriodTimesheets(cutoffId);

        public string[] ListCutoffIds() =>
            _timesheetProvider.GetTimesheets().ExtractCutoffIds().ToArray();

        public string[] ListPayrollCodes() =>
            _timesheetProvider.GetTimesheets().ExtractPayrollCodes().ToArray();

        public IEnumerable<string> ListTimesheetNoEETimesheet(string cutoffId)
        {
            try
            {
                var da = _timesheetProvider.GetTimesheetNoEETimesheet(cutoffId)
                    .Select(ts => ts.EEId)
                    .ToList();
                return da;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Enumerable.Empty<string>();
        }

        public IEnumerable<Timesheet> MapEmployeeView(Timesheet[]? timesheets)
        {
            if (timesheets == null) return Enumerable.Empty<Timesheet>();

            List<Timesheet> mappedTimesheets = new();

            foreach (Timesheet timesheet in timesheets)
            {
                timesheet.EE = _timesheetProvider.FindEmployeeView(timesheet.EEId);
                mappedTimesheets.Add(timesheet);
            }

            return mappedTimesheets;
        }

        public void SaveTimesheet(Timesheet? timesheet)
        {
            if (timesheet == null) throw new ArgumentNullException(nameof(timesheet));
            _timesheetManager.SaveTimesheet(timesheet, timesheet.CutoffId, 0);
        }

        public async Task SaveTimesheet(Timesheet? timesheet, CancellationToken cancellationToken = default)
        {
            if (timesheet == null) throw new ArgumentNullException(nameof(timesheet));
            await _timesheetManager.SaveTimesheet(timesheet, timesheet.CutoffId, 0, cancellationToken);
        }
    }
}
