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

        public static IQueryable<Timesheet> FilterByCutoffId(this IQueryable<Timesheet> timesheets, string? cutoffId)
        {
            return string.IsNullOrEmpty(cutoffId)
                ? timesheets
                : timesheets.Where(t => t.CutoffId == cutoffId);
        }

        public static IEnumerable<Timesheet> FilterByPayrollCode(this IEnumerable<Timesheet> timesheets, string payrollCode) =>
           timesheets.Where(ts => ts.EE != null && ts.EE.PayrollCode == payrollCode);

        public static IQueryable<Timesheet> FilterByPayrollCode(this IQueryable<Timesheet> timesheets, string? payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? timesheets.Where(t => t.EE != null && t.EE.PayrollCode == payrollCode)
                : timesheets;
        }

        public static IEnumerable<Timesheet> FilterByBank(this IEnumerable<Timesheet> timesheets, TimesheetBankChoices bank)
        {
            return timesheets.Where(ts => ts.EE != null && ts.EE.Bank == bank);
        }

        public static IEnumerable<Timesheet> Exportable(this IEnumerable<Timesheet> timesheets)
        {
            return timesheets.Where(ts => ts.IsConfirmed && ts.TotalHours > 0).OrderBy(ts => ts.EE?.FullName);
        }

        public static IEnumerable<Timesheet> UnconfirmedWithoutAttendance(this IEnumerable<Timesheet> timesheets)
        {
            return timesheets.Where(ts => !ts.IsConfirmed && ts.TotalHours == 0).OrderBy(ts => ts.EE?.FullName);
        }

        public static IEnumerable<Timesheet> UnconfirmedWithAttendance(this IEnumerable<Timesheet> timesheets)
        {
            return timesheets.Where(ts => !ts.IsConfirmed && ts.TotalHours > 0).OrderBy(ts => ts.EE?.FullName);
        }

        public static List<int> GroupByPage(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .GroupBy(ts => ts.Page, ts => ts.Page)
                .Select((page, i) => page.First())
                .ToList();


        public static IEnumerable<Timesheet[]> TimesheetsByEEId(this IEnumerable<Timesheet> timesheets)
        {
            return timesheets.GroupBy(t => t.EEId).Select(t => t.ToArray());
        }

        public static List<string> ExtractCutoffIds(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(ts => ts.EE is not null)
                .GroupBy(ts => ts.CutoffId)
                .Select(ts => ts.First())
                .OrderByDescending(ts => ts.CutoffId)
                .Select(ts => ts.CutoffId)
                .ToList();

        public static IEnumerable<string> CutoffIds(this IEnumerable<Timesheet> timesheets)
        {
            return timesheets
                .GroupBy(t => t.CutoffId)
                .Select(t => t.First())
                .OrderByDescending(t => t.CutoffId)
                .Select(t => t.CutoffId);
        }


        //public static List<TimesheetBankChoices> ExtractBanks(this IEnumerable<Timesheet> timesheets) =>
        //    timesheets
        //        .Where(ts => ts.EE is not null)
        //        .GroupBy(ts => ts.EE.Bank)
        //        .Select(ts => ts.First())
        //        .OrderBy(ts => ts.EE.Bank)
        //        .Select(ts => ts.EE.Bank).ToList();

        public static IEnumerable<TimesheetBankChoices> ExtractBanks(this IEnumerable<Timesheet> timesheets)
        {
            return timesheets
                .Where(t => t.EE != null)
                .GroupBy(t => t.EE?.Bank)
                .Select(t => t.First().EE?.Bank ?? default)
                .OrderBy(t => t);
        }


        public static IEnumerable<string> ExtractPayrollCodes(this IEnumerable<Timesheet> timesheets) =>
            timesheets
                .Where(t => !string.IsNullOrEmpty(t.EE?.PayrollCode))
                .GroupBy(t => t.EE?.PayrollCode)
                .Select(t => t.First().EE?.PayrollCode ?? string.Empty)
                .OrderBy(t => t);
    }
}
