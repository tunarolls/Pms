using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.Services
{
    public interface IManageBillingService
    {
        void AddBilling(Billing billing);
        Task AddBilling(Billing billing, CancellationToken cancellationToken = default);
        void ResetBillings(string eeId, string cutoffId);
        Task ResetBillings(string eeId, string cutoffId, CancellationToken cancellationToken = default);
    }
}
