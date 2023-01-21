using Microsoft.EntityFrameworkCore;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Enums;
using Pms.Masterlists.Exceptions;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.EfCore
{
    public class EmployeeManager
    {
        private readonly IDbContextFactory<EmployeeDbContext> _factory;

        public EmployeeManager(IDbContextFactory<EmployeeDbContext> factory)
        {
            _factory = factory;
        }

        public void Save(Employee employee)
        {
            //if (employee is null)
            //    employee = new() { EEId = employee.EEId };

            employee ??= new();
            employee.ValidateAll();

            EmployeeDbContext Context = _factory.CreateDbContext();
            ValidateDuplicate(employee, Context);
            ValidatePayrollCode(employee, Context);


            AddOrUpdate(employee);
        }

        public async Task Save(Employee employee, CancellationToken cancellationToken = default)
        {
            employee.ValidateAll();
            await ValidateDuplicate(employee, cancellationToken);
            await ValidatePayrollCode(employee, cancellationToken);
            await AddOrUpdate(employee, cancellationToken);
        }

        public void Save(IActive info)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            Employee employee = Context.Employees.Where(ee => ee.EEId == info.EEId).FirstOrDefault()
                ?? new() { EEId = info.EEId };

            //if (employee is null)
            //    employee = new() { EEId = info.EEId };

            employee.EEId = info.EEId;
            employee.Active = info.Active;

            AddOrUpdate(employee);
        }

        /// <summary>
        /// Update Only. Should not insert when employee doesn't exists.
        /// </summary>
        /// <param name="info"></param>
        public void Save(IMasterFileInformation info)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            Employee? employee = Context.Employees.Where(ee => ee.EEId == info.EEId).FirstOrDefault();
            if (employee is null) return;

            employee.JobCode = info.JobCode;

            AddOrUpdate(employee);
        }

        public async Task Save(IMasterFileInformation info, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var employee = await context.Employees.SingleOrDefaultAsync(t => t.EEId == info.EEId, cancellationToken);
            if (employee == null) return;
            employee.JobCode = info.JobCode;

            await AddOrUpdate(employee, cancellationToken);
        }

        public void Save(IGovernmentInformation info)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            Employee employee = Context.Employees.Where(ee => ee.EEId == info.EEId).FirstOrDefault()
                ?? new() { EEId = info.EEId };

            //if (employee is null)
            //    employee = new() { EEId = info.EEId };

            employee.Pagibig = info.Pagibig;
            employee.PhilHealth = info.PhilHealth;
            employee.SSS = info.SSS;
            employee.TIN = info.TIN;

            employee.ValidateGovernmentInformation();

            AddOrUpdate(employee);
        }

        public void Save(IHRMSInformation info)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            Employee employee = Context.Employees.Where(ee => ee.EEId == info.EEId).FirstOrDefault()
                ?? new() {  EEId = info.EEId };

            //if (employee is null)
            //    employee = new() { EEId = info.EEId };

            employee.EEId = info.EEId;
            employee.FirstName = info.FirstName;
            employee.LastName = info.LastName;
            employee.MiddleName = info.MiddleName;
            employee.JobCode = info.JobCode;
            employee.Location = info.Location;

            employee.ValidateGovernmentInformation();
            employee.ValidatePersonalInformation();

            AddOrUpdate(employee);
        }

        public void Save(IEEDataInformation info)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            Employee employee = Context.Employees.Where(ee => ee.EEId == info.EEId).FirstOrDefault()
                ?? new() {  EEId = info.EEId };

            //if (employee is null)
            //    employee = new() { EEId = info.EEId };

            if (!string.IsNullOrEmpty(info.FirstName) && !string.IsNullOrEmpty(info.LastName))
            {
                employee.FirstName = info.FirstName;
                employee.LastName = info.LastName;
                employee.MiddleName = info.MiddleName;
                employee.NameExtension = info.NameExtension;
            }

            employee.BirthDate = info.BirthDate;

            employee.Pagibig = info.Pagibig.Replace("-", string.Empty);
            employee.PhilHealth = info.PhilHealth.Replace("-", string.Empty);
            employee.SSS = info.SSS.Replace("-", string.Empty);
            employee.TIN = info.TIN.Replace("-", string.Empty);

            employee.ValidateGovernmentInformation();
            employee.ValidatePersonalInformation();

            AddOrUpdate(employee);
        }

        public async Task Save(IEEDataInformation info, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var employee = await context.Employees.SingleOrDefaultAsync(t => t.EEId == info.EEId, cancellationToken)
                ?? new Employee() { EEId = info.EEId };

            if (!string.IsNullOrEmpty(info.FirstName) && !string.IsNullOrEmpty(info.LastName))
            {
                employee.FirstName = info.FirstName;
                employee.LastName = info.LastName;
                employee.MiddleName = info.MiddleName;
                employee.NameExtension = info.NameExtension;
            }

            employee.BirthDate = info.BirthDate;

            employee.Pagibig = info.Pagibig.Replace("-", string.Empty);
            employee.PhilHealth = info.PhilHealth.Replace("-", string.Empty);
            employee.SSS = info.SSS.Replace("-", string.Empty);
            employee.TIN = info.TIN.Replace("-", string.Empty);

            employee.ValidateGovernmentInformation();
            employee.ValidatePersonalInformation();

            await AddOrUpdate(employee, cancellationToken);
        }

        public void Save(IBankInformation info)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();

            Employee employee = Context.Employees.Where(ee => ee.EEId == info.EEId).FirstOrDefault()
                ?? new() { EEId = info.EEId };
            
            //if (employee is null)
            //    employee = new() { EEId = info.EEId };

            if (!string.IsNullOrEmpty(info.FirstName) && !string.IsNullOrEmpty(info.LastName))
            {
                employee.FirstName = info.FirstName;
                employee.LastName = info.LastName;
                employee.MiddleName = info.MiddleName;
                employee.NameExtension = info.NameExtension;
            }

            employee.AccountNumber = info.AccountNumber;
            employee.CardNumber = info.CardNumber;
            employee.Bank = info.Bank;
            employee.PayrollCode = info.PayrollCode;

            employee.ValidateBankInformation();

            ValidateDuplicate(employee, Context);
            ValidatePayrollCode(employee, Context);

            AddOrUpdate(employee);
        }

        public async Task Save(IBankInformation info, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var employee = await context.Employees.SingleOrDefaultAsync(t => t.EEId == info.EEId, cancellationToken)
                ?? new Employee() { EEId = info.EEId };

            if (!string.IsNullOrEmpty(info.FirstName) && !string.IsNullOrEmpty(info.LastName))
            {
                employee.FirstName = info.FirstName;
                employee.LastName = info.LastName;
                employee.MiddleName = info.MiddleName;
                employee.NameExtension = info.NameExtension;
            }

            employee.AccountNumber = info.AccountNumber;
            employee.CardNumber = info.CardNumber;
            employee.Bank = info.Bank;
            employee.PayrollCode = info.PayrollCode;

            employee.ValidateBankInformation();

            await ValidateDuplicate(employee, cancellationToken);
            await ValidatePayrollCode(employee, cancellationToken);
            await AddOrUpdate(employee, cancellationToken);
        }

        private void ValidateDuplicate(Employee employee, EmployeeDbContext Context)
        {
            Employee? hasDuplicateAccountNumber = null;
            Employee? hasDuplicateCardNumber = null;

            if (employee.Bank == BankChoices.LBP)
            {
                hasDuplicateAccountNumber = Context.Employees.Where(ee => ee.EEId != employee.EEId && ee.AccountNumber == employee.AccountNumber).FirstOrDefault();
                hasDuplicateCardNumber = Context.Employees.Where(ee => ee.EEId != employee.EEId && ee.CardNumber == employee.CardNumber).FirstOrDefault();
            }
            else if (employee.Bank != BankChoices.CHK)
                hasDuplicateAccountNumber = Context.Employees.Where(ee => ee.EEId != employee.EEId && ee.AccountNumber == employee.AccountNumber).FirstOrDefault();


            if (hasDuplicateAccountNumber is not null)
                throw new DuplicateBankInformationException(employee.EEId, hasDuplicateAccountNumber.EEId, "Account Number", hasDuplicateAccountNumber.AccountNumber);

            if (hasDuplicateCardNumber is not null)
                throw new DuplicateBankInformationException(employee.EEId, hasDuplicateCardNumber.EEId, "Card Number", hasDuplicateCardNumber.CardNumber);

        }

        private async Task ValidateDuplicate(Employee employee, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var duplicateAccountNumber = await context.Employees.Where(t => t.EEId != employee.EEId && t.AccountNumber == employee.AccountNumber).ToListAsync(cancellationToken);
            var duplicateCardNumber = await context.Employees.Where(t => t.EEId != employee.EEId && t.CardNumber == employee.CardNumber).ToListAsync(cancellationToken);
            var hasDuplicateAccountNumber = employee.Bank == BankChoices.LBP || employee.Bank != BankChoices.CHK && duplicateAccountNumber.Any();
            var hasDuplicateCardNumber = employee.Bank == BankChoices.LBP && duplicateCardNumber.Any();

            if (hasDuplicateAccountNumber)
            {
                throw new DuplicateBankInformationException(employee.EEId, duplicateCardNumber.First().EEId, "Account Number", duplicateAccountNumber.First().AccountNumber);
            }

            if (hasDuplicateCardNumber)
            {
                throw new DuplicateBankInformationException(employee.EEId, duplicateCardNumber.First().EEId, "Card Number", duplicateCardNumber.First().CardNumber);
            }
        }

        private void ValidatePayrollCode(Employee employee, EmployeeDbContext Context)
        {
            if (Context.PayrollCodes.Where(pc => pc.PayrollCodeId == employee.PayrollCode).FirstOrDefault() is PayrollCode payrollCode)
            {
                employee.CompanyId = payrollCode.CompanyId;
                employee.Site = payrollCode.Site;
            }
            else
                throw new InvalidFieldValueException("Payroll Code", employee.PayrollCode, employee.EEId, "Unknown Payroll code.");

        }

        private async Task ValidatePayrollCode(Employee employee, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            var payrollCode = await context.PayrollCodes.Where(t => t.PayrollCodeId == employee.PayrollCode).FirstOrDefaultAsync(cancellationToken);

            if (payrollCode != null)
            {
                employee.CompanyId = payrollCode.CompanyId;
                employee.Site = payrollCode.Site;
            }
            else
            {
                throw new InvalidFieldValueException("Payroll Code", employee.PayrollCode, employee.EEId, "Unknown payroll code.");
            }
        }

        private void AddOrUpdate(Employee employee)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            if (Context.Employees.Any(ee => ee.EEId == employee.EEId))
                Context.Update(employee);
            else
                Context.Add(employee);
            Context.SaveChanges();
        }

        private async Task AddOrUpdate(Employee employee, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (await context.Employees.AnyAsync(t => t.EEId == employee.EEId, cancellationToken))
                {
                    context.Update(employee);
                }
                else
                {
                    await context.AddAsync(employee, cancellationToken);
                }

                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
