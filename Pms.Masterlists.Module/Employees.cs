using Microsoft.Extensions.Primitives;
using Pms.Common;
using Pms.Masterlists.Entities;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Files;
using Pms.Masterlists.ServiceLayer.Hrms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module
{
    public class Employees
    {
        private readonly HrmsEmployeeProvider s_Hrms;
        private readonly EmployeeManager s_Manager;
        private readonly EmployeeProvider s_Provider;
        public Employees(EmployeeProvider provider, EmployeeManager manager, HrmsEmployeeProvider hrms)
        {
            s_Provider = provider;
            s_Manager = manager;
            s_Hrms = hrms;
        }

        public bool Exists(string eeId)
        {
            return s_Provider.EmployeeExists(eeId);
        }

        public void ExportMasterlist(IEnumerable<Employee> employees, PayrollCode payrollCode, string remarks = "")
        {
            MasterlistExporter.StartExport(employees, payrollCode, remarks);
        }

        public async Task ExportMasterlist(IEnumerable<Employee> employees, PayrollCode payrollCode, string remarks = "", CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                MasterlistExporter.StartExport(employees, payrollCode, remarks);
            }, cancellationToken);
        }

        public Employee FindEmployee(string eeId)
        {
            return s_Provider.FindEmployee(eeId);
        }

        public async Task<Employee?> FindEmployee(string eeId, CancellationToken cancellationToken = default)
        {
            return await s_Provider.FindEmployee(eeId, cancellationToken);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return s_Provider.GetEmployees();
        }

        public async Task<ICollection<Employee>> GetEmployees(CancellationToken cancellationToken = default)
        {
            return await s_Provider.GetEmployees(cancellationToken);
        }

        public IEnumerable<IBankInformation> ImportBankInformation(string payRegisterPath)
        {
            return EmployeeBankInformationImporter.StartImport(payRegisterPath);
        }

        public async Task<ICollection<IBankInformation>> ImportBankInformation(string payRegisterPath, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return EmployeeBankInformationImporter.StartImport(payRegisterPath).ToList();
            }, cancellationToken);
        }

        public IEnumerable<IEEDataInformation> ImportEEData(string eeDataPath)
        {
            return EmployeeEEDataImporter.StartImport(eeDataPath);
        }

        public async Task<ICollection<IEEDataInformation>> ImportEEData(string eeDataPath, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return EmployeeEEDataImporter.StartImport(eeDataPath);
            }, cancellationToken);
        }

        public IEnumerable<IMasterFileInformation> ImportMasterFile(string payRegisterPath)
        {
            return MasterFileImporter.StartImport(payRegisterPath);
        }

        public async Task<ICollection<IMasterFileInformation>> ImportMasterFile(string payRegisterPath, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return MasterFileImporter.StartImport(payRegisterPath).ToList();
            }, cancellationToken);
        }

        public void ReportExceptions(IEnumerable<Exception> exceptions, PayrollCode payrollCode, string suffix)
        {
            if (exceptions.Any())
            {
                InvalidValueReporter.StartReport(exceptions, payrollCode, suffix);
            }
        }

        public void Save(Employee employee)
        {
            s_Manager.Save(employee);
        }

        public async Task Save(Employee employee, CancellationToken cancellationToken = default)
        {
            await s_Manager.Save(employee, cancellationToken);
        }

        public void Save(IActive employee)
        {
            s_Manager.Save(employee);
        }

        public void Save(IMasterFileInformation employee)
        {
            s_Manager.Save(employee);
        }

        public async Task Save(IMasterFileInformation employee, CancellationToken cancellationToken = default)
        {
            await s_Manager.Save(employee, cancellationToken);
        }

        public void Save(IHRMSInformation employee)
        {
            s_Manager.Save(employee);
        }

        public void Save(IBankInformation employee)
        {
            s_Manager.Save(employee);
        }

        public async Task Save(IBankInformation employee, CancellationToken cancellationToken = default)
        {
            await s_Manager.Save(employee, cancellationToken);
        }

        public void Save(IGovernmentInformation employee)
        {
            s_Manager.Save(employee);
        }

        public void Save(IEEDataInformation employee)
        {
            s_Manager.Save(employee);
        }

        public async Task Save(IEEDataInformation employee, CancellationToken cancellationToken = default)
        {
            await s_Manager.Save(employee, cancellationToken);
        }

        public async Task<IEnumerable<Employee>> SyncNewlyHiredAsync(DateTime fromDate, string site)
        {
            IEnumerable<Employee>? result = await s_Hrms.GetNewlyHiredEmployeesAsync(fromDate, site);
            if (result is not null)
                return result;

            return Enumerable.Empty<Employee>();
        }

        public async Task<ICollection<Employee>> SyncNewlyHired(DateTime fromDate, string site, CancellationToken cancellationToken = default)
        {
            return await s_Hrms.GetNewlyHiredEmployees(fromDate, site, cancellationToken);
        }

        public async Task<Employee> SyncOneAsync(string eeId, string site)
        {
            return await s_Hrms.GetEmployeeAsync(eeId, site);
        }

        public async Task<Employee?> SyncOne(string eeId, string site, CancellationToken cancellationToken = default)
        {
            return await s_Hrms.GetEmployee(eeId, site, cancellationToken);
        }

        public async Task<IEnumerable<Employee>> SyncResignedAsync(DateTime fromDate, string site)
        {
            IEnumerable<Employee>? result = await s_Hrms.GetResignedEmployeesAsync(fromDate, site);
            if (result is not null)
                return result;

            return Enumerable.Empty<Employee>();
        }

        public async Task<ICollection<Employee>> SyncResigned(DateTime fromDate, string site, CancellationToken cancellationToken = default)
        {
            return await s_Hrms.GetResignedEmployees(fromDate, site, cancellationToken);
        }
    }
}
