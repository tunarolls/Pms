using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public interface IProvideTimesheetService
    {
        EmployeeView FindEmployeeView(string eeId);

        public int GetLastPage(string cutoffId, string payrollCode);

        public List<int> GetMissingPages(string cutoffId, string payrollCode);

        public List<int> GetPages(string cutoffId, string payrollCode);

        public List<int> GetPageWithUnconfirmedTS(string cutoffId, string payrollCode);

        IEnumerable<Timesheet> GetTimesheetNoEETimesheet(string cutoffId);

        IEnumerable<Timesheet> GetTimesheets();

        Task<IEnumerable<Timesheet>> GetTimesheets(string cutoffId, CancellationToken cancellationToken = default);

        IEnumerable<Timesheet> GetTimesheets(string cutoffId, string payrollCodeId);

        IEnumerable<Timesheet> GetTimesheetsByMonth(int month);

        IEnumerable<Timesheet> GetTwoPeriodTimesheets(string cutoffId);
    }
}
