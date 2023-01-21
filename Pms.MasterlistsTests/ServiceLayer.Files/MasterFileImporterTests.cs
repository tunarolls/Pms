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
    public class MasterFileImporterTests
    {
        [TestMethod()]
        public void StartImportTest()
        {
            Assert.Fail();
        }
    }

    //[TestClass]
    //public class MasterFileImporterIsValid
    //{
    //    [TestMethod]
    //    public void IfItDoesNotThrowException()
    //    {
    //        MasterFileImporter importer = new();

    //        string filename = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\MASTER FILE.xls";
    //        IEnumerable<IMasterFileInformation> masterFileInformations = importer.StartImport(filename);



    //        Assert.IsNotNull(masterFileInformations);
    //        Assert.IsTrue(!masterFileInformations.Any());
    //    }
    //}
}