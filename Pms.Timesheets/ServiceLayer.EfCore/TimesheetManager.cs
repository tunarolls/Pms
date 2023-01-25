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
    public class TimesheetManager
    {
        private IDbContextFactory<TimesheetDbContext> _factory;
        public TimesheetManager(IDbContextFactory<TimesheetDbContext> factory)
        {
            _factory = factory;
        }

        public void CreateOrUpdate(Timesheet timesheet, bool save = true)
        {
            using (TimesheetDbContext Context = _factory.CreateDbContext())
            {
                var timesheetFound = Context.Timesheets.Where(ts => ts.TimesheetId == timesheet.TimesheetId).FirstOrDefault();
                if (timesheetFound is null)
                    Context.Add(timesheet);
                else
                    Context.Entry(timesheetFound).CurrentValues.SetValues(timesheet);

                if (save) Context.SaveChanges();
            }
        }

        public async Task CreateOrUpdate(Timesheet timesheet, bool save = true, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var timesheetFound = await context.Timesheets.SingleOrDefaultAsync(t => t.TimesheetId == timesheet.TimesheetId, cancellationToken);
            if (timesheetFound != null)
            {
                context.Entry(timesheetFound).CurrentValues.SetValues(timesheet);
            }
            else
            {
                await context.AddAsync(timesheet, cancellationToken);
            }

            if (save) await context.SaveChangesAsync(cancellationToken);
        }

        public EmployeeView? FindEmployee(string eeId)
        {
            using var context = _factory.CreateDbContext();
            return context.Employees.Find(eeId);
        }

        public async Task<EmployeeView?> FindEmployee(string eeId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Employees.FindAsync(new object[] { eeId }, cancellationToken);
        }

        public void SaveTimesheet(Timesheet timesheet, string cutoffId, int page)
        {
            timesheet.CutoffId = cutoffId;
            timesheet.Page = page;
            timesheet.TimesheetId = $"{timesheet.EEId}_{timesheet.CutoffId}";

            timesheet.RawPCV = ToRawPCV(timesheet.PCV);

            CreateOrUpdate(timesheet, true);
        }

        public async Task SaveTimesheet(Timesheet timesheet, string cutoffId, int page, CancellationToken cancellationToken = default)
        {
            timesheet.CutoffId = cutoffId;
            timesheet.Page = page;
            timesheet.TimesheetId = $"{timesheet.EEId}_{timesheet.CutoffId}";
            timesheet.RawPCV = ToRawPCV(timesheet.PCV);
            await CreateOrUpdate(timesheet, true, cancellationToken);
        }

        private static string ToRawPCV(string[,] pcv)
        {
            string[] to1D = new string[pcv.Length / 2];

            for (int i = 0; i < to1D.Length; i++)
            {
                to1D[i] = $"{pcv[i, 0]}~{pcv[i, 1]}";
            }

            return string.Join('|', to1D);
        }
    }
}
