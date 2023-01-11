using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.Tests;

namespace Pms.Payrolls.Tests
{
    [TestClass]
    public class AlphalistVerifierExporterTests
    {
        private IDbContextFactory<PayrollDbContext> _factory;
        private IProvidePayrollService _payrollProvider;

        public AlphalistVerifierExporterTests()
        {
            _factory = new PayrollDbContextFactoryFixture();
            _payrollProvider = new PayrollProvider(_factory);
        }

        [TestMethod]
        public void ShouldExportAlphaVerifier()
        {
            CompanyView company = new();// { RegisteredName = "TEST COMPANY", MinimumRate = 71.25 };
            int yearCovered = 2022;
            IEnumerable<Payroll> payrolls = _payrollProvider.GetPayrolls(yearCovered, company.CompanyId);
            var employeePayrolls = payrolls.GroupBy(py => py.EEId).Select(py => py.ToList()).ToList();

            AlphalistVerifierExporter exporter = new();
            exporter.StartExport(employeePayrolls,yearCovered,company.CompanyId);
        }
    }
}