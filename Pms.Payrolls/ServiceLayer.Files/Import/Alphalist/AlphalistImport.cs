using DotNetDBF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.ServiceLayer.Files;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Payrolls.ServiceLayer.Files.Import.Alphalist
{
    public class AlphalistImport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void ImportToBIRProgram(string alphalistFilepath, string birProgramDataDirectory, CompanyView company, int year)
        {
            IWorkbook workbook;
            using (var alphalistFile = new FileStream(alphalistFilepath, FileMode.Open, FileAccess.Read))
                workbook = new HSSFWorkbook(alphalistFile);

            List<AlphalistDetail> alphalists = new();
            ISheet sheet = workbook.GetSheet("D1");

            int sequence = 1;
            for (int rowIndex = 1; rowIndex < sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row is not null && row.GetCell(0) is not null && !string.IsNullOrEmpty(row.GetCell(0).StringCellValue))
                    alphalists.Add(GetDetail(row, company, "D1", year, sequence));
                else break;
                sequence++;
            }
            sheet = workbook.GetSheet("D2");
            for (int rowIndex = 1; rowIndex < sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row is not null && row.GetCell(0) is not null && !string.IsNullOrEmpty(row.GetCell(0).StringCellValue))
                    alphalists.Add(GetDetail(row, company, "D2", year, sequence));
                else break;
                sequence++;
            }


            string connectionString = $"Provider=vfpoledb;Data Source={birProgramDataDirectory};Extended Properties=dBase IV";
            OleDbConnection olecon = new(connectionString);
            olecon.Open();

            OleDbCommand olecom = new OleDbCommand() { Connection = olecon };
            olecom.CommandText = "DELETE FROM Alphadtl.DBF;";
            olecom.ExecuteNonQuery();

            try
            {
                string headers = string.Join(",", GetHeadersMaxLength10());
                StartSaving(headers, alphalists, olecom);
            }
            catch (Exception)
            {
                string headers = string.Join(",", GetHeaders());
                StartSaving(headers, alphalists, olecom);
            }
            olecon.Close();
        }
        private static int append(ref int index)
        {
            index++;
            return index;
        }

        private string[] ConvertValuesToInsertableString(object[] values)
        {
            List<string> convertedValues = new();
            foreach (var value in values)
            {
                if (value is string)
                    convertedValues.Add($"'{value}'");
                else if (value is int)
                    convertedValues.Add($"{value}");
                else if (value is double)
                    convertedValues.Add($"{value:0.00}");
                else if (value is DateTime)
                    convertedValues.Add($"{{{value:MM/dd/yyyy}}}");
            }
            return convertedValues.ToArray();
        }

        private AlphalistDetail GetDetail(IRow row, CompanyView company, string schedule, int year, int sequence)
        {
            AlphalistDetail alpha = new();
            alpha.EmployerBranchCode = company.BranchCode.ToString("0000");
            alpha.EmployerTin = company.TIN;
            alpha.ReturnPeriod = DateTime.Parse($"{year}-12-31");
            alpha.ScheduleNumber = schedule;
            alpha.SequenceNumber = sequence;
            alpha.RegisteredName = company.RegisteredName;

            alpha.FormType = "1604C";
            alpha.BranchCode = "0000";
            alpha.RegionNumber = company.Region;
            alpha.SubsFiling = "Y";

            int columnIndex = -1;
            alpha.EEId = row.GetCell(append(ref columnIndex)).StringCellValue;
            alpha.FirstName = row.GetCell(append(ref columnIndex)).StringCellValue;
            alpha.LastName = row.GetCell(append(ref columnIndex)).StringCellValue;
            alpha.MiddleName = row.GetCell(append(ref columnIndex)).StringCellValue;
            alpha.Tin = row.GetCell(append(ref columnIndex)).GetValue();
            alpha.StartDate = DateTime.Parse(row.GetCell(append(ref columnIndex)).StringCellValue);
            alpha.ResignationDate = DateTime.Parse(row.GetCell(append(ref columnIndex)).StringCellValue);
            alpha.FactorUsed = (int)row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.AcutalAmountWithheld = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentTaxableSalary = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentTaxable13thMonth = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentTaxWithheld = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableSalary = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxable13thMonth = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableSssGsisOtherContribution = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.OverWithheld = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.AmmountWithheldOnDecember = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.TaxDue = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.NetTaxableCompensationIncome = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.GrossCompensationIncome = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableDeMinimis = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentTotalCompensation = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentTotalNonTaxableCompensationIncome = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableGrossCompensationIncome = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableBasicSmwDay = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableBasicSmwMonth = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableBasicSmwYear = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableHolidayPay = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableOvertimePay = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableNightDifferential = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.PresentNonTaxableHazardPay = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.NonTaxableBasicSalary = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.TaxableBasicSalary = row.GetCell(append(ref columnIndex)).NumericCellValue;
            alpha.Nationality = row.GetCell(append(ref columnIndex)).GetValue();
            alpha.ReasonForSeparation = row.GetCell(append(ref columnIndex)).GetValue();
            alpha.EmploymentStatus = row.GetCell(append(ref columnIndex)).GetValue();
            //alpha.December = row.GetCell(append(ref columnIndex)).NumericCellValue;
            //alpha.OverWithheld = row.GetCell(append(ref columnIndex)).NumericCellValue;


            return alpha;
        }

        private string[] GetHeaders()
        {
            return new[] {
                "FORM_TYPE",
                "EMPLOYER_TIN",
                "EMPLOYER_BRANCH_CODE",
                "RETRN_PERIOD",
                "SCHEDULE_NUM",
                "SEQUENCE_NUM",
                "REGISTERED_NAME",
                "FIRST_NAME",
                "LAST_NAME",
                "MIDDLE_NAME",
                "TIN",
                "BRANCH_CODE",
                "EMPLOYMENT_FROM",
                "EMPLOYMENT_TO",
                "ATC_CODE",
                "STATUS_CODE",
                "REGION_NUM",
                "SUBS_FILING",
                "EXMPN_CODE",
                "FACTOR_USED",
                "ACTUAL_AMT_WTHLD",
                "INCOME_PAYMENT",
                "PRES_TAXABLE_SALARIES",
                "PRES_TAXABLE_13TH_MONTH",
                "PRES_TAX_WTHLD",
                "PRES_NONTAX_SALARIES",
                "PRES_NONTAX_13TH_MONTH",
                "PREV_TAXABLE_SALARIES",
                "PREV_TAXABLE_13TH_MONTH",
                "PREV_TAX_WTHLD",
                "PREV_NONTAX_SALARIES",
                "PREV_NONTAX_13TH_MONTH",
                "PRES_NONTAX_SSS_GSIS_OTH_CONT",
                "PREV_NONTAX_SSS_GSIS_OTH_CONT",
                "TAX_RATE",
                "OVER_WTHLD",
                "AMT_WTHLD_DEC",
                "EXMPN_AMT",
                "TAX_DUE",
                "HEATH_PREMIUM",
                "FRINGE_BENEFIT",
                "MONETARY_VALUE",
                "NET_TAXABLE_COMP_INCOME",
                "GROSS_COMP_INCOME",
                "PREV_NONTAX_DE_MINIMIS",
                "PREV_TOTAL_NONTAX_COMP_INCOME",
                "PREV_TAXABLE_BASIC_SALARY",
                "PRES_NONTAX_DE_MINIMIS",
                "PRES_TAXABLE_BASIC_SALARY",
                "PRES_TOTAL_COMP",
                "PREV_PRES_TOTAL_TAXABLE",
                "PRES_TOTAL_NONTAX_COMP_INCOME",
                "PREV_NONTAX_GROSS_COMP_INCOME",
                "PREV_NONTAX_BASIC_SMW",
                "PREV_NONTAX_HOLIDAY_PAY",
                "PREV_NONTAX_OVERTIME_PAY",
                "PREV_NONTAX_NIGHT_DIFF",
                "PREV_NONTAX_HAZARD_PAY",
                "PRES_NONTAX_GROSS_COMP_INCOME",
                "PRES_NONTAX_BASIC_SMW_DAY",
                "PRES_NONTAX_BASIC_SMW_MONTH",
                "PRES_NONTAX_BASIC_SMW_YEAR",
                "PRES_NONTAX_HOLIDAY_PAY",
                "PRES_NONTAX_OVERTIME_PAY",
                "PRES_NONTAX_NIGHT_DIFF",
                "PREV_PRES_TOTAL_COMP_INCOME",
                "PRES_NONTAX_HAZARD_PAY",
                "TOTAL_NONTAX_COMP_INCOME",
                "TOTAL_TAXABLE_COMP_INCOME",
                "PREV_TOTAL_TAXABLE",
                "NONTAX_BASIC_SAL",
                "TAX_BASIC_SAL",
                "QRT_NUM",
                "QUARTERDATE",
                "NATIONALITY",
                "REASON_SEPARATION",
                "EMPLOYMENT_STATUS",
                "ADDRESS1",
                "ADDRESS2",
                "ATC_DESC",
                "DATE_DEATH",
                "DATE_WTHELD",
            };
        }

        private string[] GetHeadersMaxLength10()
        {
            return new[] {
                "FORM_TYPE",
                "EMPLOYER_T",
                "EMPLOYER_B",
                "RETRN_PERI",
                "SCHEDULE_N",

                "SEQUENCE_N",
                "REGISTERED",
                "FIRST_NAME",
                "LAST_NAME",
                "MIDDLE_NAM",

                "TIN",
                "BRANCH_COD",
                "EMPLOYMENT",
                "EMPLOYMEN2",
                "ATC_CODE",

                "STATUS_COD",
                "REGION_NUM",
                "SUBS_FILIN",
                "EXMPN_CODE",
                "FACTOR_USE",

                "ACTUAL_AMT",
                "INCOME_PAY",
                "PRES_TAXAB",
                "PRES_TAXA2",
                "PRES_TAX_W",

                "PRES_NONTA",
                "PRES_NONT2",
                "PREV_TAXAB",
                "PREV_TAXA2",
                "PREV_TAX_W",

                "PREV_NONTA",
                "PREV_NONT2",
                "PRES_NONT3",
                "PREV_NONT3",
                "TAX_RATE",

                "OVER_WTHLD",
                "AMT_WTHLD_",
                "EXMPN_AMT",
                "TAX_DUE",
                "HEATH_PREM",


                "FRINGE_BEN",
                "MONETARY_V",
                "NET_TAXABL",
                "GROSS_COMP",
                "PREV_NONT4",

                "PREV_TOTAL",
                "PREV_TAXA3",
                "PRES_NONT4",
                "PRES_TAXA3",
                "PRES_TOTAL",

                "PREV_PRES_",
                "PRES_TOTA2",
                "PREV_NONT5",
                "PREV_NONT6",
                "PREV_NONT7",

                "PREV_NONT8",
                "PREV_NONT9",
                "PREV_NON10",
                "PRES_NONT5",
                "PRES_NONT6",

                "PRES_NONT7",
                "PRES_NONT8",
                "PRES_NONT9",
                "PRES_NON10",
                "PRES_NON11",

                "PREV_PRES2",
                "PRES_NON12",
                "TOTAL_NONT",
                "TOTAL_TAXA",
                "PREV_TOTA2",

                "NONTAX_BAS",
                "TAX_BASIC_",
                "QRT_NUM",
                "QUARTERDAT",
                "NATIONALIT",

                "REASON_SEP",
                "EMPLOYMEN3",
                "ADDRESS1",
                "ADDRESS2",
                "ATC_DESC",

                "DATE_DEATH",
                "DATE_WTHEL",
            };
        }

        private object[] GetValues(AlphalistDetail alpha)
        {
            return new object[]
            {
                alpha.FormType,
                alpha.EmployerTin,
                alpha.EmployerBranchCode,
                alpha.ReturnPeriod,
                alpha.ScheduleNumber,

                alpha.SequenceNumber,
                alpha.RegisteredName,
                alpha.FirstName,
                alpha.LastName,
                alpha.MiddleName,

                alpha.Tin,
                alpha.BranchCode,
                alpha.StartDate,
                alpha.ResignationDate,
                alpha.AtcCode,

                alpha.StatusCode,
                alpha.RegionNumber,
                alpha.SubsFiling,
                alpha.ExmpnCode,
                alpha.FactorUsed,

                alpha.AcutalAmountWithheld,
                alpha.IncomePayment,
                alpha.PresentTaxableSalary,
                alpha.PresentTaxable13thMonth,
                alpha.PresentTaxWithheld,

                alpha.PresentNonTaxableSalary,
                alpha.PresentNonTaxable13thMonth,
                alpha.PreviousTaxableSalary,
                alpha.PreviousTaxable13thMonth,
                alpha.PreviousTaxWithheld,

                alpha.PreviousNonTaxableSalary,
                alpha.PreviousNonTaxable13thMonth,
                alpha.PresentNonTaxableSssGsisOtherContribution,
                alpha.PreviousNonTaxableSssGsisOtherContribution,
                alpha.TaxRate,

                alpha.OverWithheld,
                alpha.AmmountWithheldOnDecember,
                alpha.ExmpnAmount,
                alpha.TaxDue,
                alpha.HeathPremium,

                alpha.FringeBenefit,
                alpha.MonetaryValue,
                alpha.NetTaxableCompensationIncome,
                alpha.GrossCompensationIncome,
                alpha.PreviousNonTaxableDeMinimis,

                alpha.PreviousTotalNonTaxableCompensationIncome,
                alpha.PreviousTaxableBasicSalary,
                alpha.PresentNonTaxableDeMinimis,
                alpha.PresentTaxableBasicSalary,
                alpha.PresentTotalCompensation,

                alpha.PreviousAndPresentTotalTaxable,
                alpha.PresentTotalNonTaxableCompensationIncome,
                alpha.PreviousNonTaxableGrossCompensationIncome,
                alpha.PreviousNonTaxableBasicSmw,
                alpha.PreviousNonTaxableHolidayPay,


                alpha.PreviousNonTaxableOvertimePay,
                alpha.PreviousNonTaxableNightDifferential,
                alpha.PreviousNonTaxableHazardPay,
                alpha.PresentNonTaxableGrossCompensationIncome,
                alpha.PresentNonTaxableBasicSmwDay,

                alpha.PresentNonTaxableBasicSmwMonth,
                alpha.PresentNonTaxableBasicSmwYear,
                alpha.PresentNonTaxableHolidayPay,
                alpha.PresentNonTaxableOvertimePay,
                alpha.PresentNonTaxableNightDifferential,

                alpha.PreviousAndPresentTotalCompensationIncome,
                alpha.PresentNonTaxableHazardPay,
                alpha.TotalNontaxableCompensationIncome,
                alpha.TotalTaxableCompensationIncome,
                alpha.PreviousTotalTaxable,

                alpha.NonTaxableBasicSalary,
                alpha.TaxableBasicSalary,
                alpha.QrtNumber,
                alpha.QuarterDate,
                alpha.Nationality,

                alpha.ReasonForSeparation,
                alpha.EmploymentStatus,
                alpha.Address1,
                alpha.Address2,
                alpha.AtcDescription,

                alpha.DateOfDeath,
                alpha.DateWithheld,
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void StartSaving(string headers, List<AlphalistDetail> alphalists, OleDbCommand olecom)
        {
            foreach (AlphalistDetail alpha in alphalists)
            {
                object[] values = GetValues(alpha);
                string convertedValues = string.Join(",", ConvertValuesToInsertableString(values));
                olecom.CommandText = $"INSERT INTO Alphadtl.DBF ({headers}) VALUES({convertedValues});";
                olecom.ExecuteNonQuery();
            }
        }
    }
}

