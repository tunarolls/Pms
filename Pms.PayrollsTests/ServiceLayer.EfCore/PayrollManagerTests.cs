using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Payrolls;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.ServiceLayer.EfCore;
using Pms.Payrolls.Services;
using Pms.Payrolls.Tests;

namespace Pms.PayrollsTests.ServiceLayer.EfCore
{
    [TestClass]
    public class PayrollManagerTests
    {
        private IDbContextFactory<PayrollDbContext> _factory;
        private IProvidePayrollService _payrollProvider;
        private IManagePayrollService _payrollManager;

        public PayrollManagerTests()
        {
            _factory = new PayrollDbContextFactoryFixture();
            _payrollProvider = new PayrollProvider(_factory);
            _payrollManager = new PayrollManager(_factory);
        }

        [TestMethod]
        public void ShouldSavePayrollSuccessfully()
        {
            Payroll expected = Seeder.GenerateSeedPayroll("DYYJ", "2207-1", "MANILAIDCSI0000", 19000, 20000, 17000, -1000, -1000, -1000);
            _payrollManager.SavePayroll(expected);
            Payroll? actual = _payrollProvider.GetPayrolls(expected.CutoffId).Where(p => p.PayrollId == expected.PayrollId).FirstOrDefault();
            Assert.IsNotNull(actual);
        }
    }
}
