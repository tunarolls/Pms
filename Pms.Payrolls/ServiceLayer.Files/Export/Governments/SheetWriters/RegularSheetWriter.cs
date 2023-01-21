using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System.Collections.Generic;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments
{
    public class RegularSheetWriter : ISheetWriter
    {
        public IRowWriter RowWriter;
        readonly IEnumerable<Payroll> Payrolls;

        public RegularSheetWriter(IEnumerable<Payroll> payrolls, IRowWriter rowWriter)
        {
            RowWriter = rowWriter;
            Payrolls = payrolls.OrderBy(p => p.EE.Fullname);
        }

        public RegularSheetWriter(IEnumerable<Payroll> payrolls) =>
            Payrolls = payrolls.OrderBy(p => p.EE.Fullname);



        public void Write(ISheet sheet, int startIndex = 1)
        {
            int index = startIndex;
            int sequence = 0;
            foreach (Payroll payroll in Payrolls)
                RowWriter.Write(sheet.GetOrCreateRow(append(ref index)), payroll, append(ref sequence));

            PayrollRegister payrollRegister = new("GRAND", Payrolls);
            RowWriter.WriteTotal(sheet.GetOrCreateRow(append(ref index)), payrollRegister);
        }


        private static int append(ref int index)
        {
            index++;
            return index;
        }
    }
}
