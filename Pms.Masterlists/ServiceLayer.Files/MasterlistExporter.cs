using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Masterlists.Entities;
using Pms.Masterlists.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.Files
{
    public class MasterlistExporter
    {
        public void StartExport(IEnumerable<Employee> employees, PayrollCode payrollCode)
        {
            IWorkbook nWorkbook = new HSSFWorkbook();
            ISheet nSheet = nWorkbook.CreateSheet("Sheet1");


            int rowIndex = -1;

            WriteHeader(nSheet.CreateRow(Append(ref rowIndex)));
            foreach (Employee employee in employees)
                WriteRow(nSheet.CreateRow(Append(ref rowIndex)), employee);



            string filedirectory = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\MASTERLIST";
            Directory.CreateDirectory(filedirectory);

            string filename = "MASTER-ALL";
            if (payrollCode.PayrollCodeId != string.Empty)
                filename = $"MASTER-{payrollCode.PayrollCodeId}";

            string fullname = $@"{filedirectory}\{filename}-{DateTime.Now:yyyyMMdd}.xls";

            using (var nReportFile = new FileStream(fullname, FileMode.OpenOrCreate, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }

        private static int Append(ref int index)
        {
            index++;
            return index;
        }

        private void WriteHeader(IRow row)
        {
            int cellIndex = -1;
            row.CreateCell(Append(ref cellIndex)).SetCellValue("#");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("EE ID");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("FULL NAME");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("DEPARTMENT");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("JOBCODE");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("PAYROLL CODE");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("BANK");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("ACCOUNT NUMBER");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("CARD NUMBER");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("SSS");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("PHILHEALTH");
            row.CreateCell(Append(ref cellIndex)).SetCellValue("PAGIBIG");
        }

        private void WriteRow(IRow row, Employee employee)
        {
            int cellIndex = -1;
            row.CreateCell(Append(ref cellIndex)).SetCellValue(row.RowNum);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.EEId);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.Fullname);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.Location);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.JobCode);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.PayrollCode);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.Bank.ToString());
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.AccountNumber);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.CardNumber);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.SSS);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.PhilHealth);
            row.CreateCell(Append(ref cellIndex)).SetCellValue(employee.Pagibig);
        }
    }
}
