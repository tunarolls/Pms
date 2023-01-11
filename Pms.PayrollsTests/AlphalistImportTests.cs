using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pms.Payrolls.Tests
{
    [TestClass]
    public class AlphalistImportTests
    {
        [TestMethod]
        public void ShouldImportAlphalistsToBirProgram()
        {
            CompanyView company = new("INTERNATIONAL DATA CONVERSION SOLUTIONS INC", "214271279", 0, "VII");
            string alphalistFilepath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TESTDATA\AlphalistImportTests\MIDCSI00_2022-Alpha.xls";
            string birDbfFilepath = @"Y:\\DATA";

            AlphalistImport importer = new();
            importer.ImportToBIRProgram(alphalistFilepath, birDbfFilepath, company, 2022);
        }
    }
}
