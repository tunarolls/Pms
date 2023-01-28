using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Timesheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.Tests
{
    [TestClass()]
    public class EmployeeViewTests
    {
        [DataRow("Last", " ", " ", "Last")]
        [DataRow("Last, First M.", "First", "Middle", "Last")]
        [DataRow("Last, First", "First", " ", "Last")]
        [DataRow("Last, First", "First", "", "Last")]
        [DataRow("Last", "", "", "Last")]
        [DataRow("First", "First")]
        [DataRow("Last M.", "", "Middle", "Last")]
        [DataRow("First M.", "First", "Middle")]
        [DataRow("Last, First Extension.", "First", "", "Last", "Extension")]
        [DataTestMethod]
        public void GetFullNameTest(string expected, string first = "", string middle = "", string last = "", string extension = "")
        {
            var employee = new EmployeeView()
            {
                FirstName = first,
                MiddleName = middle,
                LastName = last,
                NameExtension = extension
            };

            Assert.AreEqual(expected, employee.FullName);
        }
    }
}