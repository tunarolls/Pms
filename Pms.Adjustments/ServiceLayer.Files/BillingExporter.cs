using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.ServiceLayer.Files
{
    public class BillingExporter
    {
        public static int ExportBillings(IEnumerable<Billing> billings, string cutoffId, string payrollCodeId, AdjustmentTypes adjustmentName)
        {
            if (billings?.Any() ?? false)
            {
                IWorkbook nWorkBook = new HSSFWorkbook();
                ISheet nSheet = nWorkBook.CreateSheet(adjustmentName.ToString());

                int ridx = 0;
                IRow nRow = nSheet.CreateRow(ridx);

                nRow.CreateCell(0).SetCellValue("EE ID");
                nRow.CreateCell(1).SetCellValue("FULLNAME");
                nRow.CreateCell(2).SetCellValue("ADJUSTMENT NAME");
                nRow.CreateCell(3).SetCellValue("AMOUNT");
                nRow.CreateCell(4).SetCellValue("REMARKS");

                ridx++;

                foreach (Billing billing in billings)
                {
                    nRow = nSheet.CreateRow(ridx);
                    nRow.CreateCell(0).SetCellValue(billing.EEId);
                    if (billing.EE is not null) nRow.CreateCell(1).SetCellValue(billing.EE.FullName);

                    nRow.CreateCell(2).SetCellValue(billing.AdjustmentType.ToString());
                    nRow.CreateCell(3).SetCellValue(billing.Amount);
                    nRow.CreateCell(4).SetCellValue(billing.Remarks);

                    ridx++;
                }

                string fileDir = $@"{AppDomain.CurrentDomain.BaseDirectory}EXPORT\{cutoffId}\{payrollCodeId}\BILLING";
                Directory.CreateDirectory(fileDir);
                using (FileStream nNewPayreg = new FileStream($@"{fileDir}\{cutoffId}_{payrollCodeId}_{adjustmentName}.xls", FileMode.Create, FileAccess.Write))
                    nWorkBook.Write(nNewPayreg, false);

                return ridx - 1;
            }

            return 0;
        }

    }
}
