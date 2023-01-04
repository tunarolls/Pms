using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Services
{
    public class BillingGenerator : IGenerateBillingService
    {
        protected IDbContextFactory<AdjustmentDbContext> _factory;
        //private IManageBillingService _manageBillingService;

        public BillingGenerator(IDbContextFactory<AdjustmentDbContext> factory)
        {
            _factory = factory;
            //_manageBillingService = manageBillingService;
        }

        public IEnumerable<string> CollectEEIdWithPcv(string cutoffId)
        {
            using AdjustmentDbContext context = _factory.CreateDbContext();
            IEnumerable<string> eeIds = context.Timesheets
                .Where(ts => !string.IsNullOrEmpty(ts.RawPCV) || ts.Allowance > 0)
                .Where(ts => ts.CutoffId == cutoffId)
                .Select(ts => ts.EEId)
                .ToList();

            return eeIds;
        }

        public IEnumerable<Billing> GenerateBillingFromRecords(string eeId, string cutoffId)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Billing> GenerateBillingFromTimesheetView(string eeId, string cutoffId)
        {
            using AdjustmentDbContext context = _factory.CreateDbContext();
            IEnumerable<TimesheetView> eeTimesheets = context.Timesheets
                .Where(ts => ts.EEId == eeId)
                .Where(ts => ts.CutoffId == cutoffId)
                .ToList();

            List<Billing> billings = new();

            var timesheetsWithAllowance = eeTimesheets.Where(ts => ts.Allowance > 0).ToList();
            foreach (TimesheetView timesheet in timesheetsWithAllowance)
            {
                var billing = new Billing()
                {
                    EEId = eeId,
                    CutoffId = cutoffId,
                    AdjustmentType = AdjustmentTypes.ALLOWANCE,
                    Amount = timesheet.Allowance,
                    AdjustmentOption = AdjustmentOptions.ADJUST1,
                    Deducted = true,
                    DateCreated = DateTime.Now
                };
                billing.BillingId = Billing.GenerateId(billing);
                billings.Add(billing);
            }

            var timesheetsWithPCV = eeTimesheets.Where(ts => !string.IsNullOrEmpty(ts.RawPCV)).ToList();
            foreach (TimesheetView timesheet in timesheetsWithPCV)
            {
                string[] rawPCVs = timesheet.RawPCV.Split("|");
                for (int i = 0; i < rawPCVs.Length; i++)
                {
                    string rawPCV = rawPCVs[i];
                    string[] rawPcvArgs = rawPCV.Split("~");
                    string remarks = rawPcvArgs[0];
                    double amount = double.Parse(rawPcvArgs[1]);

                    var billing = new Billing()
                    {
                        EEId = eeId,
                        CutoffId = cutoffId,
                        AdjustmentType = AdjustmentTypes.PCV,
                        Amount = amount,
                        AdjustmentOption = AdjustmentOptions.ADJUST1,
                        Deducted = true,
                        Remarks = remarks,
                        DateCreated = DateTime.Now
                    };
                    billing.BillingId = Billing.GenerateId(billing, i);
                    billings.Add(billing);
                }
            }

            return billings;
        }
    }
}
