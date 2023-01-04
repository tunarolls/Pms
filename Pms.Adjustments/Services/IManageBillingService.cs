using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Services
{
    public interface IManageBillingService
    {
        void AddBilling(Billing billing);
        void ResetBillings(string eeId, string cutoffId);
    }
}
