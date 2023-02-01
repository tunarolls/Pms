using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Export.BankReport
{
    public class CHKExporter : IExportBankReportService
    {
        private Cutoff _cutoff;
        private string _payrollCode;

        private string _site;
        private double minimumSalary;

        public CHKExporter(Cutoff cutoff, string payrollCode)
        {
            _cutoff = cutoff;
            _payrollCode = payrollCode;

            _site = _payrollCode.First().ToString();
            if (_site == "M") minimumSalary = 7500;
            else minimumSalary = 5000;
        }

        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = $@"{startupPath}\EXPORT\{_cutoff.CutoffId}\{_payrollCode}\BANK REPORT\CHK";
            string filename = $@"{filePath}\{_payrollCode}_{_cutoff.CutoffDate:yyyyMMdd}-CHK.xls";
            Directory.CreateDirectory(filePath);

            string templatePath = $@"{startupPath}\TEMPLATES\CHK-{_site}.xls";
            File.Copy(templatePath, filename, true);

            payrolls = payrolls.OrderBy(p => p.EE.FullName);
            GenerateXls(filename, payrolls.ToArray());
        }

        private void GenerateXls(string filename, Payroll[] payrolls)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);

            WritePayrolls(payrolls, nWorkbook);

            using (var nReportFile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }


        public void WritePayrolls(Payroll[] payrolls, IWorkbook workbook)
        {
            // Get sheets from workbook
            ISheet xl200DOWNSheet = workbook.GetSheetAt(0);
            ISheet xl7500DOWNSheet = workbook.GetSheetAt(1);
            ISheet xl7500UPSheet = workbook.GetSheetAt(2);
            ISheet xl100KUPSheet = workbook.GetSheetAt(3);

            // Filter payrolls into different arrays based on NetPay value
            Payroll[] payrollsUnder200 = payrolls
                .Where(p => p.NetPay <= 200 && p.NetPay > 0)
                .ToArray();
            Payroll[] payrollsBetween200And7500 = payrolls
                .Where(p => p.NetPay > 200 && p.NetPay < minimumSalary)
                .ToArray();
            Payroll[] payrollsOver7500 = payrolls
                .Where(p => p.NetPay >= minimumSalary)
                .ToArray();
            Payroll[] payrollsOver100000 = payrolls
                .Where(p => p.NetPay >= 100000)
                .ToArray();

            // Write payrolls to sheets
            WriteToSheet(xl7500UPSheet, payrollsOver7500);
            WriteToSheet(xl7500DOWNSheet, payrollsBetween200And7500);
            WriteToSheet(xl200DOWNSheet, payrollsUnder200);
            WriteToSheet(xl100KUPSheet, payrollsOver100000);

        }

        public static void WriteToSheet(ISheet sheet, Payroll[] payrollRecords)
        {
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("IDNo");
            row.CreateCell(1).SetCellValue("Fullname");
            row.CreateCell(2).SetCellValue("Amount");

            for (int i = 0, loopTo = payrollRecords.Count() - 1; i <= loopTo; i++)
            {
                var rec = payrollRecords[i];
                row = sheet.CreateRow(i + 1);
                row.CreateCell(0).SetCellValue(rec.EEId);
                row.CreateCell(1).SetCellValue(rec.EE.Fullname_FML);
                row.CreateCell(2).SetCellValue(Math.Round(rec.NetPay, 2));
            }
        }





    }
}
