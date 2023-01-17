using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pms.Payrolls.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Payrolls.Services;
using Pms.Payrolls.ServiceLayer.Files.Export.Alphalist;
using Pms.Payrolls;
using Pms.Payrolls.Tests;
using Pms.Payrolls.ServiceLayer.EfCore;

namespace Pms.PayrollsTests.ServiceLayer.Files.Export
{
    [TestClass]
    public class AlphalistExportTests
    {
        private IDbContextFactory<PayrollDbContext> _factory;
        private IProvidePayrollService _payrollProvider;

        public AlphalistExportTests()
        {
            _factory = new PayrollDbContextFactoryFixture();
            _payrollProvider = new PayrollProvider(_factory);
        }

        [TestMethod]
        public void ShouldExportAlphalist()
        {
            CompanyView company = new() { CompanyId = "MANILAIDCSI0000", MinimumRate = 71.25 };
            int yearCovered = 2022;
            IEnumerable<Payroll> payrolls = _payrollProvider.GetPayrolls(yearCovered, company.CompanyId);
            var employeePayrolls = payrolls.GroupBy(py => py.EEId).Select(py => py.ToList()).ToList();

            List<AlphalistDetail> alphalists = new();
            foreach (var employeePayroll in employeePayrolls)
            {
                alphalists.Add(new AutomatedAlphalistDetail(employeePayroll, company.MinimumRate, yearCovered).CreateAlphalistDetail());
            }

            Assert.IsTrue(alphalists.Any());
            AlphalistExporter exporter = new();
            exporter.StartExport(alphalists, yearCovered, company.CompanyId, company.MinimumRate);
        }
    }
}
