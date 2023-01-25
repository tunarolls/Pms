using Pms.Adjustments;
using Pms.Adjustments.Models;
using Pms.Adjustments.ServiceLayer.Files;
using Pms.Adjustments.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.Module.Models
{
    public class Billings
    {
        private readonly IManageBillingService _billingManager;
        private readonly IProvideBillingService _billingProvider;
        private readonly IGenerateBillingService _billingGenerator;


        public Billings(IManageBillingService manageBilling, IProvideBillingService provideBilling, IGenerateBillingService generateBilling)
        {
            _billingManager = manageBilling;
            _billingProvider = provideBilling;
            _billingGenerator = generateBilling;
        }


        public IEnumerable<Billing> GetBillings(string cutoffId)
        {
            return _billingProvider.GetBillings(cutoffId);
        }

        public async Task<ICollection<Billing>> GetBillings(string cutoffId, CancellationToken cancellationToken = default)
        {
            return await _billingProvider.GetBillings(cutoffId, cancellationToken);
        }

        public IEnumerable<Billing> GetBillings(string cutoffId, string payrollCodeId)
        {
            return _billingProvider.GetBillings(cutoffId).Where(p => p.EE.PayrollCode == payrollCodeId);
        }

        public IEnumerable<string> GetEmployeesWithPcv(string payrollCodeId, string cutoffId)
        {
            return _billingGenerator.CollectEEIdWithPcv(payrollCodeId, cutoffId);
        }

        public async Task<ICollection<string>> GetEmployeesWithPcv(string payrollCodeId, string cutoffId, CancellationToken cancellationToken = default)
        {
            return await _billingGenerator.CollectEEIdWithPcv(payrollCodeId, cutoffId, cancellationToken);
        }

        public IEnumerable<string> GetEmployeesWithBillingRecord(string payrollCodeId, string cutoffId)
        {
            return _billingGenerator.CollectEEIdWithBillingRecord(payrollCodeId, cutoffId);
        }

        public async Task<ICollection<string>> GetEmployeesWithBillingRecord(string payrollCodeId, string cutoffId, CancellationToken cancellationToken = default)
        {
            return await _billingGenerator.CollectEEIdWithBillingRecord(payrollCodeId, cutoffId, cancellationToken);
        }

        public double GetTotalAdvances(string eeId, string cutoffId)
        {
            return _billingProvider.GetTotalAdvances(eeId, cutoffId);
        }

        public void ResetBillings(string cutoffId, string eeId)
        {
            _billingManager.ResetBillings(eeId, cutoffId);
        }

        public async Task ResetBillings(string cutoffId, string eeId, CancellationToken cancellationToken = default)
        {
            await _billingManager.ResetBillings(eeId, cutoffId, cancellationToken);
        }

        public IEnumerable<Billing> GenerateBillingFromTimesheetView(string cutoffId, string eeId)
        {
            return _billingGenerator.GenerateBillingFromTimesheetView(eeId, cutoffId);
        }

        public async Task<ICollection<Billing>> GenerateBillingFromTimesheetView(string cutoffId, string eeId, CancellationToken cancellationToken = default)
        {
            return await _billingGenerator.GenerateBillingFromTimesheetView(eeId, cutoffId, cancellationToken);
        }

        public IEnumerable<Billing> GenerateBillingFromBillingRecord(string cutoffId, string eeId)
        {
            return _billingGenerator.GenerateBillingFromRecords(eeId, cutoffId);
        }

        public async Task<ICollection<Billing>> GenerateBillingFromBillingRecord(string cutoffId, string eeId, CancellationToken cancellationToken = default)
        {
            return await _billingGenerator.GenerateBillingFromRecords(eeId, cutoffId, cancellationToken);
        }

        public void AddBilling(Billing billing)
        {
            billing.EE = null;
            _billingManager.AddBilling(billing);
        }

        public async Task AddBilling(Billing billing, CancellationToken cancellationToken = default)
        {
            await _billingManager.AddBilling(billing, cancellationToken);
        }

        public void Export(IEnumerable<Billing> billings, string cutoffId, string payrollCodeId, AdjustmentTypes adjustmentName)
        {
            BillingExporter.ExportBillings(billings, cutoffId, payrollCodeId, adjustmentName);
        }

        public async Task Export(IEnumerable<Billing> billings, string cutoffId, string payrollCodeId, AdjustmentTypes adjustmentName, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                BillingExporter.ExportBillings(billings, cutoffId, payrollCodeId, adjustmentName);
            }, cancellationToken);
        }
    }
}
