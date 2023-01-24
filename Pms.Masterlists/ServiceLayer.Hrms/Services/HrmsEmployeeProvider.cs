using Pms.Masterlists.Entities;
using Pms.Masterlists.ServiceLayer.Hrms.Adapter;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.Hrms.Services
{
    public class HrmsEmployeeProvider
    {
        private readonly HRMSAdapter _hrmsAdapter;
        public HrmsEmployeeProvider(HRMSAdapter hrmsAdapter)
        {
            _hrmsAdapter = hrmsAdapter;
        }

        public async Task<Employee?> GetEmployeeAsync(string EEId, string site)
        {
            if (_hrmsAdapter is not null)
            {
                Employee? employee = await _hrmsAdapter.GetEmployeeFromHRMS<Employee>(EEId, site);
                if (employee is not null)
                {
                    employee.PayrollCode = ParsePayrollCode(employee.PayrollCode, site);
                    return employee;
                }
                return null;
            }
            throw new Exception("HRMS Service is not set.");
        }

        public async Task<Employee?> GetEmployee(string eeId, string site, CancellationToken cancellationToken = default)
        {
            var employee = await _hrmsAdapter.GetEmployeeFromHRMS<Employee>(eeId, site, cancellationToken);
            if (employee != null)
            {
                employee.PayrollCode = ParsePayrollCode(employee.PayrollCode, site);
            }
            return employee;
        }

        public async Task<IEnumerable<Employee>?> GetNewlyHiredEmployeesAsync(DateTime fromDate, string site)
        {
            if (_hrmsAdapter is not null)
            {
                IEnumerable<Employee>? employees = await _hrmsAdapter.GetNewlyHiredEmployeesFromHRMS<Employee>(fromDate, site); ;
                if (employees is not null)
                {
                    foreach (Employee employee in employees)
                        employee.PayrollCode = ParsePayrollCode(employee.PayrollCode, site);
                    return employees;
                }
                return null;
            }
            throw new Exception("HRMS Service is not set.");
        }

        public async Task<ICollection<Employee>> GetNewlyHiredEmployees(DateTime fromDate, string site, CancellationToken cancellationToken = default)
        {
            var employees = await _hrmsAdapter.GetNewlyHiredEmployeesFromHRMS<Employee>(fromDate, site, cancellationToken);

            foreach (var employee in employees)
            {
                employee.PayrollCode = ParsePayrollCode(employee.PayrollCode, site);
            }

            return employees;
        }

        public async Task<IEnumerable<Employee>?> GetResignedEmployeesAsync(DateTime fromDate, string site)
        {
            if (_hrmsAdapter is not null)
            {
                var employees = await _hrmsAdapter.GetResignedEmployeesFromHRMS<Employee>(fromDate, site);
                if (employees is not null)
                {
                    foreach (Employee employee in employees)
                        employee.PayrollCode = ParsePayrollCode(employee.PayrollCode, site);
                    return employees;
                }
                return null;
            }
            throw new Exception("HRMS Service is not set.");
        }

        public async Task<ICollection<Employee>> GetResignedEmployees(DateTime fromDate, string site, CancellationToken cancellationToken = default)
        {
            var employees = await _hrmsAdapter.GetResignedEmployeesFromHRMS<Employee>(fromDate, site, cancellationToken);

            foreach (var employee in employees)
            {
                employee.PayrollCode = ParsePayrollCode(employee.PayrollCode, site);
            }

            return employees;
        }

        private static string ParseBankCategory(string payrollCode, string bankCategory)
        {
            if (payrollCode is not null)
            {
                if (payrollCode.Contains("ATM2") || bankCategory.Contains("ATM2")) { return "ATM2"; }
                if (payrollCode.Contains("ATM") || bankCategory.Contains("ATM")) { return "ATM1"; }
                if (payrollCode.Contains("CHK") || payrollCode.Contains("NO BANK") || payrollCode.Contains("CHEQUE")) { return "CHK"; }
                if (payrollCode.Contains("CASHCARD") || payrollCode.Contains("CCARD")) { return "CCARD"; }
            }
            else payrollCode = "";

            if (bankCategory is not null)
            {
                string bankCat = $"{payrollCode} {bankCategory}";
                bankCat = Regex.Replace(bankCat, "(CASHCARD)", "CCARD");
                bankCat = Regex.Replace(bankCat, "(CHECK|CHEQUE|NO BANK)", "CHK");

                return Regex.Match(bankCat, "(CHK|ATM1|ATM2|CCARD)").Value;
            }

            return "CHK";
        }

        private static string ParsePayrollCode(string payrollCode, string site)
        {
            if (!string.IsNullOrEmpty(payrollCode))
            {
                string pCode = payrollCode.Split('-')[0].Replace("PAY", "P").Trim();
                pCode = $"{site[0]}-{Regex.Match(pCode, "([BLKP]{1,2}[0-9]{1,2}A?)").Value}";

                return pCode;
            }
            return string.Empty;
        }
    }
}
