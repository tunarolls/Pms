using Pms.Masterlists.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Entities
{
    public interface IBankInformation
    {
        string AccountNumber { get; set; }
        BankChoices Bank { get; set; }
        string BankSetter { set; }
        string CardNumber { get; set; }
        string EEId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string MiddleName { get; set; }
        string NameExtension { get; set; }
        string PayrollCode { get; set; }
    }
}
