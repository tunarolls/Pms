using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
{
    public class MonthlyPayroll : Payroll
    {

        public MonthlyPayroll(Payroll[] monthlyPayroll)
        {
            Payroll payroll = monthlyPayroll.First();

            EE = payroll.EE;

            EEId = payroll.EEId;
            PayrollCode = payroll.PayrollCode;
            CompanyId = payroll.CompanyId;


            //GrossPay = monthlyPayroll.Sum(p => p.GrossPay); // NOT NEEDED
            RegularPay = monthlyPayroll.Sum(p => p.RegularPay);
            NetPay = monthlyPayroll.Sum(p => p.NetPay);

            ComputePagibig();
            ComputePhilHealth();
            ComputeSSS();
            ComputeWTAX();

        }


        public void ComputePagibig()
        {
            EmployeePagibig = RegularPay * 0.02d;
            if (EmployeePagibig >= 21d & EmployeePagibig < 100d)
                EmployerPagibig = EmployeePagibig;
            else if (EmployeePagibig < 21d)
                EmployerPagibig = EmployeePagibig * 2d;
            else
                EmployerPagibig = 100;
        }

        public void ComputeSSS()
        {
            int multiplier = (int)((long)Math.Round(RegularPay - 2750d) / 500L);

            double ER_rsc = Math.Min(255d + 42.5d * multiplier, 1700d);
            double EE_rsc = Math.Min(135d + 22.5d * multiplier, 900d);

            double ER_ec = multiplier <= 23 ? 10 : 30;
            double EE_ec = 0d;

            int multiplier_mpc = Math.Max(0, multiplier - 34);
            double ER_mpf = Math.Min(42.5d * multiplier_mpc, 425d);
            double EE_mpf = Math.Min(22.5d * multiplier_mpc, 225d);

            EmployeeSSS = EE_rsc + EE_ec + EE_mpf;
            EmployerSSS = ER_rsc + ER_ec + ER_mpf;
        }

        public void ComputePhilHealth()
        {
            switch (RegularPay)
            {
                case var @case when @case >= 79999.99:
                    EmployeePhilHealth = 1600;
                    break;
                case var case1 when case1 >= 10000.01d:
                    EmployeePhilHealth = RegularPay * 0.04d / 2;
                    break;
                case var case2 when case2 <= 10000d:
                    EmployeePhilHealth = 200;
                    break;
            }

            EmployerPhilHealth = EmployeePhilHealth;
        }

        public void ComputeWTAX()
        {
            double wtax = 0d;
            switch (RegularPay)
            {
                case var case3 when case3 >= 666667d:
                    wtax = 200833.33d + ((RegularPay - 666667d) * 0.35d);
                    break;
                case var case4 when case4 >= 166667d:
                    wtax = 40833.33d + ((RegularPay - 166667d) * 0.32d);
                    break;
                case var case5 when case5 >= 66667d:
                    wtax = 10833.33d + ((RegularPay - 66667d) * 0.3d);
                    break;
                case var case6 when case6 >= 33333d:
                    wtax = 2500d + ((RegularPay - 33333d) * 0.25d);
                    break;
                case var case7 when case7 >= 20833.01d:
                    wtax = 0d + ((RegularPay - 20833.01d) * 0.2d);
                    break;
                case var case8 when case8 <= 20833d:
                    wtax = 0d;
                    break;
            }

            WithholdingTax = wtax;
        }

    }
}
