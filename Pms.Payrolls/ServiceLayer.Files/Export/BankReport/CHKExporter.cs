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

        public CHKExporter(Cutoff cutoff, string payrollCode)
        {
            _cutoff = cutoff;
            _payrollCode = payrollCode;
        }

        public static void WritePayrolls(Payroll[] payrolls, IWorkbook workbook)
        {
            ISheet xl200DOWNSheet = workbook.GetSheetAt(0);
            ISheet xl7500DOWNSheet = workbook.GetSheetAt(1);
            ISheet xl7500UPSheet = workbook.GetSheetAt(2);
            ISheet xl100KUPSheet = workbook.GetSheetAt(3);

            Payroll[] Records200DOWN = payrolls
                .Where(p => p.NetPay <= 200)
                .Where(p => p.NetPay > 0)
                .ToArray();
            Payroll[] Records7500DOWN = payrolls
                .Where(p => p.NetPay < 7500)
                .Where(p => p.NetPay > 200)
                .ToArray();
            Payroll[] Recordsd7500UP = payrolls
                .Where(p => p.NetPay >= 7500)
                .ToArray();
            Payroll[] Records100KUP = payrolls
                .Where(p => p.NetPay >= 100000)
                .ToArray();

            WriteToSheet(xl7500UPSheet, Recordsd7500UP);
            WriteToSheet(xl7500DOWNSheet, Records7500DOWN);
            WriteToSheet(xl200DOWNSheet, Records200DOWN);
            WriteToSheet(xl100KUPSheet, Records100KUP);
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
                row.CreateCell(2).SetCellValue(rec.NetPay);
            }
        }

        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = $@"{startupPath}\EXPORT\{_cutoff.CutoffId}\{_payrollCode}\BANK REPORT\CHK";
            string filename = $@"{filePath}\{_payrollCode}_{_cutoff.CutoffDate:yyyyMMdd}-CHK.xls";
            Directory.CreateDirectory(filePath);

            string templatePath = $@"{startupPath}\TEMPLATES\CHK.xls";
            File.Copy(templatePath, filename, true);

            payrolls = payrolls.OrderBy(p => p.EE.Fullname);
            GenerateXls(filename, payrolls.ToArray());
        }

        private static void GenerateXls(string filename, Payroll[] payrolls)
        {
            IWorkbook nWorkbook;
            using var nTemplateFile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            nWorkbook = new HSSFWorkbook(nTemplateFile);
            WritePayrolls(payrolls, nWorkbook);
            using var nReportFile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            nWorkbook.Write(nReportFile, false);
        }
    }
}
