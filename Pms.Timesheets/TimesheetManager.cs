using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
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
            if (timesheet == null) throw new ArgumentNullException(nameof(timesheet));
            using TimesheetDbContext context = _factory.CreateDbContext();
            Timesheet? timesheetFound = context.Timesheets.Where(ts => ts.TimesheetId == timesheet.TimesheetId).FirstOrDefault();
            if (timesheetFound == null) context.Add(timesheet);
            else context.Entry(timesheetFound).CurrentValues.SetValues(timesheet);

            if (save) context.SaveChanges();
        }

        public EmployeeView? FindEmployee(string eeId)
        {
            using TimesheetDbContext context = _factory.CreateDbContext();
            return context.Employees.Find(eeId);
        }

        public void SaveTimesheet(Timesheet timesheet, string cutoffId, int page)
        {
            if (timesheet == null) throw new ArgumentNullException(nameof(timesheet));
            timesheet.CutoffId = cutoffId;
            timesheet.Page = page;
            timesheet.TimesheetId = $"{timesheet.EEId}_{timesheet.CutoffId}";

            timesheet.RawPCV = ToRawPCV(timesheet.PCV);

            timesheet.SetEmployeeDetail(FindEmployee(timesheet.EEId));

            CreateOrUpdate(timesheet, true);
        }

        public void SaveTimesheetEmployeeData(Timesheet timesheet)
        {
            if (timesheet.EE is not null)
            {
                timesheet.SetEmployeeDetail(timesheet.EE);
                CreateOrUpdate(timesheet, true);
            }
        }

        private static string ToRawPCV(string[,]? pcv)
        {
            if (pcv != null)
            {
                string[] to1D = new string[pcv.Length / 2];

                for (int i = 0; i < to1D.Length; i++)
                    to1D[i] = $"{pcv[i, 0]}~{pcv[i, 1]}";
                return string.Join('|', to1D);
            }

            return string.Empty;
        }
    }
}
