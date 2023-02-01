using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models
{
    public class Billing
    {
        public AdjustmentOptions AdjustmentOption { get; set; }
        public AdjustmentTypes AdjustmentType { get; set; }
        public double Amount { get; set; }
        public bool Applied { get; set; }
        public string BillingId { get; set; } = string.Empty;
        public string CutoffId { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public EmployeeView? EE { get; set; }
        public string? EEId { get; set; }
        public BillingRecord? Record { get; set; }
        public string? RecordId { get; set; }
        public string? Remarks { get; set; }

        public static string GenerateId(Billing billing, int iterator = 0)
        {
            return $"{(!string.IsNullOrEmpty(billing.RecordId) ? billing.RecordId : $"{billing.EEId}_{billing.AdjustmentType}")}_" + $"{billing.CutoffId}_{iterator}";
        }

        public override string ToString() => !string.IsNullOrEmpty(BillingId) ? BillingId : "EMPTY BILLING";
    }
}
