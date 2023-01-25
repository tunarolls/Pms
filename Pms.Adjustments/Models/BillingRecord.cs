using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models
{
    public class BillingRecord : IBillingRecord
    {
        public AdjustmentOptions AdjustmentOption { get; set; }
        public AdjustmentTypes AdjustmentType { get; set; }
        public double Advances { get; set; }
        public double Amortization { get; set; }
        public double Balance { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DeductionOptions DeductionOption { get; set; }
        public EmployeeView EE { get; set; }
        public string EEId { get; set; } = string.Empty;
        public DateTime EffectivityDate { get; set; }
        public string RecordId { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty; 
        public BillingRecordStatus Status { get; set; }
        public static string GenerateId(BillingRecord rec) => $"{rec.EEId}_{rec.AdjustmentType}_{rec.EffectivityDate:MMyy}";

        public void Validate()
        {
            if (string.IsNullOrEmpty(EEId))
                throw new Exception("EEId should not be blank.");

            if (string.IsNullOrEmpty(RecordId))
                throw new Exception("RecordId should not be blank.");

            //if (Balance > Advances)
            //    throw new Exception("Remaining Balance should not be greater than Advances.");

            if (Amortization > Advances)
                throw new Exception("Monthly Amortization should not be greater than Advances.");
        }
    }
}
