using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.Services
{
    public interface IGenerateBillingService
    {
        IEnumerable<Billing> GenerateBillingFromTimesheetView(string eeId, string cutoffId);
        Task<ICollection<Billing>> GenerateBillingFromTimesheetView(string eeId, string cutoffId, CancellationToken cancellationToken = default);
        IEnumerable<Billing> GenerateBillingFromRecords(string eeId, string cutoffId);
        Task<ICollection<Billing>> GenerateBillingFromRecords(string eeId, string cutoffId, CancellationToken cancellationToken = default);
        IEnumerable<string?> CollectEEIdWithPcv(string payrollCodeId, string cutoffId);
        Task<ICollection<string?>> CollectEEIdWithPcv(string payrollCode, string cutoffId, CancellationToken cancellationToken = default);
        IEnumerable<string?> CollectEEIdWithBillingRecord(string payrollCodeId, string cutoffId);
        Task<ICollection<string?>> CollectEEIdWithBillingRecord(string payrollCode, CancellationToken cancellationToken = default);
    }
}
