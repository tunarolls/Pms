using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.ServiceLayer.EfCore
{
    public static class BillingProviderExtensions
    {
        public static IEnumerable<AdjustmentTypes> ExtractAdjustmentNames(this IEnumerable<Billing> billings)
        {
            return billings
                .GroupBy(b => b.AdjustmentType)
                .Select(n => n.First())
                .OrderBy(b => b.AdjustmentType)
                .Select(b => b.AdjustmentType)
                .ToList();
        }

        public static IQueryable<Billing> FilterByCutoffId(this IQueryable<Billing> billings, string? cutoffId)
        {
            return !string.IsNullOrEmpty(cutoffId)
                ? billings.Where(t => t.CutoffId == cutoffId)
                : billings;
        }

        public static IQueryable<Billing> FilterByPayrollCode(this IQueryable<Billing> billings, string? payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? billings.Where(t => t.EE != null && t.EE.PayrollCode == payrollCode)
                : billings;
        }

        public static IQueryable<TimesheetView> FilterByCutoffId(this IQueryable<TimesheetView> timesheets, string? cutoffId)
        {
            return !string.IsNullOrEmpty(cutoffId)
                ? timesheets.Where(t => t.CutoffId == cutoffId)
                : timesheets;
        }

        public static IQueryable<TimesheetView> FilterByPayrollCode(this IQueryable<TimesheetView> timesheets, string? payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? timesheets.Where(t => t.EE != null && t.EE.PayrollCode == payrollCode)
                : timesheets;
        }

        public static IQueryable<BillingRecord> FilterByPayrollCode(this IQueryable<BillingRecord> records, string? payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode)
                ? records.Where(t => t.EE != null && t.EE.PayrollCode == payrollCode)
                : records;
        }
    }
}
