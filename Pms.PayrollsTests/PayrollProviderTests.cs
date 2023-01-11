using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pms.Payrolls.Persistence;
using Microsoft.EntityFrameworkCore;
using Pms.Payrolls.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pms.Payrolls.Tests
{
    [TestClass]
    public class PayrollProviderTests
    {
        private IDbContextFactory<PayrollDbContext> _factory;
        private IProvidePayrollService _payrollProvider;

        public PayrollProviderTests()
        {
            _factory = new PayrollDbContextFactoryFixture();
            _payrollProvider = new PayrollProvider(_factory);
        }
         
        [DataTestMethod]
        [DataRow(2022,"")]
        public void ShouldGetAccuratePayrollsByYearsCovoredAndCompanyId(int yearsCovered, string companyId)
        {
            _payrollProvider.GetPayrolls(yearsCovered, companyId);
        }
    }
}
