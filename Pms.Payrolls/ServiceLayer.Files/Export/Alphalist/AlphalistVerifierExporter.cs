using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Export.Alphalist
{
    public class AlphalistVerifierExporter
    {
        public void StartExport(IEnumerable<IEnumerable<Payroll>> employeePayrolls, int year, string companyId)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = $@"{startupPath}\EXPORT\ALPHALIST";
            Directory.CreateDirectory(filePath);
            string filename = $"{filePath}\\{companyId}_{year}-Alpha Verifier.xls";

            IWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet("ALL");
            WriteToSheet(employeePayrolls, sheet);

            using (var nTemplateFile = new FileStream(filename, FileMode.Create, FileAccess.Write))
                workbook.Write(nTemplateFile, false);
        }

        private static int Append(ref int index)
        {
            index++;
            return index;
        }

        private void WriteData(IRow row, Payroll payroll)
        {
            int columnIndex = -1;
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.Cutoff.CutoffDate.ToString("MMM dd, yyyy"));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.Rate);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.RegHours);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.Overtime);

            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.RestDayOvertime);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.HolidayOvertime);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.NightDifferential);

            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.AbsTar);

            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.EmployeeSSS);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.EmployeePagibig);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.EmployeePhilHealth);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.WithholdingTax);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.RegularPay);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.AdjustedRegPay);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.GrossPay);
            row.CreateCell(Append(ref columnIndex)).SetCellValue(payroll.NetPay);
        }

        private void WriteEmployeeData(IRow row, string? eeId, EmployeeView? employee)
        {
            int columnIndex = -1;
            row.CreateCell(Append(ref columnIndex)).SetCellValue(eeId);
            if (employee is not null)
            {
                row.CreateCell(Append(ref columnIndex)).SetCellValue(employee.FullName);
                row.CreateCell(Append(ref columnIndex)).SetCellValue(employee.TIN);
            }
        }

        private void WriteHeader(IRow row)
        {
            int columnIndex = -1;
            row.CreateCell(Append(ref columnIndex)).SetCellValue("CUTOFF DATE");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("RATE");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("REG HRS");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("OT");

            row.CreateCell(Append(ref columnIndex)).SetCellValue("R_OT");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("H_OT");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("ND");

            row.CreateCell(Append(ref columnIndex)).SetCellValue("TARDY");

            row.CreateCell(Append(ref columnIndex)).SetCellValue("SSS");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("PAGIBIG");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("PHIC");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("TAX");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("REG PAY");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("REG PAY_13th MONTH");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("GROSS PAY");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("NET PAY");
        }

        private void WriteToSheet(IEnumerable<IEnumerable<Payroll>> employeePayrolls, ISheet sheet)
        {
            int rowIndex = -1;
            IRow row = sheet.CreateRow(Append(ref rowIndex));
            WriteHeader(row);

            foreach (IEnumerable<Payroll> employeePayroll in employeePayrolls)
            {
                Payroll temp = employeePayroll.First();
                WriteEmployeeData(sheet.CreateRow(Append(ref rowIndex)), temp.EEId, temp.EE);

                foreach (Payroll payroll in employeePayroll)
                    WriteData(sheet.CreateRow(Append(ref rowIndex)), payroll);

                Append(ref rowIndex);
                WriteTotal(sheet.CreateRow(Append(ref rowIndex)), employeePayroll);
                Append(ref rowIndex);
                Append(ref rowIndex);
            }
        }

        private void WriteTotal(IRow row, IEnumerable<Payroll> employeePayroll)
        {
            int columnIndex = -1;
            row.CreateCell(Append(ref columnIndex)).SetCellValue("TOTAL");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("");

            row.CreateCell(Append(ref columnIndex)).SetCellValue("");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("");
            row.CreateCell(Append(ref columnIndex)).SetCellValue("");

            row.CreateCell(Append(ref columnIndex)).SetCellValue("");

            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.EmployeeSSS));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.EmployeePagibig));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.EmployeePhilHealth));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.WithholdingTax));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.RegularPay));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.AdjustedRegPay));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.GrossPay));
            row.CreateCell(Append(ref columnIndex)).SetCellValue(employeePayroll.Sum(p => p.NetPay));
        }
    }
}