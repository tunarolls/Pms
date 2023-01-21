using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Common.Enums;
using Pms.Payrolls;
using Pms.Payrolls.ServiceLayer.Files.Import.PayrollRegister;
using Pms.Payrolls.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pms.PayrollsTests.ServiceLayer.Files.Import
{
    [TestClass]
    public class PayRegisterKSImportTests
    {
        [TestMethod]
        public void ShouldImportPayroll()
        {
            IImportPayrollService payregImporter = new PayrollRegisterImportBase(ImportProcessChoices.KS);
            string payregFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\PayRegisterImportTests\K13202208B.XLS";
            List<Payroll> actualPayrolls = payregImporter.StartImport(payregFilePath).ToList();

            Assert.IsNotNull(actualPayrolls);
            Assert.IsTrue(actualPayrolls.Any());
            Assert.IsTrue(actualPayrolls[0].RegHours > 0);
        }

        [TestMethod]
        public void ShouldImportK12Payroll()
        {
            IImportPayrollService payregImporter = new PayrollRegisterImportBase(ImportProcessChoices.KS);
            string payregFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\PayRegisterImportTests\K12202208B.XLS";
            List<Payroll> actualPayrolls = payregImporter.StartImport(payregFilePath).ToList();

            Assert.IsNotNull(actualPayrolls);
            Assert.IsTrue(actualPayrolls.Any());
            Assert.IsTrue(actualPayrolls[0].RegHours > 0);
        }

        [TestMethod]
        public void ShouldImportK9APayroll()
        {
            IImportPayrollService payregImporter = new PayrollRegisterImportBase(ImportProcessChoices.KS);
            string payregFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\PayRegisterImportTests\K9APAYREG.xls";
            List<Payroll> actualPayrolls = payregImporter.StartImport(payregFilePath).ToList();

            Assert.IsNotNull(actualPayrolls);
            Assert.IsTrue(actualPayrolls.Any());
            Assert.IsTrue(actualPayrolls[0].RegHours > 0);
        }
    }
}
