using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class EmployeeView
    {
        public TimesheetBankChoices Bank { get; private set; }
        public string EEId { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
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

        public string LastName { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string MiddleName { get; private set; } = string.Empty;
        public string NameExtension { get; private set; } = string.Empty;
        public string PayrollCode { get; private set; } = string.Empty;
    }
}
