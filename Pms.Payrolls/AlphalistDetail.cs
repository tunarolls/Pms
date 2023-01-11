using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
{
    public class AlphalistDetail
    {
        public double AcutalAmountWithheld { get; set; }
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public double AmmountWithheldOnDecember { get; set; }
        public string AtcCode { get; set; } = string.Empty;
        public string AtcDescription { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public DateTime DateOfDeath { get; set; }
        public DateTime DateWithheld { get; set; }
        public double December { get; set; }
        public string EEId { get; set; } = string.Empty;
        public string EmployerBranchCode { get; set; } = string.Empty;
        public string EmployerTin { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public double ExmpnAmount { get; set; } = 0;
        public string ExmpnCode { get; set; } = string.Empty;
        public double FactorUsed { get; set; } = 313;
        public double Final { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string FormType { get; set; } = string.Empty;
        public double FringeBenefit { get; set; } = 0;
        public double GrossCompensationIncome { get; set; }
        public double HeathPremium { get; set; } = 0;
        public double IncomePayment { get; set; } = 0;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public double MonetaryValue { get; set; } = 0;
        public string Nationality { get; set; } = string.Empty;
        public double NetTaxableCompensationIncome { get; set; }
        public double NonTaxableBasicSalary { get; set; }
        public double OverWithheld { get; set; }
        public double PresentNonTaxable13thMonth { get; set; }
        public double PresentNonTaxableBasicSmwDay { get; set; } = 0;
        public double PresentNonTaxableBasicSmwHour { get; set; }
        public double PresentNonTaxableBasicSmwMonth { get; set; } = 0;
        public double PresentNonTaxableBasicSmwYear { get; set; } = 0;
        public double PresentNonTaxableDeMinimis { get; set; } = 0;
        public double PresentNonTaxableGrossCompensationIncome { get; set; }
        public double PresentNonTaxableHazardPay { get; set; } = 0;
        public double PresentNonTaxableHolidayPay { get; set; } = 0;
        public double PresentNonTaxableNightDifferential { get; set; }
        public double PresentNonTaxableOvertimePay { get; set; } = 0;
        public double PresentNonTaxableSalary { get; set; }
        public double PresentNonTaxableSssGsisOtherContribution { get; set; }
        public double PresentTaxable13thMonth { get; set; }
        public double PresentTaxableBasicSalary { get; set; } = 0;
        public double PresentTaxableSalary { get; set; }
        public double PresentTaxWithheld { get; set; }
        public double PresentTotalCompensation { get; set; }
        public double PresentTotalNonTaxableCompensationIncome { get; set; }
        public double PreviousAndPresentTotalCompensationIncome { get; set; } = 0;
        public double PreviousAndPresentTotalTaxable { get; set; } = 0;
        public double PreviousNonTaxable13thMonth { get; set; } = 0;
        public double PreviousNonTaxableBasicSmw { get; set; } = 0;
        public double PreviousNonTaxableDeMinimis { get; set; } = 0;
        public double PreviousNonTaxableGrossCompensationIncome { get; set; }
        public double PreviousNonTaxableHazardPay { get; set; } = 0;
        public double PreviousNonTaxableHolidayPay { get; set; } = 0;
        public double PreviousNonTaxableNightDifferential { get; set; } = 0;
        public double PreviousNonTaxableOvertimePay { get; set; } = 0;
        public double PreviousNonTaxableSalary { get; set; } = 0;
        public double PreviousNonTaxableSssGsisOtherContribution { get; set; } = 0;
        public double PreviousTaxable13thMonth { get; set; } = 0;
        public double PreviousTaxableBasicSalary { get; set; } = 0;
        public double PreviousTaxableSalary { get; set; } = 0;
        public double PreviousTaxWithheld { get; set; } = 0;
        public double PreviousTotalNonTaxableCompensationIncome { get; set; } = 0;
        public double PreviousTotalTaxable { get; set; } = 0;
        public int QrtNumber { get; set; } = 0;
        public DateTime QuarterDate { get; set; }
        public string ReasonForSeparation { get; set; } = string.Empty;
        public string RegionNumber { get; set; } = string.Empty;
        public string RegisteredName { get; set; } = string.Empty;
        public DateTime ResignationDate { get; set; }
        public DateTime ReturnPeriod { get; set; }
        public string ScheduleNumber { get; set; } = string.Empty;
        public double SequenceNumber { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string SubsFiling { get; set; } = string.Empty;
        public double TaxableBasicSalary { get; set; }
        public double TaxDue { get; set; }
        public double TaxRate { get; set; } = 0;
        public string Tin { get; set; } = string.Empty;
        public double TotalNontaxableCompensationIncome { get; set; } = 0;
        public double TotalTaxableCompensationIncome { get; set; } = 0;
        //public string Company { get; set; } = string.Empty;
    }
}
