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
        Task<EmployeeView?> FindEmployeeView(string eeId, CancellationToken cancellationToken = default);
        public int GetLastPage(string cutoffId, string payrollCode);

        public List<int> GetMissingPages(string cutoffId, string payrollCode);

        public Task<int[]> GetMissingPages(string cutoffId, string payrollCode, CancellationToken cancellationToken = default);

        public List<int> GetPages(string cutoffId, string payrollCode);

        public List<int> GetPageWithUnconfirmedTS(string cutoffId, string payrollCode);

        IEnumerable<Timesheet> GetTimesheetNoEETimesheet(string cutoffId);

        IEnumerable<Timesheet> GetTimesheets();
        Task<ICollection<Timesheet>> GetTimesheets(CancellationToken cancellationToken = default);

        IEnumerable<Timesheet> GetTimesheets(string cutoffId);

        Task<ICollection<Timesheet>> GetTimesheets(string cutoffId, CancellationToken cancellationToken = default);
        Task<ICollection<Timesheet>> GetTimesheets(string cutoffId, string payrollCode, CancellationToken cancellationToken = default);
        IEnumerable<Timesheet> GetTimesheetsByMonth(int month);

        IEnumerable<Timesheet> GetTwoPeriodTimesheets(string cutoffId);

        Task<ICollection<Timesheet>> GetTwoPeriodTimesheets(string cutoffId, CancellationToken cancellationToken = default);
        Task<ICollection<Timesheet>> GetTwoPeriodTimesheets(string cutoffId, string payrollCode, CancellationToken cancellationToken = default);
    }
}
