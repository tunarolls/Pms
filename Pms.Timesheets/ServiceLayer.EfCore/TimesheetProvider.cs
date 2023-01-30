using Microsoft.EntityFrameworkCore;
using Pms.Timesheets.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Timesheets.ServiceLayer.EfCore
{
    public class TimesheetProvider : IProvideTimesheetService
    {
        private readonly IDbContextFactory<TimesheetDbContext> _factory;
        public TimesheetProvider(IDbContextFactory<TimesheetDbContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Timesheet> GetTimesheets()
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            return Context.Timesheets.Include(ts => ts.EE).ToList();
        }

        public async Task<ICollection<Timesheet>> GetTimesheets(CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Timesheets.Include(t => t.EE).ToListAsync(cancellationToken);
        }

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            IEnumerable<Timesheet> timesheetsWithEE = Context.Timesheets
                .Include(ts => ts.EE).ToList()
                .FilterByCutoffId(cutoffId);

            IEnumerable<Timesheet> allTimesheets = Context.Timesheets.ToList()
                .FilterByCutoffId(cutoffId);

            return timesheetsWithEE.Union(allTimesheets);
        }

        public async Task<ICollection<Timesheet>> GetTimesheets(string cutoffId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Timesheets
                .Include(t => t.EE)
                .FilterByCutoffId(cutoffId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<Timesheet>> GetTimesheets(string cutoffId, string? payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            //var timesheets = from t in context.Timesheets
            //                 join e in context.Employees on t.EEId equals e.EEId into grouping
            //                 from g in grouping.DefaultIfEmpty()
            //                 select t;
            //return await timesheets
            //    .FilterByCutoffId(cutoffId)
            //    .FilterByPayrollCode(payrollCode)
            //    .ToListAsync(cancellationToken);
            return await context.Timesheets
                .Include(t => t.EE)
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode)
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId, string payrollCodeId)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            IEnumerable<Timesheet> timesheetsWithEE = Context.Timesheets
                .Include(ts => ts.EE).ToList()
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCodeId);

            IEnumerable<Timesheet> allTimesheets = Context.Timesheets.ToList()
                .FilterByCutoffId(cutoffId);

            return timesheetsWithEE.Union(allTimesheets);
        }
        public IEnumerable<Timesheet> GetTwoPeriodTimesheets(string cutoffId)
        {
            Cutoff currentCutoff = new(cutoffId);
            using TimesheetDbContext Context = _factory.CreateDbContext();
            return Context.Timesheets
                .Include(ts => ts.EE).ToList()
                .Where(ts => ts.CutoffId == cutoffId || ts.CutoffId == currentCutoff.GetPreviousCutoff())
                .OrderBy(ts => ts.CutoffId);
        }

        public async Task<ICollection<Timesheet>> GetTwoPeriodTimesheets(string cutoffId, CancellationToken cancellationToken = default)
        {
            var currentCutoff = new Cutoff(cutoffId);
            using var context = _factory.CreateDbContext();
            return await context.Timesheets
                .Include(t => t.EE)
                .Where(t => t.CutoffId == cutoffId || t.CutoffId == currentCutoff.GetPreviousCutoff())
                .OrderBy(t => t.CutoffId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<Timesheet>> GetTwoPeriodTimesheets(string cutoffId, string payrollCode, CancellationToken cancellationToken = default)
        {
            var currentCutoff = new Cutoff(cutoffId);
            using var context = _factory.CreateDbContext();
            return await context.Timesheets
                .Include(t => t.EE)
                .Where(t => t.CutoffId == cutoffId || t.CutoffId == currentCutoff.GetPreviousCutoff())
                .Where(t => t.EE != null && t.EE.PayrollCode == payrollCode)
                .OrderBy(t => t.CutoffId)
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<Timesheet> GetTimesheetsByMonth(int month)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            return Context.Timesheets
                .Include(ts => ts.EE).ToList()
                .Where(ts => ts.Cutoff.CutoffDate.Month == month)
                .OrderBy(ts => ts.CutoffId);
        }

        public IEnumerable<Timesheet> GetTimesheetNoEETimesheet(string cutoffId)
        {
            TimesheetDbContext Context = _factory.CreateDbContext();
            IEnumerable<Timesheet> validTimesheets = Context.Timesheets
                .Include(ts => ts.EE)
                .Where(ts => ts.EE != null && ts.EE.PayrollCode != "")
                .FilterByCutoffId(cutoffId);
            IEnumerable<Timesheet> timesheets = Context.Timesheets
                .FilterByCutoffId(cutoffId);
            timesheets = timesheets.Except(validTimesheets);
            Console.WriteLine(timesheets.Count());

            return timesheets;
        }


        public int GetLastPage(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            IEnumerable<Timesheet> timesheets = Context.Timesheets
                .OrderByDescending(ts => ts.TotalHours)
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode);

            if (timesheets.Any())
                return timesheets.Max(ts => ts.Page);

            return 0;
        }

        public List<int> GetPageWithUnconfirmedTS(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            IEnumerable<Timesheet> timesheets = Context.Timesheets
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode);

            timesheets = timesheets.Where(ts =>
                !ts.IsConfirmed &&
                ts.TotalHours > 0
            );

            return timesheets.GroupByPage();
        }

        public List<int> GetPages(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            return Context.Timesheets
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode)
                .GroupByPage();

        }

        public List<int> GetMissingPages(string cutoffId, string payrollCode)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            List<int> pages = Context.Timesheets
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode)
                .ToList()
                .GroupByPage();

            if (pages.Count > 0)
            {
                List<int> assumedPages = Enumerable.Range(0, pages.Max()).ToList();
                if (pages.Count > assumedPages.Count)
                    return assumedPages.Except(pages).ToList();
            }

            return new List<int>();
        }

        public async Task<int[]> GetMissingPages(string cutoffId, string payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Timesheets
                .FilterByCutoffId(cutoffId)
                .FilterByPayrollCode(payrollCode)
                .GroupBy(t => t.Page, t => t.Page)
                .Select((page, _) => page.First())
                .ToArrayAsync(cancellationToken);
        }

        public EmployeeView FindEmployeeView(string? eeId)
        {
            using TimesheetDbContext Context = _factory.CreateDbContext();
            return Context.Employees.Find(eeId);
        }

        public async Task<EmployeeView?> FindEmployeeView(string? eeId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Employees.FindAsync(new object[] { eeId ?? string.Empty }, cancellationToken);
        }
    }
}
