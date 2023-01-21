using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System.Collections.Generic;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class PagibigRowWriter : IRowWriter
    {
        public void Write(IRow row, Payroll payroll, int sequence)
        {
            row.GetOrCreateCell(0).SetCellValue(sequence);
            row.GetOrCreateCell(1).SetCellValue(payroll.EEId);
            row.GetOrCreateCell(2).SetCellValue(payroll.EE.Fullname);
            row.GetOrCreateCell(3).SetCellValue(payroll.EmployeePagibig);
            row.GetOrCreateCell(4).SetCellValue(payroll.EmployerPagibig);// should be employer
            row.GetOrCreateCell(5).SetCellValue(payroll.EmployerPagibig + payroll.EmployeePagibig);// should be employer
        }

        public void WriteTotal(IRow row, PayrollRegister payrollRegister)
        {
            
            row.GetOrCreateCell(2).SetCellValue($"{payrollRegister.Name} TOTAL"); 
            row.GetOrCreateCell(3).SetCellValue(payrollRegister.EmployeePagibig);
            row.GetOrCreateCell(4).SetCellValue(payrollRegister.EmployerPagibig);// should be employer
            row.GetOrCreateCell(5).SetCellValue(payrollRegister.EmployerPagibig + payrollRegister.EmployeePagibig);// should be employer
             
        }
    }
}
