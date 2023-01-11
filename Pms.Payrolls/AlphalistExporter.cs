using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls
{
    public class AlphalistExporter
    {
        public void StartExport(IEnumerable<AlphalistDetail> alphalists, int year, string companyId,double minimumRate)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = $@"{startupPath}\EXPORT\Alphalist";
            Directory.CreateDirectory(filePath);
            string filename = $"{companyId}_{year}-Alpha".AppendFile(filePath);

            IWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet("D1");
            WriteToSheet(alphalists.Where(a => a.PresentNonTaxableBasicSmwHour > minimumRate), sheet, AlphalistScheduleNumberChoices.D1);

            sheet = workbook.CreateSheet("D2");
            WriteToSheet(alphalists.Where(a => a.PresentNonTaxableBasicSmwHour <= minimumRate), sheet, AlphalistScheduleNumberChoices.D2);

            using (var nTemplateFile = new FileStream(filename, FileMode.Create, FileAccess.Write))
                workbook.Write(nTemplateFile, false);
        }

        public void WriteToSheet(IEnumerable<AlphalistDetail> alphalists, ISheet sheet, AlphalistScheduleNumberChoices type)
        {
            int i = -1;
            IRow row = sheet.CreateRow(Append(ref i));
            WriteHeader(row);

            foreach (AlphalistDetail alpha in alphalists)
                WriteData(sheet.CreateRow(Append(ref i)), alpha);
        }

        private static int Append(ref int index)
        {
            index++;
            return index;
        }

        private void WriteData(IRow row, AlphalistDetail alpha)
        {
            int index = -1;
            row.CreateCell(Append(ref index)).SetCellValue(alpha.EEId);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.FirstName);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.LastName);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.MiddleName);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.Tin);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.StartDate.ToString("yyyy-MM-dd"));
            row.CreateCell(Append(ref index)).SetCellValue(alpha.ResignationDate.ToString("yyyy-MM-dd"));
            row.CreateCell(Append(ref index)).SetCellValue(alpha.FactorUsed);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.AcutalAmountWithheld);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentTaxableSalary);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentTaxable13thMonth);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentTaxWithheld);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableSalary);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxable13thMonth);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableSssGsisOtherContribution);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.OverWithheld);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.AmmountWithheldOnDecember);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.TaxDue);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.NetTaxableCompensationIncome);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.GrossCompensationIncome);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableDeMinimis);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentTotalCompensation);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentTotalNonTaxableCompensationIncome);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableGrossCompensationIncome);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableBasicSmwDay);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableBasicSmwMonth);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableBasicSmwYear);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableHolidayPay);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableOvertimePay);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableNightDifferential);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.PresentNonTaxableHazardPay);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.NonTaxableBasicSalary);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.TaxableBasicSalary);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.Nationality);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.ReasonForSeparation);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.EmploymentStatus);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.December);
            row.CreateCell(Append(ref index)).SetCellValue(alpha.OverWithheld);
        }

        private void WriteHeader(IRow row)
        {
            int index = -1;
            row.CreateCell(Append(ref index)).SetCellValue("ID");
            row.CreateCell(Append(ref index)).SetCellValue("FIRST_NAME");
            row.CreateCell(Append(ref index)).SetCellValue("LAST_NAME");
            row.CreateCell(Append(ref index)).SetCellValue("MIDDLE_NAME");
            row.CreateCell(Append(ref index)).SetCellValue("TIN");
            row.CreateCell(Append(ref index)).SetCellValue("EMPLOYMENT_FROM");
            row.CreateCell(Append(ref index)).SetCellValue("EMPLOYMENT_TO");
            row.CreateCell(Append(ref index)).SetCellValue("FACTOR_USED");
            row.CreateCell(Append(ref index)).SetCellValue("ACTUAL_AMT_WTHLD");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_TAXABLE_SALARIES");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_TAXABLE_13TH_MONTH");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_TAX_WTHLD");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_SALARIES");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_13TH_MONTH");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_SSS_GSIS_OTH_CONT");
            row.CreateCell(Append(ref index)).SetCellValue("OVER_WTHLD");
            row.CreateCell(Append(ref index)).SetCellValue("AMT_WTHLD_DEC");
            row.CreateCell(Append(ref index)).SetCellValue("TAX_DUE");
            row.CreateCell(Append(ref index)).SetCellValue("NET_TAXABLE_COMP_INCOME");
            row.CreateCell(Append(ref index)).SetCellValue("GROSS_COMP_INCOME");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_DE_MINIMIS");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_TOTAL_COMP");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_TOTAL_NONTAX_COMP_INCOME");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_GROSS_COMP_INCOME");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_BASIC_SMW_DAY");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_BASIC_SMW_MONTH");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_BASIC_SMW_YEAR");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_HOLIDAY_PAY");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_OVERTIME_PAY");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_NIGHT_DIFF");
            row.CreateCell(Append(ref index)).SetCellValue("PRES_NONTAX_HAZARD_PAY");
            row.CreateCell(Append(ref index)).SetCellValue("NONTAX_BASIC_SAL");
            row.CreateCell(Append(ref index)).SetCellValue("TAX_BASIC_SAL");
            row.CreateCell(Append(ref index)).SetCellValue("NATIONALITY");
            row.CreateCell(Append(ref index)).SetCellValue("REASON_SEPARATION");
            row.CreateCell(Append(ref index)).SetCellValue("EMPLOYMENT_STATUS");
            row.CreateCell(Append(ref index)).SetCellValue("December");
            row.CreateCell(Append(ref index)).SetCellValue("Final");
        }
    }
}