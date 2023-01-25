using Pms.Timesheets;
using Pms.Timesheets.ServiceLayer.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.Module.Models
{
    public class Timesheets
    {
        private readonly TimesheetProvider t_Provider;
        private readonly TimesheetManager t_Manager;

        public Timesheets(TimesheetProvider provider, TimesheetManager manager)
        {
            t_Provider = provider;
            t_Manager = manager;
        }

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId)
        {
            return t_Provider.GetTimesheets(cutoffId);
        }

        public async Task<ICollection<Timesheet>> GetTimesheets(string cutoffId, CancellationToken cancellationToken = default)
        {
            return await t_Provider.GetTimesheets(cutoffId, cancellationToken);
        }

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId, string payrollCodeId)
        {
            return t_Provider.GetTimesheets(cutoffId, payrollCodeId);
        }

        public void SaveTimesheet(Timesheet timesheet)
        {
            t_Manager.SaveTimesheet(timesheet, timesheet.CutoffId, 0);
        }

        public async Task SaveTimesheet(Timesheet timesheet, CancellationToken cancellationToken = default)
        {
            await t_Manager.SaveTimesheet(timesheet, timesheet.CutoffId, 0, cancellationToken);
        }
    }
}
