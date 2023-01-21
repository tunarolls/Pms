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
        public string EEId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MiddleName { get; private set; }
        public string NameExtension { get; private set; } = "";

        public string Pagibig { get; set; }
        public string PhilHealth { get; set; }
        public string SSS { get; set; }
        public string TIN { get; set; }
        public DateTime BirthDate { get; set; }

        public string MiddleInitial => MiddleName != string.Empty ? MiddleName?[0].ToString() : "";

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


        public string Location { get; private set; }

        public string JobCode { get; private set; }

        public string PayrollCode { get; private set; }

        public string AccountNumber { get; private set; }
        public string CardNumber { get; private set; }

        public bool Active { get; private set; }
        public BankChoices Bank { get; private set; }

    }
}
