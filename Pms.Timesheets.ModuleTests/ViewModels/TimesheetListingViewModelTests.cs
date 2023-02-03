using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Timesheets.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.Module.ViewModels.Tests
{
    [TestClass()]
    public class TimesheetListingViewModelTests
    {
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("cutoffId", "payrollCode")]
        [DataTestMethod]
        public void GetTimesheetExportDirectoryTest(string cutoffId, string payrollCode)
        {
            var directory = TimesheetListingViewModel.GetTimesheetExportDirectory(cutoffId, payrollCode);
            Console.WriteLine(directory);
        }
    }
}