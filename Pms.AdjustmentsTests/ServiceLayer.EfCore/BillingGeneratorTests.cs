using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Adjustments.Services;
using Pms.AdjustmentsTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.AdjustmentsTests.ServiceLayer.EfCore
{
    [TestClass]
    public class BillingGeneratorTests
    {
        private Cutoff _cutoff;
        private IDbContextFactory<AdjustmentDbContext> _factory;
        private IGenerateBillingService _generateBillingService;
        private IManageBillingService _manageBillingService;

        public BillingGeneratorTests()
        {
            _factory = new AdjustmentDbContextFactoryFixture();
            _manageBillingService = new BillingManager(_factory);
            _generateBillingService = new BillingGenerator(_factory);
            _cutoff = new();
        }


        [TestMethod]
        public void ShouldGenerateBillingsByTimesheetView()
        {
            // GIVEN
            string eeId = "DYYJ";
            string cutoffId = _cutoff.CutoffId;
            using AdjustmentDbContext context = _factory.CreateDbContext();
            int expectedBillingCount = context.Timesheets
                .Where(ts => ts.EEId == eeId)
                .Where(ts => ts.CutoffId == cutoffId)
                .Where(ts => ts.Allowance > 0)
                .Count();

            List<TimesheetView> timesheetsWithPCV = context.Timesheets
                .Where(ts => ts.EEId == eeId)
                .Where(ts => ts.CutoffId == cutoffId)
                .Where(ts => ts.RawPCV != "")
                .ToList();
            foreach (TimesheetView timesheet in timesheetsWithPCV)
                expectedBillingCount += timesheet.RawPCV.Split("|").Length;


            // WHEN
            IEnumerable<Billing> billings = _generateBillingService.GenerateBillingFromTimesheetView(eeId, cutoffId);
            int actualBillingCount = billings.Count();


            // THEN
            Assert.AreEqual(expectedBillingCount, actualBillingCount);
        }
    }
}