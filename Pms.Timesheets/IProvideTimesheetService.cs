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

        IEnumerable<Timesheet> GetTimesheets();

        IEnumerable<Timesheet> GetTimesheets(string cutoffId);

        Task<ICollection<Timesheet>> GetTimesheets(string cutoffId, CancellationToken cancellationToken = default);

        IEnumerable<Timesheet> GetTwoPeriodTimesheets(string cutoffId);

        IEnumerable<Timesheet> GetTimesheetsByMonth(int month);

        IEnumerable<Timesheet> GetTimesheetNoEETimesheet(string cutoffId);

        public int GetLastPage(string cutoffId, string payrollCode);

        public List<int> GetPageWithUnconfirmedTS(string cutoffId, string payrollCode);

        public List<int> GetPages(string cutoffId, string payrollCode);

        public List<int> GetMissingPages(string cutoffId, string payrollCode);
    }
}
