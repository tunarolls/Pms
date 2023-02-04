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
        private readonly Dictionary<string, Dictionary<string, List<Payroll>>> PayrollsByPayrollAndJobCode;

        public SpecialSheetWriter(IEnumerable<Payroll> payrolls, IRowWriter rowWriter)
        {
            PayrollsByPayrollAndJobCode = new();
            RowWriter = rowWriter;

            InitPayrolls(payrolls);
        }

        public SpecialSheetWriter(IEnumerable<Payroll> payrolls )
        {
            PayrollsByPayrollAndJobCode = new();

            InitPayrolls(payrolls);
        }

        public IRowWriter? RowWriter { get; set; }


        private void InitPayrolls(IEnumerable<Payroll> payrolls)
        {
            List<List<Payroll>> payrollCodePayrolls = payrolls
                .GroupBy(p => p.EE?.PayrollCode ?? "")
                .Select(pp => pp.ToList())
                .ToList();

            foreach (List<Payroll> payrollCodePayroll in payrollCodePayrolls)
            {
                var payrollCode = payrollCodePayroll.First().EE?.PayrollCode ?? "";
                Dictionary<string, List<Payroll>> payrollsByJobCode = payrollCodePayroll
                    .GroupBy(p => p.EE?.JobCode ?? "")
                    .Select(pp => pp.OrderBy(p => p.EE?.FullName).ToList())
                    .OrderBy(p => p.First().EE?.JobCode)
                    .ToDictionary(pp => pp.First().EE?.JobCode ?? "");

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
                sheet.GetOrCreateRow(Append(ref index)).GetOrCreateCell(0).SetCellValue(payrollCode);
                foreach (string jobCode in PayrollsByPayrollAndJobCode[payrollCode].Keys)
                {
                    Append(ref index);
                    sheet.GetOrCreateRow(Append(ref index)).GetOrCreateCell(0).SetCellValue($"*** Status Code = {jobCode}");
                    Append(ref index);

                    foreach (Payroll payroll in PayrollsByPayrollAndJobCode[payrollCode][jobCode])
                    {
                        RowWriter?.Write(sheet.GetOrCreateRow(Append(ref index)), payroll, Append(ref sequence));
                    }

                    Append(ref index);
                    Append(ref index);

                    PayrollRegister payrollRegisterByJobCode = new(jobCode, PayrollsByPayrollAndJobCode[payrollCode][jobCode]);
                    RowWriter?.WriteTotal(sheet.GetOrCreateRow(Append(ref index)), payrollRegisterByJobCode);
                    payrollRegisterByPayrollCode.Merge(payrollRegisterByJobCode);

                    grandTotaRecords++;
                }

                Append(ref index);
                RowWriter?.WriteTotal(sheet.GetOrCreateRow(Append(ref index)), payrollRegisterByPayrollCode);

                grandPayrollRegister.Merge(payrollRegisterByPayrollCode);
                grandTotal += sequence;
            }

            Append(ref index);
            RowWriter?.WriteTotal(sheet.GetOrCreateRow(Append(ref index)), grandPayrollRegister);
        }

        private static int Append(ref int index)
        {
            index++;
            return index;
        }
    }
}
