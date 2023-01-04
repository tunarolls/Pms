using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Exceptions
{
    public class BillingAlreadyExistsException : Exception
    {
        public string BillingId { get; set; }
        public override string Message => "Billing already exists.";

        public BillingAlreadyExistsException(string billingId)
        {
            BillingId = billingId;
        }
    }
}
