using Pms.Common;
using Pms.Masterlists.ServiceLayer.EfCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module
{
    public class PayrollCodes
    {
        private readonly PayrollCodeManager _payrollCodeManager;

        public PayrollCodes(PayrollCodeManager payrollCodeManager)
        {
            _payrollCodeManager = payrollCodeManager;
        }

        public ICollection<PayrollCode> ListPayrollCodes()
        {
            return _payrollCodeManager.GetPayrollCodes().ToList();
        }

        public async Task<ICollection<PayrollCode>> ListPayrollCode(CancellationToken cancellationToken = default)
        {
            return await _payrollCodeManager.GetPayrollCodes(cancellationToken);
        }

        public void Save(PayrollCode payrollCode)
        {
            _payrollCodeManager.SavePayrollCode(payrollCode);
        }

        public async Task Save(PayrollCode payrollCode, CancellationToken cancellationToken = default)
        {
            await _payrollCodeManager.SavePayrollCode(payrollCode, cancellationToken);
        }
    }
}
