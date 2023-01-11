using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pms.Payrolls.Tests
{
    [TestClass]
    public class BankReportExportTests
    {
        private IDbContextFactory<PayrollDbContext> _factory;
        private IProvidePayrollService _payrollProvider;

        public BankReportExportTests()
        {
            _factory = new PayrollDbContextFactoryFixture();
            _payrollProvider = new PayrollProvider(_factory);
        }

        [TestMethod]
        public void ShouldExportInMBFormat()
        {
            string cutoffId = "2209-1";
            string payrollCode = "P4A";
            IEnumerable<Payroll> payrolls = _payrollProvider.GetPayrolls(cutoffId, payrollCode);

            BankReportBase exporter = new(cutoffId, payrollCode);
            exporter.StartExport(payrolls.ToArray());
        }

        [TestMethod]
        public void ShouldExportInLbpCbcChkFormat()
        {
            string cutoffId = "2208-1";
            string payrollCode = "P1A";
            IEnumerable<Payroll> payrolls = _payrollProvider.GetPayrolls(cutoffId, payrollCode);

            BankReportBase exporter = new(cutoffId, payrollCode);
            exporter.StartExport(payrolls.ToArray());
        }
    }
}
