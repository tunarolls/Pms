using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.Files;
using Pms.TimesheetsTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.ServiceLayer.Files.Tests
{
    [TestClass]
    public class TimesheetManagerTests
    {
        private readonly IDbContextFactory<TimesheetDbContext> _factory;
        private readonly TimesheetProvider _providerService;
        private readonly TimesheetManager _service;

        public TimesheetManagerTests()
        {
            _factory = new TimesheetDbContextFactoryFixture();
            _service = new(_factory);
            _providerService = new(_factory);
        }

        [TestMethod]
        public void ShouldSaveTimesheetWhenAdding()
        {
            Timesheet expectedTimesheet = new Timesheet() { TimesheetId = "TEST_2208-1", EEId = "TEST", CutoffId = "2208-1" };
            expectedTimesheet.PCV = new string[,] { { "2022-08-10", "300" }, { "2022-08-11", "475" }, { "2022-08-10", "325" } };

            _service.SaveTimesheet(expectedTimesheet, expectedTimesheet.CutoffId, 0);

            Timesheet? actualTimesheet = _providerService.GetTimesheets().Where(ts => ts.TimesheetId == expectedTimesheet.TimesheetId).FirstOrDefault();

            Assert.IsNotNull(actualTimesheet);

            using var context = _factory.CreateDbContext();
            context.Remove(actualTimesheet);
            context.SaveChanges();
        }
    }
}
