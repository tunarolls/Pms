using Microsoft.EntityFrameworkCore;
using Pms.Common;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.EfCore
{
    public class CompanyManager
    {
        private readonly IDbContextFactory<EmployeeDbContext> _factory;
        public CompanyManager(IDbContextFactory<EmployeeDbContext> factory) =>
            _factory = factory;

        public IEnumerable<Company> GetAllCompanies()
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            return (IEnumerable<Company>)Context.Companies.ToList();
        }

        public void SaveCompany(Company company)
        {
            EmployeeDbContext Context = _factory.CreateDbContext();
            if (Context.Companies.Any(pc => pc.CompanyId == company.CompanyId))
                Context.Update(company);
            else
                Context.Add(company);

            Context.SaveChanges();
        }
    }
}
