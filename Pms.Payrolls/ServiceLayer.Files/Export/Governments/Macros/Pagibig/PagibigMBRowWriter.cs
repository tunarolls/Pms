using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System.Collections.Generic;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class PagibigMBRowWriter : IRowWriter
    {
        public void Write(IRow row, Payroll payroll, int sequence)
        {
            row.GetOrCreateCell(0).SetCellValue(payroll.EE.Pagibig);
            row.GetOrCreateCell(1).SetCellValue(payroll.EE.TIN);
            row.GetOrCreateCell(2).SetCellValue(payroll.EE.LastName);
            row.GetOrCreateCell(3).SetCellValue(payroll.EE.FirstName);
            row.GetOrCreateCell(4).SetCellValue(payroll.EE.MiddleName);
            row.GetOrCreateCell(5).SetCellValue(payroll.EE.BirthDate);
            row.GetOrCreateCell(6).SetCellValue(payroll.EmployeePagibig);
            row.GetOrCreateCell(7).SetCellValue(payroll.EmployerPagibig);// should be employer
        }

        /// <summary>
        /// Will not be used.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="payrollRegister"></param>
        public void WriteTotal(IRow row, PayrollRegister payrollRegister) { }
    }
}
