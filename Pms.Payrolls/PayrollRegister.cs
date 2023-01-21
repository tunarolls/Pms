using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Domain.SupportTypes
{
    public class PayrollRegister
    {
        public string Name { get; set; }

        public double EmployeeSSS { get; set; }
        public double EmployerSSS { get; set; }

        public double EmployeePhilHealth { get; set; }
        public double EmployerPhilHealth { get; set; }

        public double EmployeePagibig { get; set; }
        public double EmployerPagibig { get; set; }

        public double WithholdingTax { get; set; }

        public PayrollRegister(string name) { Name = name; }
        public PayrollRegister(string name, IEnumerable<Payroll> payrolls)
        {
            Name = name;

            EmployeeSSS = payrolls.Sum(p => p.EmployeeSSS);
            EmployerSSS = payrolls.Sum(p => p.EmployerSSS);

            EmployeePhilHealth = payrolls.Sum(p => p.EmployeePhilHealth);
            EmployerPhilHealth = payrolls.Sum(p => p.EmployerPhilHealth);

            EmployeePagibig = payrolls.Sum(p => p.EmployeePagibig);
            EmployerPagibig = payrolls.Sum(p => p.EmployerPagibig);

            WithholdingTax = payrolls.Sum(p => p.WithholdingTax);
        }

        public void Merge(PayrollRegister payrollRegister)
        {
            EmployeeSSS += payrollRegister.EmployeeSSS;
            EmployerSSS += payrollRegister.EmployerSSS;

            EmployeePhilHealth += payrollRegister.EmployeePhilHealth;
            EmployerPhilHealth += payrollRegister.EmployerPhilHealth;

            EmployeePagibig += payrollRegister.EmployeePagibig;
            EmployerPagibig += payrollRegister.EmployerPagibig;

            WithholdingTax += payrollRegister.WithholdingTax;
        }
    }
}
