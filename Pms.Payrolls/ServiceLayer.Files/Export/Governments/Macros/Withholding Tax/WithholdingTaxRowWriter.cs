﻿using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class WithholdingTaxRowWriter : IRowWriter
    {
        public void Write(IRow row, Payroll payroll, int sequence)
        {
            row.GetOrCreateCell(0).SetCellValue(sequence);
            row.GetOrCreateCell(1).SetCellValue(payroll.EEId);
            row.GetOrCreateCell(2).SetCellValue(payroll.EE.FullName);
            row.GetOrCreateCell(3).SetCellValue(payroll.WithholdingTax);
        }

        public void WriteTotal(IRow row, PayrollRegister payrollRegister)
        {

            row.GetOrCreateCell(2).SetCellValue($"{payrollRegister.Name} TOTAL");

            row.GetOrCreateCell(3).SetCellValue(payrollRegister.WithholdingTax);
           
        }
    }
}
