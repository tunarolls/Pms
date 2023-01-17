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
        private int _absentAndTardyIndex = -1;
        private int _eeIdIndex = -1;
        private int _employeePagibigIndex = -1;
        private int _employeePhilHealthIndex = -1;
        private int _employeeSSSIndex = -1;
        private int _grossIndex = -1;
        private int _holidayOvertimeIndex = -1;
        private int _netPayIndex = -1;
        private int _nightDifferentialIndex = -1;
        private int _overtimeIndex = -1;
        private int _regularHoursIndex = -1;
        private int _regularPayIndex = -1;
        private int _restDayOvertimeIndex = -1;
        private int _withholdingTaxIndex = -1;

        private DateTime CutoffDate { get; set; }
        private string PayrollRegisterFilePath { get; set; } = string.Empty;

        public IEnumerable<Payroll> StartImport(string payrollRegisterFilePath)
        {
            PayrollRegisterFilePath = payrollRegisterFilePath;

            List<Payroll> payrolls = new();
            using (var stream = File.Open(PayrollRegisterFilePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    FindHeaders(reader);
                    FindPayrollDate(reader);

                    ValidatePayRegisterFile();

                    Cutoff cutoff = new Cutoff(CutoffDate);
                    do
                    {
                        string employee_id = "";
                        if (_eeIdIndex > -1)
                        {
                            if (reader.GetString(_eeIdIndex) is null)
                                continue;
                            employee_id = reader.GetString(_eeIdIndex).Trim();
                        }

                        var newPayroll = new Payroll()
                        {
                            EEId = employee_id,
                            CutoffId = cutoff.CutoffId,
                            YearCovered = cutoff.YearCovered,
                        };

                        newPayroll.RegularPay = reader.GetDouble(_regularPayIndex);
                        newPayroll.GrossPay = reader.GetDouble(_grossIndex);
                        newPayroll.NetPay = reader.GetDouble(_netPayIndex);

                        newPayroll.RegHours = reader.GetDouble(_regularHoursIndex);
                        newPayroll.Overtime = reader.GetDouble(_overtimeIndex);
                        newPayroll.RestDayOvertime = reader.GetDouble(_restDayOvertimeIndex);
                        newPayroll.HolidayOvertime = reader.GetDouble(_holidayOvertimeIndex);
                        newPayroll.NightDifferential = reader.GetDouble(_nightDifferentialIndex);
                        newPayroll.AbsTar = reader.GetDouble(_absentAndTardyIndex);

                        if (cutoff.CutoffDate.Day != 15)
                        {
                            newPayroll.EmployeeSSS = reader.GetDouble(_employeeSSSIndex);
                            newPayroll.EmployeePagibig = reader.GetDouble(_employeePagibigIndex);
                            newPayroll.EmployeePhilHealth = reader.GetDouble(_employeePhilHealthIndex);
                            newPayroll.WithholdingTax = reader.GetDouble(_withholdingTaxIndex);
                        }

                        newPayroll.PayrollId = Payroll.GenerateId(newPayroll);
                        newPayroll.UpdateValues();
                        payrolls.Add(newPayroll);
                    } while (reader.Read());
                }
            }

            return payrolls;
        }

        public void ValidatePayRegisterFile()
        {
            if (_eeIdIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Employee ID", PayrollRegisterFilePath);
            if (_regularHoursIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Regular Hours", PayrollRegisterFilePath);
            if (_overtimeIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Overtime", PayrollRegisterFilePath);
            if (_restDayOvertimeIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Restdat Overtime", PayrollRegisterFilePath);
            if (_holidayOvertimeIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Holiday Overtime", PayrollRegisterFilePath);
            if (_nightDifferentialIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Night Differential", PayrollRegisterFilePath);
            if (_absentAndTardyIndex == -1) throw new PayrollRegisterHeaderNotFoundException("AbsTar", PayrollRegisterFilePath);

            if (_regularPayIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Regular Pay", PayrollRegisterFilePath);
            if (_grossIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Gross Pay", PayrollRegisterFilePath);
            if (_netPayIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Net Pay", PayrollRegisterFilePath);

            if (_employeeSSSIndex == -1) throw new PayrollRegisterHeaderNotFoundException("SSS EE", PayrollRegisterFilePath);
            if (_employeePagibigIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Pagibig EE", PayrollRegisterFilePath);
            if (_employeePhilHealthIndex == -1) throw new PayrollRegisterHeaderNotFoundException("PhilHealth EE", PayrollRegisterFilePath);

            if (_withholdingTaxIndex == -1) throw new PayrollRegisterHeaderNotFoundException("Withholding Tax", PayrollRegisterFilePath);

            if (CutoffDate == default) throw new PayrollRegisterHeaderNotFoundException("Cutoff Date", PayrollRegisterFilePath);
        }

        private static string[]? ParseEmployeeDetail(IExcelDataReader reader, int nameIdx)
        {
            if (reader.GetValue(nameIdx) is not null)
            {
                var fullname_raw = reader.GetString(nameIdx).Trim(')').Split('(');
                if (fullname_raw.Length < 2)
                    return null;

                return new[] { fullname_raw[0].Trim(), fullname_raw[1].Trim() };
            }
            return null;
        }

        private void CheckCutoffDate(IExcelDataReader reader)
        {
            if (CutoffDate == default)
            {
                string payrollDateRaw = "";
                if (reader.GetValue(0) is not null)
                    payrollDateRaw = reader.GetString(0).Split(':')[1].Trim();
                else if (reader.GetValue(1) is not null)
                    payrollDateRaw = reader.GetString(1).Trim().Replace("*", "").Trim();

                if (payrollDateRaw != "")
                    CutoffDate = DateTime.ParseExact(payrollDateRaw, "dd MMMM yyyy", CultureInfo.InvariantCulture);
            }
        }

        private void CheckHeaders(IExcelDataReader reader)
        {
            FindHeaderColumnIndex(ref _eeIdIndex, "ID", reader);
            FindHeaderColumnIndex(ref _grossIndex, "GROSS", reader);
            FindHeaderColumnIndex(ref _regularPayIndex, "REG_PAY", reader);
            FindHeaderColumnIndex(ref _netPayIndex, "NET", reader);

            FindHeaderColumnIndex(ref _regularHoursIndex, "REG", reader);
            FindHeaderColumnIndex(ref _overtimeIndex, "R_OT", reader);
            FindHeaderColumnIndex(ref _restDayOvertimeIndex, "RD_OT", reader);
            FindHeaderColumnIndex(ref _holidayOvertimeIndex, "HOL_OT", reader);
            FindHeaderColumnIndex(ref _nightDifferentialIndex, "ND", reader);
            FindHeaderColumnIndex(ref _absentAndTardyIndex, "ABS_TAR", reader);

            FindHeaderColumnIndex(ref _employeeSSSIndex, "SSS_EE", reader);
            FindHeaderColumnIndex(ref _employeePagibigIndex, "ADJUST1", reader);
            FindHeaderColumnIndex(ref _employeePhilHealthIndex, "PHIC_EE", reader);
            FindHeaderColumnIndex(ref _employeePhilHealthIndex, "PHIC", reader);

            FindHeaderColumnIndex(ref _withholdingTaxIndex, "TAX", reader);

        }

        private void FindHeaderColumnIndex(ref int index, string header, IExcelDataReader reader)
        {
            if (index == -1)
            {
                for (int column = 0; column < reader.FieldCount; column++)
                {
                    if (reader.GetValue(column) is not null)
                        if ((reader.GetString(column).Trim().ToUpper() ?? "") == (header ?? ""))
                        {
                            index = column;
                            return;
                        }
                }
                index = -1;
            }
        }

        private void FindHeaders(IExcelDataReader reader)
        {
            reader.Read();
            CheckHeaders(reader);
            reader.Read();
            CheckHeaders(reader);
            reader.Read();
            CheckHeaders(reader);
            reader.Read();
        }
        private void FindPayrollDate(IExcelDataReader reader)
        {
            CheckCutoffDate(reader);
            reader.Read();
            CheckCutoffDate(reader);
            reader.Read();
        }
    }
}
