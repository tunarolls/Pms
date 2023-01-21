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
    [TestClass()]
    public class EmployeeEEDataImporterTests
    {
        [TestMethod()]
        public void StartImportTest()
        {
            Assert.Fail();
        }
    }

    //[TestClass]
    //public class An_extracted_employees_from_EE_DATA_import_can_be_saved
    //{
    //    [TestMethod]
    //    public void IfThisDoesNotThrowAnException()
    //    {
    //        EmployeeEEDataImporter importer = new();

    //        string filename = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\EE DATA 2209.xls";
    //        IEnumerable<IEEDataInformation> actualBankInformations = importer.StartImport(filename);

    //        foreach (Employee actualBankInformation in actualBankInformations)
    //            actualBankInformation.ValidateBankInformation();

    //        Assert.IsNotNull(actualBankInformations);
    //        Assert.IsTrue(!actualBankInformations.Any());
    //    }
    //}
    //public class An_extracted_employees_from_L_EE_DATA_import_can_be_saved
    //{
    //    [TestMethod]
    //    public void IfThisDoesNotThrowAnException()
    //    {
    //        EmployeeEEDataImporter importer = new();

    //        string filename = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\L EE DATA 2209.xls";
    //        IEnumerable<IEEDataInformation> actualBankInformations = importer.StartImport(filename);

    //        foreach (Employee actualBankInformation in actualBankInformations)
    //            actualBankInformation.ValidateBankInformation();

    //        Assert.IsNotNull(actualBankInformations);
    //        Assert.IsTrue(!actualBankInformations.Any());

    //    }
    //}
}