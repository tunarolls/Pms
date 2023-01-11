using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls
{
    public class CBCExporter : IExportBankReportService
    {
        private string _bankName;
        private Cutoff _cutoff;
        private string _payrollCode;
        public CBCExporter(Cutoff cutoff, string payrollCode, string bankName = "")
        {
            _cutoff = cutoff;
            _payrollCode = payrollCode;
            _bankName = bankName;
        }

        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = $@"{startupPath}\EXPORT\{_cutoff.CutoffId}\{_payrollCode}\BANK REPORT\{_bankName}";
            string filename = $@"{filePath}\{_payrollCode}_{_cutoff.CutoffDate:yyyyMMdd}-{_bankName}.xls";
            Directory.CreateDirectory(filePath);
            
            string templatePath = $@"{startupPath}\TEMPLATES\CBC.xls";
            File.Copy(templatePath, filename,true);

            payrolls = payrolls.OrderBy(p => p.EE.Fullname);

            GenerateXls(filename, payrolls.ToArray());
        }

        private static void GenerateXls(string filename, Payroll[] payrolls)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            WritePayrollToOriginalSheet(payrolls, nSheet);

            using (var nReportFile = new FileStream(filename, FileMode.Open, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }

        private static void WritePayrollToOriginalSheet(Payroll[] validayrolls, ISheet sheet)
        {
            if (validayrolls.Length > 0)
            {
                IRow row;
                for (int i = 0; i < validayrolls.Length; i++)
                {
                    Payroll payroll = validayrolls[i];
                    row = sheet.GetRow(i + 4);
                    row.GetCell(3).SetCellValue(payroll.EE.AccountNumber);
                    row.GetCell(4).SetCellValue(payroll.EE.LastName);
                    row.GetCell(5).SetCellValue(payroll.EE.FirstName);
                    row.GetCell(6).SetCellValue(payroll.EE.MiddleName);
                    row.GetCell(7).SetCellValue(payroll.NetPay);
                }
            }
        }
    }
}
