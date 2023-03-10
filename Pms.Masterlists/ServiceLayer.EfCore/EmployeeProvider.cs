using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Wordprocessing;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Persistence;
using System.Linq;

namespace Pms.Masterlists.ServiceLayer.EfCore
{
    static class EmployeeProviderExtensions
    {
        public static IQueryable<Employee> FilterByPayrollCode(this IQueryable<Employee> employees, string? payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? employees.Where(t => t.PayrollCode == payrollCode)
                : employees;
        }
    }

    public class EmployeeProvider 
    {
        private readonly IDbContextFactory<EmployeeDbContext> _factory;
        public EmployeeProvider(IDbContextFactory<EmployeeDbContext> factory)
        {
            _factory = factory;
        }

        public bool EmployeeExists(string eeId)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            return Context.Employees.Any(ee => ee.EEId == eeId);
        }

        public async Task<bool> EmployeeExists(string eeId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Employees.AnyAsync(t => t.EEId == eeId, cancellationToken);
        }

        public Employee? FindEmployee(string eeId)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            return Context.Employees.Find(eeId);
        }

        public async Task<Employee?> FindEmployee(string? eeId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Employees.FindAsync(new object?[] { eeId }, cancellationToken);
        }

        public IQueryable<Employee> GetEmployees()
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            return Context.Employees.AsNoTracking();
        }

        public async Task<ICollection<Employee>> GetEmployees(CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Employees.ToListAsync(cancellationToken);
        }

        public async Task<ICollection<Employee>> GetEmployees(string? payrollCode, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Employees.FilterByPayrollCode(payrollCode).ToListAsync(cancellationToken);
        }
    }
}



