using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Tests
{
    static class Seeder
    {
        public static Payroll GenerateSeedPayroll(string eeId, string cutoffId, string companyId, double grossPay, double regPay, double netPay, double adjust1Total, double adjust2Total, double governmentTotal)
        {
            Payroll payroll = new()
            {
                EEId = eeId,
                CutoffId = cutoffId,
                PayrollCode = "P1A",
                CompanyId= companyId,
                GrossPay = grossPay,
                RegularPay = regPay,
                NetPay = netPay,
                Adjust1Total = adjust1Total,
                Adjust2Total = adjust2Total,
                GovernmentTotal = governmentTotal,
            };
            payroll.PayrollId = Payroll.GenerateId(payroll);
            return payroll;
        }
    }
}
