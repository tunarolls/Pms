using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pms.Adjustments.ServiceLayer.Files
{
    public class BillingRecordImporter
    {

        public IEnumerable<BillingRecord> Import(string filePath)
        {
            List<BillingRecord> records = new();

            IWorkbook nWorkBook;
            using (FileStream nNewPayreg = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                nWorkBook = new HSSFWorkbook(nNewPayreg);

            var nSheet = nWorkBook.GetSheetAt(0);
            var nRow = nSheet.GetRow(0);
            int rIdx = 1;
            while (nRow is not null || nRow.GetCell(3) is not null)
            {
                try
                {
                    nRow = nSheet.GetRow(rIdx);
                    if (nRow is null || nRow.GetCell(3) is null)
                        break;

                    BillingRecord newRecord = new();
                    newRecord.AdjustmentOption = AdjustmentOptions.ADJUST2;

                    DeductionOptions deductionOption;
                    if (!Enum.TryParse(nRow.GetCell(0).StringCellValue, out deductionOption))
                    {
                        rIdx += 1;
                        continue;
                    }
                    else
                        newRecord.DeductionOption = deductionOption;

                    AdjustmentTypes adjustmentType;
                    if (!Enum.TryParse(nRow.GetCell(1).StringCellValue, out adjustmentType))
                    {
                        rIdx += 1;
                        continue;
                    }
                    else
                        newRecord.AdjustmentType = adjustmentType;



                    newRecord.EEId = nRow.GetCell(2).GetValue();
                    newRecord.Advances = double.Parse(nRow.GetCell(4).GetValue());
                    newRecord.EffectivityDate = DateTime.Parse(nRow.GetCell(5).GetValue());
                    newRecord.Amortization = double.Parse(nRow.GetCell(6).GetValue());
                    newRecord.Balance = double.Parse(nRow.GetCell(7).GetValue());

                    newRecord.RecordId = BillingRecord.GenerateId(newRecord);
                    records.Add(newRecord);
                    rIdx += 1;

                    nRow.CreateCell(8).SetCellValue("OKAY");
                    nRow.CreateCell(10).SetCellValue("");
                }
                catch (Exception ex)
                {
                    nRow.CreateCell(8).SetCellValue("ERROR!");
                    nRow.CreateCell(10).SetCellValue(ex.Message);
                    Console.WriteLine(ex.Message);
                    rIdx += 1;
                }
            }
            using (var nImportFile = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                nWorkBook.Write(nImportFile, false);

            return records;
        }
    }
}
