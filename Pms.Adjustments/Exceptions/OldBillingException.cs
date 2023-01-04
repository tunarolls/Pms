using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Exceptions
{
    public class OldBillingException : Exception
    {
        public string BillingId { get; set; }
        public string CutoffId { get; set; }

        public OldBillingException(string billingId, string cutoffId)
        {
            BillingId = billingId;
            CutoffId = cutoffId;
        }
    }
}