//"FORM_TYPE",
//"EMPLOYER_TIN",
//"EMPLOYER_BRANCH_CODE",
//"RETRN_PERIOD",
//"SCHEDULE_NUM",
//"SEQUENCE_NUM",
//"REGISTERED_NAME",
//"FIRST_NAME",
//"LAST_NAME",
//"MIDDLE_NAME",
//"TIN",
//"BRANCH_CODE",
//"EMPLOYMENT_FROM",
//"EMPLOYMENT_TO",
//"ATC_CODE",
//"STATUS_CODE",
//"REGION_NUM",
//"SUBS_FILING",
//"EXMPN_CODE",
//"FACTOR_USED",
//"ACTUAL_AMT_WTHLD",
//"INCOME_PAYMENT",
//"PRES_TAXABLE_SALARIES",
//"PRES_TAXABLE_13TH_MONTH",
//"PRES_TAX_WTHLD",
//"PRES_NONTAX_SALARIES",
//"PRES_NONTAX_13TH_MONTH",
//"PREV_TAXABLE_SALARIES",
//"PREV_TAXABLE_13TH_MONTH",
//"PREV_TAX_WTHLD",
//"PREV_NONTAX_SALARIES",
//"PREV_NONTAX_13TH_MONTH",
//"PRES_NONTAX_SSS_GSIS_OTH_CONT",
//"PREV_NONTAX_SSS_GSIS_OTH_CONT",
//"TAX_RATE",
//"OVER_WTHLD",
//"AMT_WTHLD_DEC",
//"EXMPN_AMT",
//"TAX_DUE",
//"HEATH_PREMIUM",
//"FRINGE_BENEFIT",
//"MONETARY_VALUE",
//"NET_TAXABLE_COMP_INCOME",
//"GROSS_COMP_INCOME",
//"PREV_NONTAX_DE_MINIMIS",
//"PREV_TOTAL_NONTAX_COMP_INCOME",
//"PREV_TAXABLE_BASIC_SALARY",
//"PRES_NONTAX_DE_MINIMIS",
//"PRES_TAXABLE_BASIC_SALARY",
//"PRES_TOTAL_COMP",
//"PREV_PRES_TOTAL_TAXABLE",
//"PRES_TOTAL_NONTAX_COMP_INCOME",
//"PREV_NONTAX_GROSS_COMP_INCOME",
//"PREV_NONTAX_BASIC_SMW",
//"PREV_NONTAX_HOLIDAY_PAY",
//"PREV_NONTAX_OVERTIME_PAY",
//"PREV_NONTAX_NIGHT_DIFF",
//"PREV_NONTAX_HAZARD_PAY",
//"PRES_NONTAX_GROSS_COMP_INCOME",
//"PRES_NONTAX_BASIC_SMW_DAY",
//"PRES_NONTAX_BASIC_SMW_MONTH",
//"PRES_NONTAX_BASIC_SMW_YEAR",
//"PRES_NONTAX_HOLIDAY_PAY",
//"PRES_NONTAX_OVERTIME_PAY",
//"PRES_NONTAX_NIGHT_DIFF",
//"PREV_PRES_TOTAL_COMP_INCOME",
//"PRES_NONTAX_HAZARD_PAY",
//"TOTAL_NONTAX_COMP_INCOME",
//"TOTAL_TAXABLE_COMP_INCOME",
//"PREV_TOTAL_TAXABLE",
//"NONTAX_BASIC_SAL",
//"TAX_BASIC_SAL",
//"QRT_NUM",
//"QUARTERDATE",
//"NATIONALITY",
//"REASON_SEPARATION",
//"EMPLOYMENT_STATUS",
//"ADDRESS1",
//"ADDRESS2",
//"ATC_DESC",
//"DATE_DEATH",
//"DATE_WTHELD"

