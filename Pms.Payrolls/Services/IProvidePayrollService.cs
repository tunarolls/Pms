using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Payrolls.Services
{
    public interface IProvidePayrollService
    {
        /// <summary>
        /// Used for extracting banks
        /// </summary>
        /// <returns></returns>
        IEnumerable<Payroll> GetAllPayrolls();

        Task<ICollection<Payroll>> GetAllPayrolls(CancellationToken cancellationToken = default);

        Task<ICollection<Payroll>> GetYearlyPayrolls(int year, CancellationToken cancellationToken = default);

        /// <summary>
        /// Used for generating Government Computation.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="payrollCode"></param>
        /// <returns></returns>
        IEnumerable<MonthlyPayroll> GetMonthlyPayrolls(int month, string payrollCode);

        Task<ICollection<MonthlyPayroll>> GetMonthlyPayrolls(int month, string payrollCode, CancellationToken cancellationToken = default);

        IEnumerable<Payroll> GetNoEEPayrolls();

        IEnumerable<Payroll> GetPayrolls(string cutoffId);

        Task<ICollection<Payroll>> GetPayrolls(string cutoffId, CancellationToken cancellationToken = default);

        IEnumerable<Payroll> GetPayrolls(string cutoffId, string payrollCode);

        Task<ICollection<Payroll>> GetPayrolls(string cutoffId, string payrollCode, CancellationToken cancellationToken = default);

        IEnumerable<Payroll> GetPayrolls(int yearsCovered, string companyId);

        Task<ICollection<Payroll>> GetPayrolls(int yearsCovered, string companyId, CancellationToken cancellationToken = default);

        IEnumerable<Payroll> GetPayrollsByCcompany(string cutoffId, string CompanyId);
    }
}
