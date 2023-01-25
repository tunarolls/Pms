using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.ServiceLayer.EfCore
{
    public static class TimesheetProviderExtensions
    {
        public static IEnumerable<Timesheet> FilterByCutoffId(this IEnumerable<Timesheet> timesheets, string cutoffId)
        {
            return timesheets.Where(ts => ts.CutoffId == cutoffId);
        }

        public static IQueryable<Timesheet> FilterByCutoffId(this IQueryable<Timesheet> timesheets, string cutoffId)
        {
            return timesheets.Where(t => t.CutoffId == cutoffId);
        }

        public static IEnumerable<Timesheet> FilterByPayrollCode(this IEnumerable<Timesheet> timesheets, string payrollCode) =>
           timesheets
                .Where(ts => ts.EE is not null)
                .Where(ts => ts.EE.PayrollCode == payrollCode);

        public static IEnumerable<Timesheet> FilterByBank(this IEnumerable<Timesheet> timesheets, TimesheetBankChoices bank) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .Where(ts => ts.EE.Bank == bank);

        public static IEnumerable<Timesheet> ByExportable(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .Where(ts => ts.IsConfirmed && ts.TotalHours > 0)
                .OrderBy(ts => ts.EE.Fullname);

        public static IEnumerable<Timesheet> ByUnconfirmedWithoutAttendance(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .Where(ts => !ts.IsConfirmed && ts.TotalHours == 0)
                .OrderBy(ts => ts.EE.Fullname);

        public static IEnumerable<Timesheet> ByUnconfirmedWithAttendance(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .Where(ts => !ts.IsConfirmed && ts.TotalHours > 0)
                .OrderBy(ts => ts.EE.Fullname);







        public static List<int> GroupByPage(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .GroupBy(ts => ts.Page, ts => ts.Page)
                .Select((page, i) => page.First())
                .ToList();

        public static IEnumerable<Timesheet[]> GroupTimesheetsByEEId(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .GroupBy(ts => ts.EEId)
                .Select(tss => tss.ToArray());




        public static List<string> ExtractCutoffIds(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .GroupBy(ts => ts.CutoffId)
                .Select(ts => ts.First())
                .OrderByDescending(ts => ts.CutoffId)
                .Select(ts => ts.CutoffId)
                .ToList();


        public static List<TimesheetBankChoices> ExtractBanks(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .GroupBy(ts => ts.EE.Bank)
                .Select(ts => ts.First())
                .OrderBy(ts => ts.EE.Bank)
                .Select(ts => ts.EE.Bank).ToList();


        public static List<string> ExtractPayrollCodes(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .Where(ts => ts.EE.PayrollCode != "")
                .GroupBy(ts => ts.EE.PayrollCode)
                .Select(ts => ts.First())
                .OrderBy(ts => ts.EE.PayrollCode)
                .Select(ts => ts.EE.PayrollCode).ToList();

        

    }
}
