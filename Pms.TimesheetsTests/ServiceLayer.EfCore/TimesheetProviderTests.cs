using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.TimesheetsTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.ServiceLayer.EfCore.Tests
{
    [TestClass]
    public class TimesheetProviderTests
    {
        private readonly IDbContextFactory<TimesheetDbContext> _factory;
        private readonly TimesheetManager _service;
        private readonly TimesheetProvider _providerService;

        public TimesheetProviderTests()
        {
            _factory = new TimesheetDbContextFactoryFixture();
            _service = new(_factory);
            _providerService = new(_factory);
        }

        [TestMethod]
        public void ShouldGetTimesheetsWithNoEE()
        {
            List<string> expectedEEIds = _providerService.GetTimesheetNoEETimesheet("2207-1")
                 .Select(ts => ts.EEId)
                 .ToList();

            Assert.IsTrue(expectedEEIds.Any());
        }
    }
}