using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models
{
    public class BillingRecord : IBillingRecord
    {

        public string RecordId { get; set; } = string.Empty;

        public string EEId { get; set; } = string.Empty;
        public EmployeeView? EE { get; set; }

        public AdjustmentTypes AdjustmentType { get; set; }

        public double Balance { get; set; }
        public double Advances { get; set; }
        public double Amortization { get; set; }

        public DateTime EffectivityDate { get; set; }

        public DeductionOptions DeductionOption { get; set; }

        public BillingRecordStatus Status { get; set; }


        public DateTime DateCreated { get; set; }
    }
}
