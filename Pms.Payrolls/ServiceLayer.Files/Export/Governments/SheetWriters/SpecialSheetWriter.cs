using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments
{
    public class SpecialSheetWriter : ISheetWriter
    {
        public IRowWriter RowWriter;
        private readonly Dictionary<string, Dictionary<string, List<Payroll>>> PayrollsByPayrollAndJobCode;

        public SpecialSheetWriter(IEnumerable<Payroll> payrolls, IRowWriter rowWriter)
        {
            PayrollsByPayrollAndJobCode = new();
            RowWriter = rowWriter;

            initPayrolls(payrolls);
        }


        public SpecialSheetWriter(IEnumerable<Payroll> payrolls )
        {
            PayrollsByPayrollAndJobCode = new();
            initPayrolls(payrolls);
        }


        private void initPayrolls(IEnumerable<Payroll> payrolls)
        {
            List<List<Payroll>> payrollCodePayrolls = payrolls
                .GroupBy(p => p.EE.PayrollCode)
                .Select(pp => pp.ToList())
                .ToList();

            foreach (List<Payroll> payrollCodePayroll in payrollCodePayrolls)
            {
                string payrollCode = payrollCodePayroll.First().EE.PayrollCode;
                Dictionary<string, List<Payroll>> payrollsByJobCode = payrollCodePayroll
                    .GroupBy(p => p.EE.JobCode)
                    .Select(pp =>
                        pp
                        .OrderBy(p => p.EE.Fullname)
                        .ToList()
                    )
                    .OrderBy(p => p.First().EE.JobCode)
                    .ToDictionary(pp => pp.First().EE.JobCode);

                PayrollsByPayrollAndJobCode.Add(payrollCode, payrollsByJobCode);
            }
        }











        public void Write(ISheet sheet, int startIndex = 0)
        {
            int index = startIndex;

            PayrollRegister grandPayrollRegister = new("GRAND");

            double grandTotal = 0;
            int grandTotaRecords = 0;
            foreach (string payrollCode in PayrollsByPayrollAndJobCode.Keys)
            {
                PayrollRegister payrollRegisterByPayrollCode = new(payrollCode);

                int sequence = 0;
                sheet.GetOrCreateRow(append(ref index)).GetOrCreateCell(0).SetCellValue(payrollCode);
                foreach (string jobCode in PayrollsByPayrollAndJobCode[payrollCode].Keys)
                {
                    append(ref index);
                    sheet.GetOrCreateRow(append(ref index)).GetOrCreateCell(0).SetCellValue($"*** Status Code = {jobCode}");
                    append(ref index);

                    foreach (Payroll payroll in PayrollsByPayrollAndJobCode[payrollCode][jobCode])
                        RowWriter.Write(sheet.GetOrCreateRow(append(ref index)), payroll, append(ref sequence));

                    append(ref index);
                    append(ref index);

                    PayrollRegister payrollRegisterByJobCode = new(jobCode, PayrollsByPayrollAndJobCode[payrollCode][jobCode]);
                    RowWriter.WriteTotal(sheet.GetOrCreateRow(append(ref index)), payrollRegisterByJobCode);
                    payrollRegisterByPayrollCode.Merge(payrollRegisterByJobCode);

                    grandTotaRecords++;
                }

                append(ref index);
                RowWriter.WriteTotal(sheet.GetOrCreateRow(append(ref index)), payrollRegisterByPayrollCode);

                grandPayrollRegister.Merge(payrollRegisterByPayrollCode);
                grandTotal += sequence;
            }

            append(ref index);
            RowWriter.WriteTotal(sheet.GetOrCreateRow(append(ref index)), grandPayrollRegister);
        }


        private static int append(ref int index)
        {
            index++;
            return index;
        }
    }
}
