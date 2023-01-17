using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Payrolls;
using Pms.Payrolls.ServiceLayer.Files.Import.PayrollRegister;
using Pms.Payrolls.Services;

namespace Pms.PayrollsTests.ServiceLayer.Files.Import
{
    [TestClass]
    public class PayRegisterPDImportTests
    {
        [TestMethod]
        public void ShouldImportOldPayroll()
        {
            IImportPayrollService payregImporter = new PayrollRegisterImportBase(ImportProcessChoices.PD);
            string payregFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\PayRegisterImportTests\P1A202201B.xls";
            List<Payroll> actualPayrolls = payregImporter.StartImport(payregFilePath).ToList();

            Assert.IsNotNull(actualPayrolls);
            Assert.IsFalse(actualPayrolls.Any());
            Assert.IsTrue(actualPayrolls[0].RegHours > 0);
        }

        [TestMethod]
        public void ShouldImportPayroll()
        {
            IImportPayrollService payregImporter = new PayrollRegisterImportBase(ImportProcessChoices.PD);
            string payregFilePath = $@"{ AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\PayRegisterImportTests\P7A201912A.xls";
            List<Payroll> actualPayrolls = payregImporter.StartImport(payregFilePath).ToList();

            Assert.IsNotNull(actualPayrolls);
            Assert.IsFalse(actualPayrolls.Any());
            Assert.IsTrue(actualPayrolls[0].RegHours > 0);
        }
    }
}
