using System;
using System.Collections.Generic;
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
        public EmployeeView EE { get; set; }
        public string EEId { get; set; } = string.Empty;
        public BillingRecord Record { get; set; }
        public string RecordId { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public static string GenerateId(Billing bil, int iterator = 0) =>
            $"{(!string.IsNullOrEmpty(bil.RecordId) ? bil.RecordId : $"{bil.EEId}_{bil.AdjustmentType}")}_" +
            $"{bil.CutoffId}_{iterator}";

        public override string ToString() => !string.IsNullOrEmpty(BillingId) ? BillingId : "EMPTY BILLING";
    }
}
