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
    public class PayrollRegisterKSImport : IImportPayrollService
    {
        private int _employeePagibigIndex = 7;
        private int _employeePhilHealthIndex = 12;
        private int _employeeSSSIndex = 9;
        private int _grossPayIndex = 5;
        private int _nameIndex = 1;
        private int _netpayIndex = 14;
        private int _nightDifferentialIndex = 6;
        private string _payrollRegisterFilePath = string.Empty;
        private int _regularHoursIndex = 2;
        private int _withholdingTaxIndex = 11;

        DateTime CutoffDate { get; set; }

        public IEnumerable<Payroll> StartImport(string payrollRegisterFilePath)
        {
            _payrollRegisterFilePath = payrollRegisterFilePath;

            CutoffDate = default;
            List<Payroll> payrolls = new();
            using (var stream = File.Open(payrollRegisterFilePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    //FindHeaders(reader);
                    FindCutoffDate(reader);

                    ValidatePayRegisterFile();

                    Cutoff cutoff = new Cutoff(CutoffDate);
                    do
                    {
                        string eeId = "";
                        var name_args = ParseEmployeeDetail(reader, 1);
                        if (name_args is null)
                            continue;
                        eeId = name_args[1].Trim();

                        var newPayroll = new Payroll()
                        {
                            EEId = eeId,
                            CutoffId = cutoff.CutoffId,
                            YearCovered = cutoff.YearCovered,
                        };

                        newPayroll.RegHours = reader.GetDouble(_regularHoursIndex);

                        newPayroll.RegularPay = reader.GetDouble(_grossPayIndex);
                        newPayroll.GrossPay = reader.GetDouble(_grossPayIndex);

                        newPayroll.NightDifferential = reader.GetDouble(_nightDifferentialIndex);

                        newPayroll.EmployeePagibig = reader.GetDouble(_employeePagibigIndex);
                        newPayroll.EmployeeSSS = reader.GetDouble(_employeeSSSIndex);
                        newPayroll.EmployeePhilHealth = reader.GetDouble(_employeePhilHealthIndex);

                        newPayroll.WithholdingTax = reader.GetDouble(_withholdingTaxIndex);

                        newPayroll.NetPay = reader.GetDouble(_netpayIndex);
                        newPayroll.PayrollId = Payroll.GenerateId(newPayroll);


                        payrolls.Add(newPayroll);
                    } while (reader.Read());
                }
            }

            return payrolls;
        }

        public void ValidatePayRegisterFile()
        {
            if (CutoffDate == default) throw new PayrollRegisterHeaderNotFoundException("Cutoff Date", _payrollRegisterFilePath);
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
            FindHeaderColumnIndex(ref _employeeSSSIndex, "SSS_EE", reader);
            FindHeaderColumnIndex(ref _employeePagibigIndex, "ADJUST1", reader);
            FindHeaderColumnIndex(ref _employeePhilHealthIndex, "PHIC_EE", reader);
            FindHeaderColumnIndex(ref _employeePhilHealthIndex, "PHIC", reader);

            FindHeaderColumnIndex(ref _nameIndex, "NAME", reader);
            FindHeaderColumnIndex(ref _regularHoursIndex, "HRS", reader);
            FindHeaderColumnIndex(ref _grossPayIndex, "GROSS", reader);
            FindHeaderColumnIndex(ref _netpayIndex, "NETPAY", reader);
            FindHeaderColumnIndex(ref _netpayIndex, "NET", reader);
        }

        private void FindCutoffDate(IExcelDataReader reader)
        {
            reader.Read();
            reader.Read();
            reader.Read();
            CheckCutoffDate(reader);
            reader.Read();
            CheckCutoffDate(reader);
            reader.Read();
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
    }
}
