using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System.Collections.Generic;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments
{
    public class RegularSheetWriter : ISheetWriter
    {
        readonly IEnumerable<Payroll> _payrolls;

        public RegularSheetWriter(IEnumerable<Payroll> payrolls, IRowWriter rowWriter)
        {
            RowWriter = rowWriter;
            _payrolls = payrolls.OrderBy(p => p.EE?.FullName);
        }

        public RegularSheetWriter(IEnumerable<Payroll> payrolls)
        {
            _payrolls = payrolls.OrderBy(p => p.EE?.FullName);
        }

        public IRowWriter? RowWriter { get; set; }

        public void Write(ISheet sheet, int startIndex = 1)
        {
            int index = startIndex;
            int sequence = 0;

            foreach (var payroll in _payrolls)
            {
                RowWriter?.Write(sheet.GetOrCreateRow(Append(ref index)), payroll, Append(ref sequence));
            }

            var payrollRegister = new PayrollRegister("GRAND", _payrolls);
            RowWriter?.WriteTotal(sheet.GetOrCreateRow(Append(ref index)), payrollRegister);
        }

        //public static void Write(IEnumerable<Payroll> payrolls, IRowWriter rowWriter, ISheet sheet, int startIndex = 1)
        //{
        //    int index = startIndex;
        //    int sequence = 0;

        //    foreach (var payroll in payrolls)
        //    {
        //        rowWriter.Write(sheet.GetOrCreateRow(Append(ref index)), payroll, Append(ref sequence));
        //    }

        //    var payrollRegister = new PayrollRegister("GRAND", payrolls);
        //    rowWriter.WriteTotal(sheet.GetOrCreateRow(Append(ref index)), payrollRegister);
        //}

        private int Append(ref int index)
        {
            index++;
            return index;
        }
    }
}
