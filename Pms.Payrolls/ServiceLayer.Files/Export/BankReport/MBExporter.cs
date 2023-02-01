using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Export.BankReport
{
    public class MBExporter : IExportBankReportService
    {
        private string _bankName;
        private Cutoff _cutoff;
        private string _payrollCode;
        public MBExporter(Cutoff cutoff, string payrollCode, string bankName = "")
        {
            _cutoff = cutoff;
            _payrollCode = payrollCode;

            _bankName = bankName;
        }

        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = $@"{startupPath}\EXPORT\{_cutoff.CutoffId}\{_payrollCode}\BANK REPORT\{_bankName}";
            Directory.CreateDirectory(filePath);
            string templatePath = $@"{startupPath}\TEMPLATES\MB.xls";

            string filename = $@"{filePath}\{_payrollCode}_{_cutoff.CutoffDate:yyyyMMdd}-{_bankName}.xls";
            File.Copy(templatePath, filename, true);

            payrolls = payrolls.OrderBy(p => p.EE.FullName);

            GenerateXls(filename, payrolls.ToArray());
        }


        private void GenerateXls(string filename, Payroll[] payrolls)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            WritePayrollInformation(nSheet);
            WritePayrollToOriginalSheet(payrolls, nSheet);

            using (var nReportFile = new FileStream(filename, FileMode.Open, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }

        private void WritePayrollInformation(ISheet sheet)
        {
            sheet.GetRow(0).GetCell(1).SetCellValue("N/A");
            sheet.GetRow(2).GetCell(1).SetCellValue("N/A");
            sheet.GetRow(3).GetCell(2).SetCellValue(_cutoff.CutoffDate);
            sheet.GetRow(4).GetCell(2).SetCellValue(DateTime.Now);
            sheet.GetRow(5).GetCell(2).SetCellValue(DateTime.Now);
        }


        private void WritePayrollToOriginalSheet(Payroll[] validayrolls, ISheet sheet)
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
                    row.CreateCell(1).SetCellValue(payroll.EE.LastName);
                    row.CreateCell(2).SetCellValue(payroll.EE.FirstName);
                    row.CreateCell(3).SetCellValue(payroll.EE.MiddleName);
                    row.CreateCell(4).SetCellValue(payroll.EE.AccountNumber);
                    row.CreateCell(5).SetCellValue(Math.Round(payroll.NetPay, 2));
                }

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
    }
}
