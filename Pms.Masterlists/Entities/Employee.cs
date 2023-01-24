using Newtonsoft.Json;
using Pms.Common.Enums;
using Pms.Masterlists.Enums;
using Pms.Masterlists.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pms.Masterlists.Entities
{
    public class Employee : IHRMSInformation, IBankInformation, IGovernmentInformation, IEEDataInformation, IActive, IMasterFileInformation
    {
        #region COMPANY
        public bool Active { get; set; } = true;

        public string CompanyId { get; set; } = string.Empty;

        public DateTime DateHired { get; set; }

        [JsonProperty("joined_date")]
        public string DateHiredSetter
        {
            set
            {
                if (value == "" || value == "0000-00-00")
                {
                    DateHired = default;
                }
                else
                {
                    if (DateTime.TryParse(value, out DateTime dateHired))
                    {
                        DateHired = dateHired;
                    }
                    else if (DateTime.TryParseExact(value, new string[] { "MM/dd/yyyy", "M/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateHired))
                    {
                        DateHired = dateHired;
                    }
                    else
                    {
                        DateHired = DateTime.Parse(value);
                    }
                }
            }
        }

        public DateTime DateResigned { get; set; }

        [JsonProperty("terminated_date")]
        public string DateResignedSetter
        {
            set
            {
                if (value == "" || value == "0000-00-00")
                {
                    DateResigned = default;
                }
                else
                {
                    if (DateTime.TryParse(value, out DateTime dateResigned))
                    {
                        DateResigned = dateResigned;
                    }
                    else if (DateTime.TryParseExact(value, new string[] { "MM/dd/yyyy", "M/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateResigned))
                    {
                        DateResigned = dateResigned;
                    }
                    else
                    {
                        DateResigned = DateTime.Parse(value);
                    }
                }
            }
        }

        [JsonProperty("idno")]
        public string EEId { get; set; } = string.Empty;

        [JsonProperty("jobcode")]
        public string JobCode { get; set; } = string.Empty;

        [JsonProperty("job_remarks")]
        public string JobRemarks { get; set; } = string.Empty;

        [JsonProperty("department")]
        public string Location { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
        #endregion

        #region PERSONAL
        public DateTime BirthDate { get; set; }

        [JsonProperty("birthdate")]
        public string BirthDateSetter
        {
            set
            {
                if (value == "" || value == "0000-00-00")
                {
                    BirthDate = default;
                }
                else
                {
                    if (DateTime.TryParse(value, out DateTime birthDate))
                    {
                        BirthDate = birthDate;
                    }
                    else if (DateTime.TryParseExact(value, new string[] { "MM/dd/yyyy", "M/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                    {
                        BirthDate = birthDate;
                    }
                    else
                    {
                        BirthDate = DateTime.Parse(value);
                    }
                }
            }
        }

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;

        public string Fullname
        {
            get
            {
                string lastname = LastName;
                string firstname = FirstName != string.Empty ? $", {FirstName}" : "";
                string middleInitial = MiddleName != string.Empty ? $" {MiddleName[0]}" : "";
                string nameExtension = NameExtension != string.Empty ? $" {NameExtension}" : "";
                string fullName = $"{lastname}{firstname}{middleInitial}{nameExtension}.";

                return fullName;
            }
        }

        public string Gender { get; set; } = string.Empty;

        [JsonProperty("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("middle_name")]
        public string MiddleName { get; set; } = string.Empty;

        public string NameExtension { get; set; } = string.Empty;
        #endregion

        #region GOVERNMENT
        [JsonProperty("pagibig")]
        public string Pagibig { get; set; } = string.Empty;

        [JsonProperty("philhealth")]
        public string PhilHealth { get; set; } = string.Empty;

        [JsonProperty("sss")]
        public string SSS { get; set; } = string.Empty;

        [JsonProperty("tin")]
        public string TIN { get; set; } = string.Empty;

        #endregion

        #region BANK
        [JsonProperty("account_number")]
        public string AccountNumber { get; set; } = string.Empty;

        public BankChoices Bank { get; set; }
        [JsonProperty("bank_category")]
        public string BankCategorySetter
        {
            set
            {
                value = value.ToUpper();

                if (value == "CHK" || value == "CHECK" || value == "CHEQUE")
                {
                    Bank = BankChoices.CHK;
                }
            }
        }

        public string BankSetter
        {
            set
            {
                value = value.ToUpper();
                if (value == "LANDBANK" || value == "LBP") Bank = BankChoices.LBP;
                else if (value == "CHINABANK" || value == "CBC") Bank = BankChoices.CBC;
                else if (value == "CBC1") Bank = BankChoices.CBC1;
                else if (value == "MPALO") Bank = BankChoices.MPALO;
                else if (value == "MTAC") Bank = BankChoices.MTAC;
                else if (value == "CHK" || value == "CHECK" || value == "CHEQUE") Bank = BankChoices.CHK;
                else if (value == "UCPB") Bank = BankChoices.UCPB;
                else Bank = BankChoices.UNKNOWN;
            }
        }

        [JsonProperty("card_number")]
        public string CardNumber { get; set; } = string.Empty;

        public string PayrollCode { get; set; } = string.Empty;

        [JsonProperty("payroll_code")]
        public string PayrollCodeSetter
        {
            set
            {
                value = value.ToUpper();
                string[] valueArgs = value.Split("-");
                PayrollCode = valueArgs[0];

                if (valueArgs.Length > 1)
                {
                    BankSetter = valueArgs[1];
                }
            }
        }
        #endregion

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public void ValidateAll()
        {
            List<InvalidFieldValueException> exceptions = new();
            exceptions.AddRange(ValidatePersonalInformation(false));
            exceptions.AddRange(ValidateBankInformation(false));
            exceptions.AddRange(ValidateGovernmentInformation(false));

            if (exceptions.Any()) throw new InvalidFieldValuesException(EEId, exceptions);
        }

        public List<InvalidFieldValueException> ValidateBankInformation(bool throwsException = true)
        {
            List<InvalidFieldValueException> exceptions = new();
            if (!Active) return exceptions; // Ignore bank information if already resigned.

            if (Bank != BankChoices.CHK && AccountNumber == string.Empty)
            {
                exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId, "Should not be blank."));
                return exceptions;
            }

            if (AccountNumber.Any(char.IsLetter))
            {
                exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId));
            }

            switch (Bank)
            {
                case BankChoices.LBP:
                    if (!string.IsNullOrEmpty(CardNumber))
                    {
                        if (CardNumber.Length != 16)
                        {
                            exceptions.Add(new InvalidFieldValueException(nameof(CardNumber), CardNumber, EEId));
                        }
                        if (CardNumber.Any(char.IsLetter))
                        {
                            exceptions.Add(new InvalidFieldValueException(nameof(CardNumber), CardNumber, EEId));
                        }
                    }
                    else
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(CardNumber), CardNumber, EEId, "Should not be blank."));
                    }
                    if (AccountNumber.Length != 19)
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId));
                    }
                    if (!AccountNumber.Contains("19372"))
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId, "LBP Account numbers contains '19372'."));
                    }
                    break;
                case BankChoices.CBC:
                case BankChoices.CBC1:
                    var validLengths = new int[] { 10, 12, 18, 19 };
                    if (!validLengths.Contains(AccountNumber.Length))
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId, "CBC/CBC1 only accepts 10, 12, 18, 19 digit account numbers."));
                    }
                    break;
                case BankChoices.MTAC:
                    if (AccountNumber.Length != 13)
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId));
                    }
                    if (AccountNumber.Substring(0, 3) != "525")
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId, "MTAC Account numbers have leading '525'."));
                    }
                    break;
                case BankChoices.MPALO:
                    if (AccountNumber.Length != 13)
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId));
                    }
                    if (AccountNumber.Substring(0, 3) != "756")
                    {
                        exceptions.Add(new InvalidFieldValueException(nameof(AccountNumber), AccountNumber, EEId, "MPALO Account numbers have leading '756'."));
                    }
                    break;
                case BankChoices.UCPB:
                    exceptions.Add(new InvalidFieldValueException(nameof(Bank), Bank.ToString(), EEId));
                    break;
                case BankChoices.UNKNOWN:
                    exceptions.Add(new InvalidFieldValueException(nameof(Bank), Bank.ToString(), EEId));
                    break;
            }

            if (exceptions.Any() && throwsException)
            {
                throw new InvalidFieldValuesException(EEId, exceptions);
            }

            return exceptions;
        }

        public List<InvalidFieldValueException> ValidateGovernmentInformation(bool throwsException = true)
        {
            List<InvalidFieldValueException> exceptions = new();

            if (!string.IsNullOrEmpty(Pagibig))
            {
                if (!Regex.IsMatch(Pagibig, @"^(\d{4}-\d{4}-\d{4}|[0-9]{12})$"))
                    exceptions.Add(new InvalidFieldValueException(nameof(Pagibig), Pagibig, EEId));
                if (Pagibig.Any(char.IsLetter)) exceptions.Add(new InvalidFieldValueException(nameof(Pagibig), Pagibig, EEId));
            }

            if (!string.IsNullOrEmpty(PhilHealth))
            {
                if (!Regex.IsMatch(PhilHealth, @"^(\d{2}-\d{9}-\d|[0-9]{12})$"))
                    exceptions.Add(new InvalidFieldValueException(nameof(PhilHealth), PhilHealth, EEId));
                if (PhilHealth.Any(char.IsLetter)) exceptions.Add(new InvalidFieldValueException(nameof(PhilHealth), PhilHealth, EEId));
            }

            if (!string.IsNullOrEmpty(SSS))
            {
                if (!Regex.IsMatch(SSS, @"^(\d{2}-\d{7}-\d|[0-9]{10})$"))
                    exceptions.Add(new InvalidFieldValueException(nameof(SSS), SSS, EEId));
                if (SSS.Any(char.IsLetter)) exceptions.Add(new InvalidFieldValueException(nameof(SSS), SSS, EEId));
            }

            if (!string.IsNullOrEmpty(TIN))
            {
                if (!Regex.IsMatch(TIN, @"^(\d{3}-\d{2}-\d{4}|\d{3}-\d{3}-\d{3}|\d{3}-\d{3}-\d{3}-\d{3}|[0-9]{9}|[0-9]{9}0{1,4})$"))
                    exceptions.Add(new InvalidFieldValueException(nameof(TIN), TIN, EEId));
                if (TIN.Any(char.IsLetter)) exceptions.Add(new InvalidFieldValueException(nameof(TIN), TIN, EEId));
            }

            if (exceptions.Any() && throwsException) throw new InvalidFieldValuesException(EEId, exceptions);

            return exceptions;
        }

        public List<InvalidFieldValueException> ValidatePersonalInformation(bool throwsException = true)
        {
            List<InvalidFieldValueException> exceptions = new();

            if (!string.IsNullOrEmpty(EEId))
            {
                if (EEId.Length < 3) exceptions.Add(new InvalidFieldValueException(nameof(EEId), EEId, EEId));
                if (EEId.Length > 4) exceptions.Add(new InvalidFieldValueException(nameof(EEId), EEId, EEId));
                //if (EEId.Any(char.IsLower)) throw new InvalidEmployeeFieldValueException(nameof(EEId), EEId, EEId);
            }
            else
                throw new InvalidFieldValueException(nameof(EEId), EEId, EEId);

            if (!string.IsNullOrEmpty(FirstName))
            {
                if (FirstName.Length < 2) exceptions.Add(new InvalidFieldValueException(nameof(FirstName), FirstName, EEId));
                if (FirstName.Length > 45) exceptions.Add(new InvalidFieldValueException(nameof(FirstName), FirstName, EEId));
                if (FirstName.Any(char.IsDigit)) exceptions.Add(new InvalidFieldValueException(nameof(FirstName), FirstName, EEId));
                //if (FirstName.Any(char.IsLower)) throw new InvalidEmployeeFieldValueException(nameof(FirstName), FirstName, EEId);
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                if (LastName.Length < 2) exceptions.Add(new InvalidFieldValueException(nameof(LastName), LastName, EEId));
                if (LastName.Length > 45) exceptions.Add(new InvalidFieldValueException(nameof(LastName), LastName, EEId));
                if (LastName.Any(char.IsDigit)) exceptions.Add(new InvalidFieldValueException(nameof(LastName), LastName, EEId));
                //if (LastName.Any(char.IsLower)) throw new InvalidEmployeeFieldValueException(nameof(LastName), LastName, EEId);
            }

            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (MiddleName.Length > 45) exceptions.Add(new InvalidFieldValueException(nameof(MiddleName), MiddleName, EEId));
                if (MiddleName.Any(char.IsDigit)) exceptions.Add(new InvalidFieldValueException(nameof(MiddleName), MiddleName, EEId));
                //if (MiddleName.Any(char.IsLower)) throw new InvalidEmployeeFieldValueException(nameof(MiddleName), MiddleName, EEId);
            }

            if (exceptions.Any() && throwsException)
                throw new InvalidFieldValuesException(EEId, exceptions);

            return exceptions;
        }
    }
}
