using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Exceptions
{
    public class UnknownBankException : Exception
    {
        public UnknownBankException(string bank)
        {
            Bank = bank;
        }

        public string Bank { get; set; }
    }
}
