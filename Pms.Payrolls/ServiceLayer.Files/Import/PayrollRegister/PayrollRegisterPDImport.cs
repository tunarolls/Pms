using ExcelDataReader;
using Pms.Payrolls.Exceptions;
using Pms.Payrolls.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Import.PayrollRegister
{
    public class PayrollRegisterPDImport : IImportPayrollService
    {
        private int EEIdIndex = 1;
        private int RegularHoursIndex = 3;
        private int OvertimeIndex = 4;
        private int RestDayOvertimeIndex = 5;
        private int HolidayOvertimeIndex = 7;
        private int NightDifferentialIndex = 9;
        private int AbsentAndTardyIndex = 10;

        private int RegularPayIndex = 11;
        private int GrossIndex = 13;
        private int NetPayIndex = 19;

        private int EmployeeSSSIndex = 16;
        private int EmployerSSSIndex = 17;
        private int EmployeePagibigIndex = 12;
        private int EmployeePhilHealthIndex = 18;

        private int WithholdingTaxIndex = 15;

        private DateTime CutoffDate { get; set; }

        private string PayrollRegisterFilePath { get; set; } = string.Empty;


        public void ValidatePayRegisterFile()
        {
            if (CutoffDate == default) throw new PayrollRegisterHeaderNotFoundException("Cutoff Date", PayrollRegisterFilePath);
        }


        public IEnumerable<Payroll> StartImport(string payrollRegisterFilePath)
        {
            PayrollRegisterFilePath = payrollRegisterFilePath;

            List<Payroll> payrolls = new();
            using (var stream = File.Open(PayrollRegisterFilePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    FindPayrollDate(reader);
                    ValidatePayRegisterFile();

                    Cutoff cutoff = new Cutoff(CutoffDate);
                    do
                    {
                        string employee_id = "";
                        if (EEIdIndex > -1)
                        {
                            if (reader.GetString(EEIdIndex) is null)
                                continue;
                            employee_id = reader.GetString(EEIdIndex).Trim();
                        }

                        var newPayroll = new Payroll()
                        {
                            EEId = employee_id,
                            CutoffId = cutoff.CutoffId,
                            YearCovered = cutoff.YearCovered,
                        };

                        newPayroll.RegularPay = reader.GetDouble(RegularPayIndex);
                        newPayroll.GrossPay = reader.GetDouble(GrossIndex);
                        newPayroll.NetPay = reader.GetDouble(NetPayIndex);

                        newPayroll.RegHours = reader.GetDouble(RegularHoursIndex);
                        newPayroll.Overtime = reader.GetDouble(OvertimeIndex);
                        newPayroll.RestDayOvertime = reader.GetDouble(RestDayOvertimeIndex);
                        newPayroll.HolidayOvertime = reader.GetDouble(HolidayOvertimeIndex);
                        newPayroll.NightDifferential = reader.GetDouble(NightDifferentialIndex);
                        newPayroll.AbsTar = reader.GetDouble(AbsentAndTardyIndex);

                        if (cutoff.CutoffDate.Day != 15)
                        {
                            newPayroll.EmployeeSSS = reader.GetDouble(EmployeeSSSIndex);
                            newPayroll.EmployerSSS = reader.GetDouble(EmployerSSSIndex);
                            newPayroll.EmployeePhilHealth = reader.GetDouble(EmployeePhilHealthIndex);
                            newPayroll.EmployeePagibig = reader.GetDouble(EmployeePagibigIndex);


                            newPayroll.WithholdingTax = reader.GetDouble(WithholdingTaxIndex);
                        }

                        newPayroll.PayrollId = Payroll.GenerateId(newPayroll);
                        newPayroll.UpdateValues();
                        payrolls.Add(newPayroll);
                    } while (reader.Read());
                }
            }

            return payrolls;
        }

        private void FindPayrollDate(IExcelDataReader reader)
        {
            reader.Read();
            reader.Read();
            reader.Read();
            CheckCutoffDate(reader);
            reader.Read();
            CheckCutoffDate(reader);
            reader.Read();
            CheckCutoffDate(reader);
            reader.Read();
        }
        private void CheckCutoffDate(IExcelDataReader reader)
        {
            if (CutoffDate == default)
            {
                string payrollDateRaw = "";
                if (!string.IsNullOrEmpty(reader.GetString(0)))
                    payrollDateRaw = reader.GetString(0).Split(':')[1].Trim();
                else if (reader.GetValue(1) is not null)
                    payrollDateRaw = reader.GetString(1).Trim().Replace("*", "").Trim();

                if (payrollDateRaw != "")
                    CutoffDate = DateTime.ParseExact(payrollDateRaw, "dd MMMM yyyy", CultureInfo.InvariantCulture);
            }
        }

        private static string[] ParseEmployeeDetail(IExcelDataReader reader, int nameIdx)
        {
            if (reader.GetValue(nameIdx) is not null)
            {
                var fullname_raw = reader.GetString(nameIdx).Trim(')').Split('(');
                if (fullname_raw.Length < 2)
                    return Array.Empty<string>();

                return new[] { fullname_raw[0].Trim(), fullname_raw[1].Trim() };
            }
            return Array.Empty<string>();
        }

    }
}
