using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class TimesheetProvider : IProvideTimesheetService
    {
        private IDbContextFactory<TimesheetDbContext> _factory;
        public TimesheetProvider(IDbContextFactory<TimesheetDbContext> factory)
        {
            _factory = factory;
        }

        public int GetLastPage(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            IEnumerable<Timesheet> timesheets = context.Timesheets
                .OrderByDescending(ts => ts.TotalHours)
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode);
            return timesheets.Any() ? timesheets.Max(ts => ts.Page) : 0;
        }

        public List<int> GetMissingPages(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            List<int> pages = context.Timesheets
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode)
                .ToList()
                .GroupByPage();
            if (pages.Any())
            {
                List<int> assumedPages = Enumerable.Range(0, pages.Max()).ToList();
                if (pages.Count > assumedPages.Count) return assumedPages.Except(pages).ToList();
            }

            return new List<int>();
        }

        public List<int> GetPages(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            return context.Timesheets
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode)
                .GroupByPage();

        }

        public List<int> GetPageWithUnconfirmedTS(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            IEnumerable<Timesheet> timesheets = context.Timesheets
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode);
            timesheets = timesheets.Where(ts => !ts.IsConfirmed && ts.TotalHours > 0);
            return timesheets.GroupByPage();
        }

        public IEnumerable<Timesheet> GetTimesheetNoEETimesheet(string cutoffId)
        {
            TimesheetDbContext context = _factory.CreateDbContext();
            IEnumerable<Timesheet> validTimesheets = context.Timesheets
                .Include(ts => ts.EE)
                .Where(ts => ts.EE != null && !string.IsNullOrEmpty(ts.EE.PayrollCode))
                .FilterByCutoffId(cutoffId);
            IEnumerable<Timesheet> timesheets = context.Timesheets
                .FilterByCutoffId(cutoffId);
            timesheets = timesheets.Except(validTimesheets);
            Console.WriteLine(timesheets.Count());

            return timesheets;
        }

        public IEnumerable<Timesheet> GetTimesheets()
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            return context.Timesheets.Include(ts => ts.EE).ToList();
        }
        public IEnumerable<Timesheet> GetTimesheets(string cutoffId)
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            return context.Timesheets
                .Include(ts => ts.EE).ToList()
                .FilterByCutoffId(cutoffId);
        }
        public IEnumerable<Timesheet> GetTimesheetsByMonth(int month)
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            return context.Timesheets
                .Include(ts => ts.EE).ToList()
                .Where(ts => ts.Cutoff.CutoffDate.Month == month)
                .OrderBy(ts => ts.CutoffId);
        }

        public IEnumerable<Timesheet> GetTwoPeriodTimesheets(string cutoffId)
        {
            Cutoff currentCutoff = new Cutoff(cutoffId);
            using TimesheetDbContext context = _factory.CreateDbContext();
            return context.Timesheets
                .Include(ts => ts.EE).ToList()
                .Where(ts => ts.CutoffId == cutoffId || ts.CutoffId == currentCutoff.GetPreviousCutoff())
                .OrderBy(ts => ts.CutoffId);
        }
    }
}
