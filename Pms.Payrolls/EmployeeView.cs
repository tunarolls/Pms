using Pms.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
{
    public class EmployeeView
    {
        public string EEId { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string MiddleName { get; private set; } = string.Empty;
        public string NameExtension { get; private set; } = string.Empty;

        public string Pagibig { get; set; } = string.Empty;
        public string PhilHealth { get; set; } = string.Empty;
        public string SSS { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        public string MiddleInitial => !string.IsNullOrEmpty(MiddleName) ? MiddleName[0].ToString() : string.Empty;

        public string Fullname
        {
            get
            {
                string lastname = LastName;
                string firstname = FirstName != string.Empty ? $", {FirstName}" : "";
                string middleInitial = MiddleName != string.Empty ? $" {MiddleName?[0]}" : "";
                string nameExtension = NameExtension != string.Empty ? $" {NameExtension}" : "";
                string fullName = $"{lastname}{firstname}{middleInitial}{nameExtension}.";

                return fullName;
            }
        }

        /// <summary>
        /// FML - {Firstname} {Middle Initial}. {Lastname}
        /// </summary>
        public string Fullname_FML
        {
            get
            {
                string firstname = FirstName is not null && FirstName != string.Empty ? $"{FirstName}" : "";
                string middleInitial = MiddleName is not null && MiddleName != string.Empty ? $" {MiddleName?[0]}." : "";
                string lastname = LastName is not null && LastName != string.Empty ? $" {LastName}" : "";
                string nameExtension = NameExtension is not null && NameExtension != string.Empty ? $" {NameExtension}" : "";
                string fullName = $"{firstname}{middleInitial}{lastname}{nameExtension}";

                return fullName;
            }
        }


        public string Location { get; private set; } = string.Empty;

        public string JobCode { get; private set; } = string.Empty;

        public string PayrollCode { get; private set; } = string.Empty;

        public string AccountNumber { get; private set; } = string.Empty;
        public string CardNumber { get; private set; } = string.Empty;

        public bool Active { get; private set; }
        public BankChoices Bank { get; private set; }

    }
}
