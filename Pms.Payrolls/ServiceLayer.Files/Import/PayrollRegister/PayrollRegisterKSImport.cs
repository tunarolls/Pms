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
        private int NameIndex;

        private int RegularHoursIndex;

        private int GrossPayIndex;

        private int NightDifferentialIndex;

        private int EmployeePagibigIndex;
        private int EmployerPagibigIndex;

        private int EmployeeSSSIndex;
        private int EmployerSSSIndex;

        private int EmployeePhilHealthIndex;

        private int WithholdingTaxIndex;

        private int Adjust1Index;

        private int Adjust2Index;

        private int NetpayIndex;

        private string PayrollRegisterFilePath;

        DateTime CutoffDate { get; set; }

        public void ValidatePayRegisterFile()
        {
            if (CutoffDate == default)
                throw new PayrollRegisterHeaderNotFoundException("Cutoff Date", PayrollRegisterFilePath);
        }


        public IEnumerable<Payroll> StartImport(string payrollRegisterFilePath)
        {
            PayrollRegisterFilePath = payrollRegisterFilePath;

            CutoffDate = default;
            List<Payroll> payrolls = new();
            using (var stream = File.Open(payrollRegisterFilePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    FindCutoffDate(reader);

                    ValidatePayRegisterFile();

                    SetHeaderIndexes();

                    Cutoff cutoff = new Cutoff(CutoffDate);
                    do
                    {
                        string eeId = "";
                        var name_args = ParseEmployeeDetail(reader, NameIndex);
                        if (name_args is null)
                            continue;
                        eeId = name_args[1].Trim();

                        var newPayroll = new Payroll()
                        {
                            EEId = eeId,
                            CutoffId = cutoff.CutoffId,
                            YearCovered = cutoff.YearCovered,
                        };

                        newPayroll.RegHours = reader.GetDouble(RegularHoursIndex);

                        newPayroll.RegularPay = reader.GetDouble(GrossPayIndex);
                        newPayroll.GrossPay = reader.GetDouble(GrossPayIndex);

                        newPayroll.NightDifferential = reader.GetDouble(NightDifferentialIndex);

                        newPayroll.EmployeePagibig = reader.GetDouble(EmployeePagibigIndex);
                        newPayroll.EmployerPagibig = reader.GetDouble(EmployerPagibigIndex);

                        newPayroll.EmployeeSSS = reader.GetDouble(EmployeeSSSIndex);
                        newPayroll.EmployerSSS = reader.GetDouble(EmployerSSSIndex);

                        newPayroll.EmployeePhilHealth = reader.GetDouble(EmployeePhilHealthIndex);

                        newPayroll.WithholdingTax = reader.GetDouble(WithholdingTaxIndex);

                        newPayroll.NetPay = reader.GetDouble(NetpayIndex);
                        newPayroll.PayrollId = Payroll.GenerateId(newPayroll);

                        payrolls.Add(newPayroll);

                    } while (reader.Read());
                }
            }

            return payrolls;
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
        private void SetHeaderIndexes()
        {
            NameIndex = 1;
            RegularHoursIndex = 2;
            GrossPayIndex = 5;
            NightDifferentialIndex = 6;
            EmployeePagibigIndex = 7;
            EmployerPagibigIndex = 8;
            EmployeeSSSIndex = 9;
            EmployerSSSIndex = 10;
            EmployeePhilHealthIndex = 11;
            WithholdingTaxIndex = 12;


            //if (CutoffDate.Day == 15) // there is no adjust 1 column on every 15th cutoff
            //{
            //    Adjust2Index = 13;
            //    NetpayIndex = 14;
            //}
            //else
            NetpayIndex = 15;
        }

        private static string[] ParseEmployeeDetail(IExcelDataReader reader, int nameIdx)
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
    }
}
