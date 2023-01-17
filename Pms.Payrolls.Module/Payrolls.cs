using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Module
{
    public class Payrolls
    {
        private readonly IManagePayrollService _manager;
        private readonly IProvidePayrollService _provider;

        public Payrolls(IManagePayrollService manager, IProvidePayrollService provider)
        {
            _manager = manager;
            _provider = provider;
        }

        public IEnumerable<MonthlyPayroll> GetMonthlyPayrolls(int month, string payrollCode)
        {
            IEnumerable<MonthlyPayroll> payrolls = _provider.GetMonthlyPayrolls(month, payrollCode);

            return payrolls;
        }


        public IEnumerable<Payroll> Get(string cutoffId) =>
            _provider.GetPayrolls(cutoffId);

        public IEnumerable<Payroll> Get(string cutoffId, string payrollCode) =>
            _provider.GetPayrolls(cutoffId, payrollCode);

        public IEnumerable<Payroll> Get(int yearCovered, string companyId) =>
            _provider.GetPayrolls(yearCovered, companyId);

        public IEnumerable<Payroll> GetByCompanyId(string cutoffId, string companyId) =>
            _provider.GetPayrollsByCcompany(cutoffId, companyId);

        public IEnumerable<IEnumerable<Payroll>> GetYearlyPayrollsByEmployee(int yearCovered, string companyId) =>
            _provider
                .GetPayrolls(yearCovered, companyId)
                .GroupBy(py => py.EEId)
                .Select(py =>
                    py.ToList()
                )
                .ToList();

        /// <summary>
        /// Used specifically for 13th month
        /// </summary>
        /// <param name="yearCovered"></param>
        /// <param name="payrollCodeId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<Payroll>> GetYearlyPayrollsByEmployee(int yearCovered, string payrollCodeId, string companyId)
        {
            var da = _provider.GetAllPayrolls()
                .Where(p => p.YearCovered == yearCovered)
                .Where(p => p.EE is not null).ToList();

            IEnumerable<IGrouping<string, Payroll>>? da1 = da.GroupBy(py => py.EEId);
            var da2 = da1.Where(p => p.Any(p => p.Cutoff.CutoffDate.Month == 11)).ToList();
            var da3 = da2.Select(py => py.Where(p => p.PayrollCode == payrollCodeId).ToList()).ToList();
            var da4 = da3.Where(p => p.Count() > 0).ToList();
            return da4.ToList();
        }



        public string[] ListCutoffIds() =>
            _provider.GetAllPayrolls().ExtractCutoffIds().ToArray();

        public string[] ListPayrollCodes() =>
            _provider.GetAllPayrolls().ExtractPayrollCodes().ToArray();







        public IEnumerable<Payroll> Import(string payregFilePath, ImportProcessChoices processType)
        {
            PayrollRegisterImportBase importer = new(processType);
            return importer.StartImport(payregFilePath);
        }









        public void ExportBankReport(IEnumerable<Payroll> payrolls, string cutoffId, string payrollCode)
        {
            BankReportBase exporter = new(cutoffId, payrollCode);
            exporter.StartExport(payrolls.Where(p => p.NetPay > 0.01));
        }











        public void ExportAlphalist(IEnumerable<AlphalistDetail> alphalists, int year, Company company)
        {
            AlphalistExporter exporter = new();
            exporter.StartExport(alphalists, year, company.CompanyId, company.MinimumRate);
        }

        public void ExportAlphalistVerifier(IEnumerable<IEnumerable<Payroll>> employeePayrolls, int year, Company company)
        {
            AlphalistVerifierExporter exporter = new();
            exporter.StartExport(employeePayrolls, year, company.CompanyId);
        }






        public void ExportMacro(IEnumerable<Payroll> payrolls, Cutoff cutoff, string companyId)
        {
            BenefitsMacroExporter exporter = new(cutoff, companyId);
            exporter.StartExport(payrolls);
        }
        public void ExportMacroB(IEnumerable<Payroll> payrolls, Cutoff cutoff, string companyId)
        {
            BenefitsBMacroExporter exporter = new(cutoff, companyId);
            exporter.StartExport(payrolls);
        }







        public IEnumerable<string> ListNoEEPayrolls() =>
            _provider.GetNoEEPayrolls().ExtractEEIds();

        public void Save(Payroll payroll, string payrollCode, string companyId)
        {
            payroll.PayrollCode = payrollCode;
            payroll.CompanyId = companyId;
            _manager.SavePayroll(payroll);
        }
    }
}
