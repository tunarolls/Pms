using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Masterlists.Entities;
using Pms.Masterlists.ServiceLayer.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.Files.Tests
{
    [TestClass]
    public class EmployeeBankInformationImporterTests
    {
        [TestMethod]
        public void StartImportTest()
        {
            Assert.Fail();
        }
    }

    [TestClass]
    public class An_extracted_employees_from_bank_information_import_can_be_saved
    {
        [TestMethod]
        public void if_this_does_not_throw_an_exception()
        {
            EmployeeBankInformationImporter importer = new();

            string filename = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\BANK INFORMATION SAMPLE 2209.xls";
            IEnumerable<IBankInformation> actualBankInformations = importer.StartImport(filename);

            foreach (Employee actualBankInformation in actualBankInformations)
                actualBankInformation.ValidateBankInformation();


            Assert.IsNotNull(actualBankInformations);
            Assert.IsTrue(!actualBankInformations.Any());
        }
    }
}