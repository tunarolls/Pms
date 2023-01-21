using Pms.Common;
using Pms.Masterlists.ServiceLayer.EfCore;
using System.Collections.Generic;
using System.Linq;

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

        public void Save(PayrollCode payrollCode)
        {
            _payrollCodeManager.SavePayrollCode(payrollCode);
        }
    }
}
