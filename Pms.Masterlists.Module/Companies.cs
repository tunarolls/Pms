using Pms.Common;
using Pms.Masterlists.Entities;
using Pms.Masterlists.ServiceLayer.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module
{
    public class Companies
    {
        private readonly CompanyManager _companyManager;

        public Companies(CompanyManager companyManager)
        {
            _companyManager = companyManager;
        }

        public ICollection<Company> ListCompanies()
        {
            return _companyManager.GetAllCompanies().ToList();
        }

        public async Task<ICollection<Company>> ListCompanies(CancellationToken cancellationToken = default)
        {
            return await _companyManager.GetAllCompanies(cancellationToken);
        }

        public void Save(Company company)
        {
            _companyManager.SaveCompany(company);
        }

        public async Task Save(Company company, CancellationToken cancellationToken = default)
        {
            await _companyManager.SaveCompany(company, cancellationToken);
        }
    }
}
