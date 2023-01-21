using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Payrolls.Services
{
    public interface IManagePayrollService
    {
        void SavePayroll(Payroll payroll);
        Task SavePayroll(Payroll payroll, CancellationToken cancellationToken = default);
    }
}
