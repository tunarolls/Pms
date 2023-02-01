using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Export.BankReport
{
    public class LBPExporter : IExportBankReportService
    {
        private Cutoff _cutoff;
        private string _payrollCode;

        public LBPExporter(Cutoff cutoff, string payrollCode)
        {
            _cutoff = cutoff;
            _payrollCode = payrollCode;
        }

        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = $@"{startupPath}\EXPORT\{_cutoff.CutoffId}\{_payrollCode}\BANK REPORT\LBP";
            string filename = $@"{filePath}\{_payrollCode}_{_cutoff.CutoffDate:yyyyMMdd}-LBP.xls";
            Directory.CreateDirectory(filePath);

            string templatePath = $@"{startupPath}\TEMPLATES\LBP.xls";
            File.Copy(templatePath, filename, true);

            payrolls = payrolls.OrderBy(p => p.EE.FullName);
            IEnumerable<Payroll> validPayrolls = payrolls.Where(p => !p.IsReadyForExport());
            IEnumerable<Payroll> invalidPayrolls = payrolls.Where(p => p.IsReadyForExport());

            GenerateXls(filename, validPayrolls.ToArray(), invalidPayrolls.ToArray());
            GenerateCsvandDat(filename, validPayrolls.Count(), _payrollCode);
        }

        private static void GenerateCsvandDat(string filename, int payrollCount, string payrollCode)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);
            HSSFFormulaEvaluator formulator = new HSSFFormulaEvaluator(nWorkbook);
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            string csvFilename = Path.ChangeExtension(filename, "csv");
            string headerInitial = "HD";
            string footerInitial = "FT";
            string datFileInitial = "TXU";
            string bankCode = "019372";
            DateTime timestamp = DateTime.Now;
            string versionNumber = "2.0";
            int totalRecords = payrollCount;

            using (var streamWriter = new StreamWriter(csvFilename))
            {
                streamWriter.WriteLine($"{headerInitial}{bankCode}{timestamp:ddMMyyyyHHmmss}{versionNumber}");

                for (int i = 0; i < totalRecords; i++)
                {
                    IRow row = nSheet.GetRow(i + 2);

                    string item = row.GetCell(0).GetValue(formulator);
                    for (int j = 1; j < 26; j++)
                        item += $"|{row.GetCell(j).GetValue(formulator)}";
                    streamWriter.WriteLine(item);
                }

                streamWriter.WriteLine($"{footerInitial}{totalRecords:000000000}");
            }

            string datFilename = $@"{Path.GetDirectoryName(filename)}\{payrollCode}_{datFileInitial}{bankCode}{timestamp:ddMMyyHHmmss}.dat";
            File.Copy(csvFilename, datFilename);

            if (!File.Exists(filename))
                throw new FileNotFoundException("Bank Report was not generated successfully.");
        }

        private static void WritePayrollToOriginalSheet(Payroll[] validayrolls, ISheet sheet)
        {
            if (validayrolls.Length > 0)
            {
                IRow row;
                for (int i = 0; i < validayrolls.Length; i++)
                {
                    Payroll payroll = validayrolls[i];
                    row = sheet.GetRow(i + 2);
                    row.GetCell(0).SetCellValue(payroll.EE.CardNumber);
                    row.GetCell(1).SetCellValue(payroll.EE.AccountNumber);
                    row.GetCell(6).SetCellValue(payroll.NetPay * 100);
                }
            }
        }

        private static void WriteValidPayrollToSheet(Payroll[] validayrolls, ISheet sheet)
        {
            if (validayrolls.Length > 0)
            {
                IRow row;
                int firstIndex = 8;
                for (int i = 0; i < validayrolls.Length; i++)
                {
                    Payroll payroll = validayrolls[i];
                    row = sheet.CreateRow(i + firstIndex);
                    row.CreateCell(0).SetCellValue(i + 1);
                    row.CreateCell(1).SetCellValue(payroll.EEId);
                    row.CreateCell(2).SetCellValue(payroll.EE.FullName);
                    row.CreateCell(3).SetCellValue(payroll.EE.CardNumber);
                    row.CreateCell(4).SetCellValue(payroll.EE.AccountNumber);
                    row.CreateCell(5).SetCellValue(Math.Round(payroll.NetPay, 2));
                }

                //row = sheet.CreateRow(validayrolls.Length+ firstIndex + 2);
                //row.CreateCell(1).SetCellValue("TOTAL");
                //row.CreateCell(5).SetCellValue(validayrolls.Sum(p => p.NetPay));


                row = sheet.CreateRow(validayrolls.Length + firstIndex + 6);
                row.CreateCell(2).SetCellValue("Prepared By:");
                row.CreateCell(3).SetCellValue("Noted By:");
                row.CreateCell(4).SetCellValue("Approved By:");

                row = sheet.CreateRow(validayrolls.Length + firstIndex + 8);
                row.CreateCell(2).SetCellValue("");
                row.CreateCell(3).SetCellValue("Arlyn C. Esmenda");
                row.CreateCell(4).SetCellValue("Frances Ann B. Petilla");
            }
        }

        private void GenerateXls(string filename, Payroll[] payrolls, Payroll[] invalidPayrolls)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            WritePayrollToOriginalSheet(payrolls, nSheet);

            nSheet = nWorkbook.GetSheetAt(1);
            WritePayrollInformation(nSheet);
            WriteValidPayrollToSheet(payrolls, nSheet);

            //nSheet = nWorkbook.GetSheetAt(2);
            //WriteInvalidPayrollToSheet(invalidPayrolls, nSheet);

            using (var nReportFile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }

        private void WritePayrollInformation(ISheet sheet)
        {
            sheet.GetRow(0).GetCell(1).SetCellValue("N/A");
            sheet.GetRow(2).GetCell(1).SetCellValue("LANDBANK OF THE PHILIPPINES");
            sheet.GetRow(3).GetCell(2).SetCellValue(_cutoff.CutoffDate);
            sheet.GetRow(4).GetCell(2).SetCellValue(DateTime.Now);
            sheet.GetRow(5).GetCell(2).SetCellValue(DateTime.Now);
        }
        //private static void WriteInvalidPayrollToSheet(Payroll[] invalidPayrolls, ISheet sheet)
        //{
        //    if (invalidPayrolls.Length > 0)
        //    {
        //        IRow row;
        //        for (int i = 0; i < invalidPayrolls.Length; i++)
        //        {
        //            Payroll payroll = invalidPayrolls[i];
        //            row = sheet.CreateRow(i + 2);
        //            row.CreateCell(0).SetCellValue(payroll.EEId);
        //            if (payroll.EE is not null)
        //            {
        //                row.CreateCell(1).SetCellValue(payroll.EE.Fullname);
        //                row.CreateCell(2).SetCellValue(payroll.EE.CardNumber);
        //                row.CreateCell(3).SetCellValue(payroll.EE.AccountNumber);
        //            }
        //            row.CreateCell(4).SetCellValue(payroll.NetPay);
        //        }

        //        row = sheet.CreateRow(invalidPayrolls.Length + 3);
        //        row.CreateCell(0).SetCellValue("TOTAL");
        //        row.CreateCell(4).SetCellValue(invalidPayrolls.Sum(p => p.NetPay));
        //    }
        //}
    }
}