/*
                "FORM_TYPE",
                "EMPLOYER_T",
                "EMPLOYER_B",
                "RETRN_PERI",
                "SCHEDULE_N",

                "SEQUENCE_N",
                "REGISTERED",
                "FIRST_NAME",
                "LAST_NAME",
                "MIDDLE_NAM",

                "TIN",
                "BRANCH_COD",
                "EMPLOYMENT",
                "EMPLOYMEN2",
                "ATC_CODE",

                "STATUS_COD",
                "REGION_NUM",
                "SUBS_FILIN",
                "EXMPN_CODE",
                "FACTOR_USE",

                "ACTUAL_AMT",
                "INCOME_PAY",
                "PRES_TAXAB",
                "PRES_TAXA2",
                "PRES_TAX_W",

                "PRES_NONTA",
                "PRES_NONT2",
                "PREV_TAXAB",
                "PREV_TAXA2",
                "PREV_TAX_W",

                "PREV_NONTA",
                "PREV_NONT2",
                "PRES_NONT3",
                "PREV_NONT3",
                "TAX_RATE",

                "OVER_WTHLD",
                "AMT_WTHLD_",
                "EXMPN_AMT",
                "TAX_DUE",
                "HEATH_PREM",


                "FRINGE_BEN",
                "MONETARY_V",
                "NET_TAXABL",
                "GROSS_COMP",
                "PREV_NONT4",

                "PREV_TOTAL",
                "PREV_TAXA3",
                "PRES_NONT4",
                "PRES_TAXA3",
                "PRES_TOTAL",

                "PREV_PRES_",
                "PRES_TOTA2",
                "PREV_NONT5",
                "PREV_NONT6",
                "PREV_NONT7",

                "PREV_NONT8",
                "PREV_NONT9",
                "PREV_NON10",
                "PRES_NONT5",
                "PRES_NONT6",

                "PRES_NONT7",
                "PRES_NONT8",
                "PRES_NONT9",
                "PRES_NON10",
                "PRES_NON11",

                "PREV_PRES2",
                "PRES_NON12",
                "TOTAL_NONT",
                "TOTAL_TAXA",
                "PREV_TOTA2",

                "NONTAX_BAS",
                "TAX_BASIC_",
                "QRT_NUM",
                "QUARTERDAT",
                "NATIONALIT",

                "REASON_SEP",
                "EMPLOYMEN3",
                "ADDRESS1",
                "ADDRESS2",
                "ATC_DESC",

                "DATE_DEATH",
                "DATE_WTHEL",
                "N_NULLFLAG"
 */
