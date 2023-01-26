using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Pms.Payrolls.ServiceLayer.Files.Export.BankReport
{
    public class CBCExporter : IExportBankReportService
    {
        private Cutoff _cutoff;
        private string _payrollCode;
        private string _bankName;

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
            File.Copy(templatePath, filename, true);

            payrolls = payrolls.OrderBy(p => p.EE.Fullname);

            GenerateXls(filename, payrolls.ToArray());
            GenerateTxt(filename, payrolls.ToArray());
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
                    row.GetCell(7).SetCellValue(Math.Round(payroll.NetPay, 2));
                }
            }
        }



        private static void GenerateTxt(string filename, Payroll[] payrolls)
        {
            filename = Path.ChangeExtension(filename, "txt");

            //IWorkbook nWorkbook;
            using (var writer = new StreamWriter(filename))
            {
                WriteDTL(payrolls, writer);
                writer.Write("TRL");
                writer.Write("|{0}", payrolls.Length);
                writer.Write("|{0}", payrolls.Sum(p => Math.Round(p.NetPay, 2)));

            }
        }


        public static void WriteDTL(Payroll[] validayrolls, StreamWriter writer)
        {
            if (validayrolls.Length > 0)
            {
                for (int i = 0; i < validayrolls.Length; i++)
                {
                    Payroll payroll = validayrolls[i];
                    writer.Write("DTL");
                    writer.Write("|{0}", payroll.EE.AccountNumber);
                    writer.Write("|{0}", payroll.EE.LastName);
                    writer.Write("|{0}", payroll.EE.FirstName);
                    writer.Write("|{0}", payroll.EE.MiddleName);
                    writer.Write("|{0}", payroll.NetPay);
                    writer.Write("|");//Mobile Number
                    writer.Write("|");//Email Address
                    writer.WriteLine("|{0}", Hash(payroll.EE.AccountNumber, Math.Round(payroll.NetPay, 2)));
                }
            }
        }

        public static string Hash(string accountNumber, double amount)
        {
            string famt = string.Join("", amount.ToString("0.00").Split("."));
            string rawHash = $"{Int64.Parse(accountNumber):D19}{Int64.Parse(famt):D13}";
            Regex splitterRegex = new(@"(\d{4})(\d{4})(\d{4})(\d{4})(\d{4})(\d{4})(\d{4})(\d{4})");
            var matchResult = splitterRegex.Match(rawHash);
            Int64[] grpStrings;
            if (matchResult.Success)
            {
                IEnumerable<string> da1 = matchResult.Groups.Values.Select(g => g.Value).Skip(1).ToList();
                List<byte[]> da2 = da1.Select(g => Encoding.ASCII.GetBytes(g)).ToList();
                List<string> da3 = da2.Select(g => string.Join("", g.Select(gg => $"{gg:D3}"))).ToList();
                grpStrings = da3.Select(g => Int64.Parse(g)).ToArray();
            }
            else return string.Empty;

            Int64 prod1 = grpStrings[0] * 1;
            Int64 prod2 = grpStrings[1] * 3 * (GetFirstDigit(prod1) > 0 ? GetFirstDigit(prod1) : 1);
            Int64 prod3 = grpStrings[2] * 7 * (GetFirstDigit(prod2) > 0 ? GetFirstDigit(prod2) : 1);
            Int64 prod4 = grpStrings[3] * 1 * (GetFirstDigit(prod3) > 0 ? GetFirstDigit(prod3) : 1);
            Int64 prod5 = grpStrings[4] * 3 * (GetFirstDigit(prod4) > 0 ? GetFirstDigit(prod4) : 1);
            Int64 prod6 = grpStrings[5] * 7 * (GetFirstDigit(prod5) > 0 ? GetFirstDigit(prod5) : 1);
            Int64 prod7 = grpStrings[6] * 1 * (GetFirstDigit(prod6) > 0 ? GetFirstDigit(prod6) : 1);
            Int64 prod8 = grpStrings[7] * 3 * (GetFirstDigit(prod7) > 0 ? GetFirstDigit(prod7) : 1);

            string prodSum = (prod1 + prod2 + prod3 + prod4 + prod5 + prod6 + prod7 + prod8).ToString();

            int startIndex = prodSum.Length > 12 ? prodSum.Length - 12 : 0;
            return prodSum.Substring(startIndex);
        }

        private static int GetFirstDigit(Int64 val) => Convert.ToInt32(val.ToString().Last().ToString());
    }
}
