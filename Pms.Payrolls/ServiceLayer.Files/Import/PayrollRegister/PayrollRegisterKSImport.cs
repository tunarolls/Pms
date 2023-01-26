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
        private int _employeePagibigIndex;
        private int _employeePhilHealthIndex;
        private int _employeeSSSIndex;
        private int _employerPagibigIndex;
        private int _employerSSSIndex;
        private int _grossPayIndex;
        private int _nameIndex;

        private int _netpayIndex;
        private int _nightDifferentialIndex;
        //private int Adjust2Index;
        private string _payrollRegisterFilePath = string.Empty;

        private int _regularHoursIndex;
        private int _withholdingTaxIndex;

        //private int Adjust1Index;
        DateTime CutoffDate { get; set; }

        public IEnumerable<Payroll> StartImport(string payrollRegisterFilePath)
        {
            var payrolls = new List<Payroll>();
            using var stream = File.Open(payrollRegisterFilePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            _payrollRegisterFilePath = payrollRegisterFilePath;
            CutoffDate = default;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            FindCutoffDate(reader);
            ValidatePayRegisterFile();
            SetHeaderIndexes();

            var cutoff = new Cutoff(CutoffDate);

            do
            {
                string eeId = string.Empty;
                var nameArgs = ParseEmployeeDetail(reader, _nameIndex);

                if (nameArgs.Length == 0) continue;

                eeId = nameArgs[1].Trim();

                var newPayroll = new Payroll
                {
                    EEId = eeId,
                    CutoffId = cutoff.CutoffId,
                    YearCovered = cutoff.YearCovered,
                    RegHours = reader.GetDouble(_regularHoursIndex),
                    RegularPay = reader.GetDouble(_grossPayIndex),
                    GrossPay = reader.GetDouble(_grossPayIndex),
                    NightDifferential = reader.GetDouble(_nightDifferentialIndex),
                    EmployeePagibig = reader.GetDouble(_employeePagibigIndex),
                    EmployerPagibig = reader.GetDouble(_employerPagibigIndex),
                    EmployeeSSS = reader.GetDouble(_employeeSSSIndex),
                    EmployerSSS = reader.GetDouble(_employerSSSIndex),
                    EmployeePhilHealth = reader.GetDouble(_employeePhilHealthIndex),
                    WithholdingTax = reader.GetDouble(_withholdingTaxIndex),
                    NetPay = reader.GetDouble(_netpayIndex)
                };

                newPayroll.PayrollId = Payroll.GenerateId(newPayroll);

                payrolls.Add(newPayroll);

            } while (reader.Read());

            return payrolls;
        }

        public void ValidatePayRegisterFile()
        {
            if (CutoffDate == default)
            {
                throw new PayrollRegisterHeaderNotFoundException("Cutoff Date", _payrollRegisterFilePath);
            }
        }

        private static string[] ParseEmployeeDetail(IExcelDataReader reader, int nameIdx)
        {
            if (reader.GetValue(nameIdx) != null)
            {
                var fullnameRaw = reader.GetString(nameIdx).Trim(')').Split('(');

                if (fullnameRaw.Length < 2)
                {
                    return Array.Empty<string>();
                }

                return new[] { fullnameRaw[0].Trim(), fullnameRaw[1].Trim() };
            }

            return Array.Empty<string>();
        }

        private void CheckCutoffDate(IExcelDataReader reader)
        {
            if (CutoffDate == default)
            {
                string payrollDateRaw = string.Empty;

                if (reader.GetValue(0) != null)
                {
                    payrollDateRaw = reader.GetString(0).Split(':')[1].Trim();
                }
                else if (reader.GetValue(1) != null)
                {
                    payrollDateRaw = reader.GetString(1).Trim().Replace("*", "").Trim();
                }

                if (payrollDateRaw != string.Empty)
                {
                    CutoffDate = DateTime.ParseExact(payrollDateRaw, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                }
            }
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

        private void SetHeaderIndexes()
        {
            _nameIndex = 1;
            _regularHoursIndex = 2;
            _grossPayIndex = 5;
            _nightDifferentialIndex = 6;
            _employeePagibigIndex = 7;
            _employerPagibigIndex = 8;
            _employeeSSSIndex = 9;
            _employerSSSIndex = 10;
            _employeePhilHealthIndex = 11;
            _withholdingTaxIndex = 12;


            //if (CutoffDate.Day == 15) // there is no adjust 1 column on every 15th cutoff
            //{
            //    Adjust2Index = 13;
            //    NetpayIndex = 14;
            //}
            //else
            _netpayIndex = 15;
        }
    }
}
