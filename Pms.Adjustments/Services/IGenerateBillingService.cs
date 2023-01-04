using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Services
{
    public interface IGenerateBillingService
    {
        IEnumerable<Billing> GenerateBillingFromTimesheetView(string eeId, string cutoffId);
        IEnumerable<Billing> GenerateBillingFromRecords(string eeId, string cutoffId);

        IEnumerable<string> CollectEEIdWithPcv(string cutoffId);
    }
}
