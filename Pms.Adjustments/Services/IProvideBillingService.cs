using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.Services
{
    public interface IProvideBillingService
    {
        double GetTotalAdvances(string eeId, string cutoffId);
        //IEnumerable<Billing> GetBillings(string eeId, string cutoffId);
        IEnumerable<Billing> GetBillings(string cutoffId);
        Task<ICollection<Billing>> GetBillings(string cutoffId, CancellationToken cancellationToken = default);
        Task<ICollection<Billing>> GetBillings(string? cutoffId, string? payrollCode, CancellationToken cancellationToken = default);
        //IEnumerable<Billing> GetBillings(string cutoffId, string adjustmentName);
        //IEnumerable<string> GetAdjustmentNames(string cutoffId, string payrollCode);
    }
}
