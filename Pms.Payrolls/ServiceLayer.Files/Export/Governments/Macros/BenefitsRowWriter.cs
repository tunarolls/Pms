using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class BenefitsRowWriter : IRowWriter
    {
        public void Write(IRow row, Payroll payroll, int sequence)
        {
            row.GetOrCreateCell(0).SetCellValue(sequence);
            row.GetOrCreateCell(1).SetCellValue(payroll.EEId);
            row.GetOrCreateCell(2).SetCellValue(payroll.EE.Fullname);

            row.GetOrCreateCell(3).SetCellValue(payroll.EmployeePhilHealth);
            row.GetOrCreateCell(4).SetCellValue(payroll.EmployeePhilHealth);// should be employer
            row.GetOrCreateCell(5).SetCellValue(payroll.EmployeePhilHealth + payroll.EmployeePhilHealth);// should be employer

            row.GetOrCreateCell(6).SetCellValue(payroll.EmployeeSSS);
            row.GetOrCreateCell(7).SetCellValue(payroll.EmployeeSSS);// should be employer
            row.GetOrCreateCell(8).SetCellValue(payroll.EmployeeSSS + payroll.EmployeeSSS);// should be employer

            row.GetOrCreateCell(9).SetCellValue(payroll.EmployeePagibig);
            row.GetOrCreateCell(10).SetCellValue(payroll.EmployeePagibig);// should be employer
            row.GetOrCreateCell(11).SetCellValue(payroll.EmployeePagibig + payroll.EmployeePagibig);// should be employer
        }

        public void WriteTotal(IRow row, PayrollRegister payrollRegister)
        {
            row.GetOrCreateCell(2).SetCellValue($"{payrollRegister.Name} TOTAL");
            row.GetOrCreateCell(3).SetCellValue(payrollRegister.EmployeePhilHealth);
            row.GetOrCreateCell(4).SetCellValue(payrollRegister.EmployeePhilHealth);// should be employer
            row.GetOrCreateCell(5).SetCellValue(payrollRegister.EmployeePhilHealth + payrollRegister.EmployeePhilHealth);// should be employer

            row.GetOrCreateCell(6).SetCellValue(payrollRegister.EmployeeSSS);
            row.GetOrCreateCell(7).SetCellValue(payrollRegister.EmployeeSSS);// should be employer
            row.GetOrCreateCell(8).SetCellValue(payrollRegister.EmployeeSSS + payrollRegister.EmployeeSSS);// should be employer

            row.GetOrCreateCell(9).SetCellValue(payrollRegister.EmployeePagibig);
            row.GetOrCreateCell(10).SetCellValue(payrollRegister.EmployeePagibig);// should be employer
            row.GetOrCreateCell(11).SetCellValue(payrollRegister.EmployeePagibig + payrollRegister.EmployeePagibig);// should be employer
             
        }
    }
}
