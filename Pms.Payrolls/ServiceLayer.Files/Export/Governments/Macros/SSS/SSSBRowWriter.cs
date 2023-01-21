using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System.Collections.Generic;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class SSSBRowWriter : IRowWriter
    {
        public void Write(IRow row, Payroll payroll, int sequence)
        {
            row.GetOrCreateCell(0).SetCellValue(payroll.EE.SSS);
            row.GetOrCreateCell(1).SetCellValue(payroll.EE.LastName);
            row.GetOrCreateCell(2).SetCellValue(payroll.EE.FirstName);
            row.GetOrCreateCell(3).SetCellValue(payroll.EE.MiddleInitial);
            row.GetOrCreateCell(4).SetCellValue(payroll.EmployerSSS+ payroll.EmployeeSSS);// should be employer
        }

        /// <summary>
        /// Will not be used.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="payrollRegister"></param>
        public void WriteTotal(IRow row, PayrollRegister payrollRegister)  {  }
    }
}
