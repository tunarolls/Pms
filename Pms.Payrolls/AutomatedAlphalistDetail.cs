using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
{
    public class AutomatedAlphalistDetail : IAlphalistDetail
    {
        public bool valid = true;

        #region EMPLOYEE INFORMATION
        private string EEId;
        private string Company = string.Empty;
        private string FirstName;
        private string LastName;
        private string MiddleName;
        private string TIN;

        private string StartDate = string.Empty;
        private string ResignationDate = string.Empty;
        private string Nationality { get; set; }
        private string ReasonForSeparation { get; set; }
        private string EmploymentStatus { get; set; }
        #endregion

        private int FactorUsed { get; set; }

        private double PresentTaxableSalary = 0;

        private double PresentTaxable13thMonth = 0;

        #region PerDay-specific Properties
        private double ActualHourlyRate = 0;
        private double PresentNonTaxableBasicSmwHour = 0;
        private double PresentNonTaxableBasicSmwDay => PresentNonTaxableBasicSmwHour * 8;
        private double PresentNonTaxableBasicSmwMonth
        {
            get
            {
                int i = 0;
                if (int.TryParse(PresentNonTaxableBasicSmwDay.ToString("0.00").Split(".")[0], out i))
                    return i * 24;
                else
                    return i;
            }
        }
        private double PresentNonTaxableBasicSmwYear => PresentNonTaxableBasicSmwMonth * 12;

        private double OvertimeAmount;

        private double RestDayOvertimeAmount;

        private double PresentNonTaxableHolidayPay;

        private double PresentNonTaxableNightDifferential;

        private double PresentNonTaxableOvertimePay => OvertimeAmount + RestDayOvertimeAmount;
        #endregion

        #region PAY
        private double GrossPay;
        //private double RegularPay;
        //private double NetPay;

        //private double Taxable13thMonth
        //{
        //    get
        //    {
        //        //if (NonTaxable13thMonth > 250000)
        //        //    return NonTaxable13thMonth - 250000;
        //        return 0;
        //    }
        //}
        private double PresentNonTaxable13thMonth;

        private double NonTaxableSalary
        {
            get
            {
                double nonTaxableSalary = GrossPay - (PresentNonTaxable13thMonth + PresentNonTaxableSssGsisOtherContribution);

                if (PresentNonTaxableBasicSmwHour < 70.25)
                    nonTaxableSalary -= (PresentNonTaxableNightDifferential + OvertimeAmount + RestDayOvertimeAmount + PresentNonTaxableHolidayPay);

                return nonTaxableSalary;
            }
        }

        private double PresentNonTaxableDeMinimis { get; set; } = 0;

        private double GrossCompensationIncome => NonTaxableSalary > 250000 ? NonTaxableSalary : 0;
        private double PresentNonTaxableSalary => GrossCompensationIncome;
        private double NetTaxableCompensationIncome => GrossCompensationIncome;
        private double PresentTotalCompensation => GrossCompensationIncome;

        private double TaxableBasicSalary => NonTaxableSalary > 250000 ? NonTaxableSalary : 0;

        private double PresentTotalNonTaxableCompensationIncome => NonTaxableSalary + PresentNonTaxable13thMonth + PresentNonTaxableSssGsisOtherContribution;
        private double PresentNonTaxableGrossCompensationIncome => NonTaxableSalary + PresentNonTaxable13thMonth + PresentNonTaxableSssGsisOtherContribution;
        #endregion

        #region PresentNonTaxableSssGsisOtherContribution
        private double EmployeeSSS;
        private double EmployeePhilHealth;
        private double EmployeePagibig;
        private double PresentNonTaxableSssGsisOtherContribution => EmployeeSSS + EmployeePhilHealth + EmployeePagibig;
        #endregion

        #region TAX
        private double WithholdingTax = 0;
        private double DecemberTaxWithheld = 0;
        private double PresentTaxWithheld => WithholdingTax - DecemberTaxWithheld;

        private double TaxDue
        {
            get
            {
                double taxableSalary = NonTaxableSalary;
                switch (taxableSalary)
                {
                    case var case3 when case3 >= 8000000:
                        return 2410000 + (taxableSalary - 8000000) * 0.35;
                    case var case4 when case4 >= 2000000:
                        return 490000 + (taxableSalary - 2000000) * 0.32;
                    case var case5 when case5 >= 800000:
                        return 130000 + (taxableSalary - 800000) * 0.3;
                    case var case6 when case6 >= 400000:
                        return 30000 + (taxableSalary - 400000) * 0.25;
                    case var case7 when case7 >= 250000:
                        return 0 + (taxableSalary - 250000) * 0.2;
                    case var case8 when case8 <= 250000:
                        return 0;
                }
                return 0;
            }
        }

        private double AmmountWithheldOnDecember
        {
            get
            {
                if (TaxDue > PresentTaxWithheld)
                    return TaxDue - PresentTaxWithheld;
                return 0;
            }
        }

        private double OverWithheld
        {
            get
            {
                if (TaxDue < PresentTaxWithheld)
                    return PresentTaxWithheld - TaxDue;
                return 0;
            }
        }

        private double ActualAmountWithheld
        {
            get
            {
                if (TaxDue > PresentTaxWithheld)
                    return PresentTaxWithheld + AmmountWithheldOnDecember;
                else
                    return PresentTaxWithheld - OverWithheld;
            }
        }

        private double NonTaxableBasicSalary { get; set; }
        private double PresentNonTaxableHazardPay { get; set; }

        #endregion

        public AutomatedAlphalistDetail(IEnumerable<Payroll> yearlyPayrolls, double minimumRate, int yearCovered)
        {
            Payroll payroll = yearlyPayrolls.First();

            ActualHourlyRate = payroll.Rate;

            PresentNonTaxableBasicSmwHour = payroll.Rate > minimumRate ? payroll.Rate : minimumRate;


            FactorUsed = payroll.Rate <= minimumRate ? 313 : 0;

            EmployeeView ee = payroll.EE;
            EEId = ee.EEId;
            FirstName = ee.FirstName.Replace("ñ", "N").Replace("Ñ", "N");
            LastName = ee.LastName.Replace("ñ", "N").Replace("Ñ", "N");
            MiddleName = ee.MiddleName.Replace("ñ", "N").Replace("Ñ", "N");
            TIN = ee.TIN.Replace("-","");
            Nationality = "FILIPINO";
            EmploymentStatus = "R";
            ReasonForSeparation = "";

            Payroll? previousDecemberPayroll = yearlyPayrolls.Where(py => py.Cutoff.CutoffDate.Month == 12 && py.Cutoff.CutoffDate.Year < yearCovered).FirstOrDefault();
            List<Payroll> JanToDecPayrolls = yearlyPayrolls.Where(py => py.Cutoff.CutoffDate.Year == yearCovered).ToList();
            List<Payroll> PreviousDecToJanPayrolls = yearlyPayrolls.Where(py => py.YearCovered == yearCovered).ToList();

            if (previousDecemberPayroll is null)
                previousDecemberPayroll = new();

            if (JanToDecPayrolls.Any())
            {
                StartDate = JanToDecPayrolls.First().Cutoff.CutoffDate.ToString("dd/MM/yyyy");
                ResignationDate = JanToDecPayrolls.Last().Cutoff.CutoffDate.ToString("dd/MM/yyyy");


                OvertimeAmount = JanToDecPayrolls.Sum(py => py.OvertimeAmount);
                RestDayOvertimeAmount = JanToDecPayrolls.Sum(py => py.RestDayOvertimeAmount);
                PresentNonTaxableHolidayPay = JanToDecPayrolls.Sum(py => py.HolidayOvertimeAmount);
                PresentNonTaxableNightDifferential = JanToDecPayrolls.Sum(py => py.NightDifferentialAmount);

                GrossPay = JanToDecPayrolls.Sum(py => py.GrossPay);

                EmployeeSSS = JanToDecPayrolls.Sum(py => py.EmployeeSSS);
                EmployeePagibig = JanToDecPayrolls.Sum(py => py.EmployeePagibig);
                EmployeePhilHealth = JanToDecPayrolls.Sum(py => py.EmployeePhilHealth);

                WithholdingTax = JanToDecPayrolls.Sum(py => py.WithholdingTax);
                valid = true;
            }
            else
                valid = false;
            DecemberTaxWithheld = previousDecemberPayroll.WithholdingTax;
            PresentNonTaxable13thMonth = PreviousDecToJanPayrolls.Sum(py => py.AdjustedRegPay) / 12;
            GrossPay += PresentNonTaxable13thMonth;
        }

        public AlphalistDetail CreateAlphalistDetail()
        {
            AlphalistDetail a = new();
            if (valid)
            {
                a.EEId = EEId;
                //a.Company = Company;
                a.FirstName = FirstName;
                a.LastName = LastName;
                a.MiddleName = MiddleName;
                a.Tin = TIN;
                a.StartDate = DateTime.Parse(StartDate);
                a.ResignationDate = DateTime.Parse(ResignationDate);
                a.AcutalAmountWithheld = ActualAmountWithheld;
                a.FactorUsed = FactorUsed;
                a.PresentTaxableSalary = PresentTaxableSalary;
                a.PresentTaxable13thMonth = PresentTaxable13thMonth;
                a.PresentTaxWithheld = PresentTaxWithheld;
                a.PresentNonTaxableSalary = PresentNonTaxableSalary;
                a.PresentNonTaxable13thMonth = PresentNonTaxable13thMonth;
                a.PresentNonTaxableSssGsisOtherContribution = PresentNonTaxableSssGsisOtherContribution;
                a.OverWithheld = OverWithheld;
                a.AmmountWithheldOnDecember = AmmountWithheldOnDecember;
                a.TaxDue = TaxDue;
                a.NetTaxableCompensationIncome = NetTaxableCompensationIncome;
                a.GrossCompensationIncome = GrossCompensationIncome;
                a.PresentNonTaxableDeMinimis = PresentNonTaxableDeMinimis;
                a.PresentTotalCompensation = PresentTotalCompensation;
                a.PresentTotalNonTaxableCompensationIncome = PresentTotalNonTaxableCompensationIncome;
                a.PresentNonTaxableGrossCompensationIncome = PresentNonTaxableGrossCompensationIncome;
                a.PresentNonTaxableBasicSmwHour = PresentNonTaxableBasicSmwHour;
                a.PresentNonTaxableBasicSmwDay = PresentNonTaxableBasicSmwDay;
                a.PresentNonTaxableBasicSmwMonth = PresentNonTaxableBasicSmwMonth;
                a.PresentNonTaxableBasicSmwYear = PresentNonTaxableBasicSmwYear;
                a.PresentNonTaxableHolidayPay = PresentNonTaxableHolidayPay;
                a.PresentNonTaxableOvertimePay = PresentNonTaxableOvertimePay;
                a.PresentNonTaxableNightDifferential = PresentNonTaxableNightDifferential;
                a.PresentNonTaxableHazardPay = PresentNonTaxableHazardPay;
                a.NonTaxableBasicSalary = NonTaxableBasicSalary;
                a.TaxableBasicSalary = TaxableBasicSalary;
                a.Nationality = Nationality;
                a.ReasonForSeparation = ReasonForSeparation;
                a.EmploymentStatus = EmploymentStatus;
                a.December = DecemberTaxWithheld;
                a.Final = OverWithheld;
            }
            Console.WriteLine(EEId);
            return a;
        }
    }
}
